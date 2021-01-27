using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVImage
    {
        //Constants
        private const uint ICO_VERSION = 0x00030000;
        private const uint LR_DEFAULTCOLOR = 0;

        //Interop
        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, LoadLibraryFlags dwFlags);

        [DllImport("kernel32.dll")]
        private static extern bool EnumResourceNames(IntPtr hModule, ResourceTypes lpType, EnumResNameProcDelegate lpEnumFunc, IntPtr lParam);
        private delegate bool EnumResNameProcDelegate(IntPtr hModule, ResourceTypes lpType, IntPtr lpEnumFunc, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern IntPtr FindResource(IntPtr hModule, string lpName, ResourceTypes lpType);

        [DllImport("kernel32.dll")]
        private static extern IntPtr FindResource(IntPtr hModule, IntPtr lpName, ResourceTypes lpType);

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll")]
        private static extern IntPtr LockResource(IntPtr hResData);

        [DllImport("kernel32.dll")]
        private static extern uint SizeofResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("user32.dll")]
        private static extern IntPtr CreateIconFromResourceEx(byte[] presbits, uint dwResSize, bool fIcon, uint dwVer, int cxDesired, int cyDesired, uint Flags);

        [DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr hIcon);

        //Methods
        private static string IntPtrToString(IntPtr intPtr)
        {
            try
            {
                if (intPtr.ToInt64() > ushort.MaxValue)
                {
                    return Marshal.PtrToStringAnsi(intPtr);
                }
                else
                {
                    return intPtr.ToString();
                }
            }
            catch { }
            return string.Empty;
        }

        private static IntPtr FindResourceString(IntPtr hModule, string lpName, ResourceTypes lpType)
        {
            try
            {
                if (int.TryParse(lpName, out int intResult))
                {
                    return FindResource(hModule, (IntPtr)intResult, lpType);
                }
                else
                {
                    return FindResource(hModule, lpName, lpType);
                }
            }
            catch { }
            return IntPtr.Zero;
        }

        private static byte[] GetResourceDataBytesFromIntPtr(IntPtr hModule, IntPtr lpName, ResourceTypes lpType)
        {
            try
            {
                IntPtr foundResource = FindResource(hModule, lpName, lpType);
                if (foundResource == IntPtr.Zero)
                {
                    return null;
                }

                uint sizeResource = SizeofResource(hModule, foundResource);
                if (sizeResource == 0)
                {
                    return null;
                }

                IntPtr loadResource = LoadResource(hModule, foundResource);
                if (loadResource == IntPtr.Zero)
                {
                    return null;
                }

                IntPtr lockResource = LockResource(loadResource);
                if (lockResource == IntPtr.Zero)
                {
                    return null;
                }

                byte[] bytesResource = new byte[sizeResource];
                Marshal.Copy(lockResource, bytesResource, 0, bytesResource.Length);
                return bytesResource;
            }
            catch { }
            return null;
        }

        private static bool GetResourceDataIntPtrFromString(IntPtr hModule, string lpName, ResourceTypes lpType, out IntPtr data, out uint size)
        {
            try
            {
                IntPtr foundResource = FindResourceString(hModule, lpName, lpType);
                if (foundResource == IntPtr.Zero)
                {
                    data = IntPtr.Zero;
                    size = 0;
                    return false;
                }

                uint sizeResource = SizeofResource(hModule, foundResource);
                if (sizeResource == 0)
                {
                    data = IntPtr.Zero;
                    size = 0;
                    return false;
                }

                IntPtr loadResource = LoadResource(hModule, foundResource);
                if (loadResource == IntPtr.Zero)
                {
                    data = IntPtr.Zero;
                    size = 0;
                    return false;
                }

                IntPtr lockResource = LockResource(loadResource);
                if (lockResource == IntPtr.Zero)
                {
                    data = IntPtr.Zero;
                    size = 0;
                    return false;
                }
                else
                {
                    data = lockResource;
                    size = sizeResource;
                    return true;
                }
            }
            catch { }
            data = IntPtr.Zero;
            size = 0;
            return false;
        }

        public static MemoryStream GetIconMemoryStreamFromExeFile(string exeFilePath, int iconIndex, ref MemoryStream imageMemoryStream)
        {
            IntPtr iconHandle = IntPtr.Zero;
            IntPtr libraryHandle = IntPtr.Zero;
            try
            {
                //Load executable file library
                Debug.WriteLine("Loading exe icon: " + exeFilePath);
                libraryHandle = LoadLibraryEx(exeFilePath, IntPtr.Zero, LoadLibraryFlags.LOAD_LIBRARY_AS_DATAFILE);
                if (libraryHandle == IntPtr.Zero)
                {
                    Debug.WriteLine("Failed to load icon from exe: " + exeFilePath);
                    return null;
                }

                //Enumerate all icon groups
                List<string> iconGroups = new List<string>();
                EnumResNameProcDelegate EnumResourceNamesCallback = (IntPtr hModule, ResourceTypes lpType, IntPtr lpEnumFunc, IntPtr lParam) =>
                {
                    try
                    {
                        string intPtrString = IntPtrToString(lpEnumFunc);
                        if (!string.IsNullOrWhiteSpace(intPtrString))
                        {
                            iconGroups.Add(intPtrString);
                            return true;
                        }
                    }
                    catch { }
                    return false;
                };
                EnumResourceNames(libraryHandle, ResourceTypes.GROUP_ICON, EnumResourceNamesCallback, IntPtr.Zero);

                //Select target icon group
                int iconGroupsCount = iconGroups.Count;
                Debug.WriteLine("Total icon groups: " + iconGroupsCount);
                string iconGroup = iconGroups[iconIndex];

                //Get all icons from group
                List<MEMICONDIRENTRY> iconDirEntryList = new List<MEMICONDIRENTRY>();
                GetResourceDataIntPtrFromString(libraryHandle, iconGroup, ResourceTypes.GROUP_ICON, out IntPtr iconDirIntPtr, out uint iconDirResSize);
                unsafe
                {
                    MEMICONDIR* iconDir = (MEMICONDIR*)iconDirIntPtr;
                    MEMICONDIRENTRY* iconDirEntryArray = &iconDir->idEntriesArray;
                    Debug.WriteLine("Total icons in group: " + iconDir->idCount);
                    for (int entryId = 0; entryId < iconDir->idCount; entryId++)
                    {
                        try
                        {
                            iconDirEntryList.Add(iconDirEntryArray[entryId]);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("No icon for you!" + ex.Message);
                        }
                    }
                }

                //Select largest icon
                MEMICONDIRENTRY iconDirEntry = iconDirEntryList.OrderByDescending(x => x.dwBytesInRes).ThenByDescending(x => x.wBitCount).FirstOrDefault();

                //Get icon bitmap data
                IntPtr iconIdentifier = (IntPtr)iconDirEntry.nIdentifier;
                byte[] iconBytes = GetResourceDataBytesFromIntPtr(libraryHandle, iconIdentifier, ResourceTypes.ICON);
                Debug.WriteLine(iconIdentifier + " / " + iconGroup);

                //Encode icon bitmap frame
                if (iconBytes[0] == 0x28)
                {
                    Debug.WriteLine("BMP image: " + iconBytes.Length);

                    //Create icon from the resource
                    iconHandle = CreateIconFromResourceEx(iconBytes, (uint)iconBytes.Length, true, ICO_VERSION, iconDirEntry.bWidth, iconDirEntry.bHeight, LR_DEFAULTCOLOR);

                    //Convert image data to bitmap
                    Bitmap bitmapImage = Icon.FromHandle(iconHandle).ToBitmap();
                    Debug.WriteLine("w/" + bitmapImage.Width + " h/" + bitmapImage.Height);
                    Debug.WriteLine("BMP OK" + iconIdentifier);

                    //Write bitmap to memorystream
                    bitmapImage.Save(imageMemoryStream, ImageFormat.Png);
                    imageMemoryStream.Seek(0, SeekOrigin.Begin);
                    return imageMemoryStream;
                }
                else
                {
                    Debug.WriteLine("PNG image: " + iconBytes.Length);
                    using (MemoryStream memoryStream = new MemoryStream(iconBytes))
                    {
                        //Convert image data to bitmap
                        Bitmap bitmapImage = new Bitmap(memoryStream);
                        Debug.WriteLine("w/" + bitmapImage.Width + " h/" + bitmapImage.Height);
                        Debug.WriteLine("PNG OK" + iconIdentifier);

                        //Write bitmap to memorystream
                        bitmapImage.Save(imageMemoryStream, ImageFormat.Png);
                        imageMemoryStream.Seek(0, SeekOrigin.Begin);
                        return imageMemoryStream;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load exe icon: " + ex.Message);
                return null;
            }
            finally
            {
                if (libraryHandle != IntPtr.Zero)
                {
                    FreeLibrary(libraryHandle);
                }
                if (iconHandle != IntPtr.Zero)
                {
                    DestroyIcon(iconHandle);
                }
            }
        }
    }
}