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

        private static IntPtr GetResourceDataIntPtrFromString(IntPtr hModule, string lpName, ResourceTypes lpType)
        {
            try
            {
                IntPtr foundResource = FindResourceString(hModule, lpName, lpType);
                if (foundResource == IntPtr.Zero)
                {
                    return IntPtr.Zero;
                }

                uint sizeResource = SizeofResource(hModule, foundResource);
                if (sizeResource == 0)
                {
                    return IntPtr.Zero;
                }

                IntPtr loadResource = LoadResource(hModule, foundResource);
                if (loadResource == IntPtr.Zero)
                {
                    return IntPtr.Zero;
                }

                return LockResource(loadResource);
            }
            catch { }
            return IntPtr.Zero;
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
                string iconGroup = string.Empty;
                int iconGroupsCount = iconGroups.Count;
                //Debug.WriteLine("Total icon groups: " + iconGroupsCount);
                if (iconGroupsCount > 0 && iconGroupsCount >= iconIndex)
                {
                    iconGroup = iconGroups[iconIndex];
                }
                else
                {
                    Debug.WriteLine("No exe icon found to load.");
                    return null;
                }

                //Get all icons from group
                List<MEMICONDIRENTRY> iconDirEntryList = new List<MEMICONDIRENTRY>();
                IntPtr iconDirIntPtr = GetResourceDataIntPtrFromString(libraryHandle, iconGroup, ResourceTypes.GROUP_ICON);
                unsafe
                {
                    MEMICONDIR* iconDir = (MEMICONDIR*)iconDirIntPtr;
                    MEMICONDIRENTRY* iconDirEntryArray = &iconDir->idEntriesArray;
                    //Debug.WriteLine("Total icons in group: " + iconDir->idCount);
                    for (int entryId = 0; entryId < iconDir->idCount; entryId++)
                    {
                        try
                        {
                            iconDirEntryList.Add(iconDirEntryArray[entryId]);
                        }
                        catch { }
                    }
                }

                //Select largest icon
                MEMICONDIRENTRY iconDirEntry = iconDirEntryList.OrderByDescending(x => x.wBitCount).ThenByDescending(x => x.dwBytesInRes).FirstOrDefault();

                //Get icon bitmap data
                byte[] iconBytes = GetResourceDataBytesFromIntPtr(libraryHandle, (IntPtr)iconDirEntry.nIdentifier, ResourceTypes.ICON);

                //Encode icon bitmap frame
                if (iconBytes[0] == 0x28)
                {
                    //Debug.WriteLine("BMP image: " + iconBytes.Length);

                    //Create icon from the resource
                    iconHandle = CreateIconFromResourceEx(iconBytes, (uint)iconBytes.Length, true, IconVersion.Windows3x, iconDirEntry.bWidth, iconDirEntry.bHeight, IconResourceFlags.LR_DEFAULTCOLOR);

                    //Convert image data to bitmap
                    Bitmap bitmapImage = Icon.FromHandle(iconHandle).ToBitmap();

                    //Write bitmap to memorystream
                    bitmapImage.Save(imageMemoryStream, ImageFormat.Png);
                    imageMemoryStream.Seek(0, SeekOrigin.Begin);
                    return imageMemoryStream;
                }
                else
                {
                    //Debug.WriteLine("PNG image: " + iconBytes.Length);
                    using (MemoryStream memoryStream = new MemoryStream(iconBytes))
                    {
                        //Convert image data to bitmap
                        Bitmap bitmapImage = new Bitmap(memoryStream);

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