using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace ArnoldVinkCode
{
    public partial class AVImage
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, LoadLibraryFlags dwFlags);

        [DllImport("kernel32.dll")]
        private static extern bool EnumResourceNames(IntPtr hModule, ResourceTypes lpszType, EnumResNameProcDelegate lpEnumFunc, IntPtr lParam);
        private delegate bool EnumResNameProcDelegate(IntPtr hModule, ResourceTypes lpszType, IntPtr lpEnumFunc, IntPtr lParam);

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

        private static byte[] GetResourceData(IntPtr hModule, IntPtr lpName, ResourceTypes lpType)
        {
            try
            {
                IntPtr findResource = FindResource(hModule, lpName, lpType);
                if (findResource == IntPtr.Zero)
                {
                    return null;
                }

                uint sizeResource = SizeofResource(hModule, findResource);
                if (sizeResource == 0)
                {
                    return null;
                }

                IntPtr loadResource = LoadResource(hModule, findResource);
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

        public static MemoryStream GetIconMemoryStreamFromExeFile(string exeFilePath, int iconIndex, ref MemoryStream imageMemoryStream)
        {
            IntPtr hModule = IntPtr.Zero;
            try
            {
                //Load executable file library
                hModule = LoadLibraryEx(exeFilePath, IntPtr.Zero, LoadLibraryFlags.LOAD_LIBRARY_AS_DATAFILE);
                if (hModule == IntPtr.Zero)
                {
                    Debug.WriteLine("Failed to load icon from exe: " + exeFilePath);
                    return null;
                }

                //Enumerate all icons
                List<BitmapFrame> listBitmapFrame = new List<BitmapFrame>();
                EnumResNameProcDelegate callback = (module, type, name, param) =>
                {
                    try
                    {
                        //Get data from resource
                        byte[] resourceData = GetResourceData(hModule, name, ResourceTypes.GROUP_ICON);

                        //Count available icons
                        int iconDirSize = 6;
                        int iconDirEntrySize = 16;
                        int groupIconDirSize = 6;
                        int groupIconDirEntrySize = 14;
                        int iconCount = BitConverter.ToUInt16(resourceData, 4);
                        int iconOffset = iconDirSize + iconDirEntrySize * iconCount;
                        List<IconDetails> listIconDetails = new List<IconDetails>();

                        //Load and decode icons
                        for (int iconNum = 0; iconNum < iconCount; iconNum++)
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                                {
                                    binaryWriter.Write(resourceData, 0, groupIconDirSize);

                                    //Load the icon
                                    int iconId = BitConverter.ToUInt16(resourceData, groupIconDirSize + groupIconDirEntrySize * iconNum + 12);
                                    byte[] iconData = GetResourceData(hModule, (IntPtr)iconId, ResourceTypes.ICON);

                                    //Write IconDirEntry
                                    binaryWriter.Seek(iconDirSize + iconDirEntrySize * iconNum, SeekOrigin.Begin);
                                    binaryWriter.Write(resourceData, groupIconDirSize + groupIconDirEntrySize * iconNum, 8);
                                    binaryWriter.Write(iconData.Length);
                                    binaryWriter.Write(iconOffset);

                                    //Write icon to stream
                                    binaryWriter.Seek(iconOffset, SeekOrigin.Begin);
                                    binaryWriter.Write(iconData, 0, iconData.Length);
                                    iconOffset += iconData.Length;

                                    //Decode icon bitmap
                                    IconBitmapDecoder iconBitmapDecoder = new IconBitmapDecoder(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

                                    //Add icon to list
                                    IconDetails newIcon = new IconDetails();
                                    newIcon.IconSize = iconData.Length;
                                    newIcon.BitmapFrame = iconBitmapDecoder.Frames.FirstOrDefault();
                                    listIconDetails.Add(newIcon);
                                }
                            }
                        }

                        //Get largest icon bitmap frame
                        listBitmapFrame.Add(listIconDetails.OrderByDescending(x => x.IconSize).FirstOrDefault().BitmapFrame);
                    }
                    catch { }
                    return true;
                };
                EnumResourceNames(hModule, ResourceTypes.GROUP_ICON, callback, IntPtr.Zero);

                PngBitmapEncoder bitmapEncoder = new PngBitmapEncoder();
                bitmapEncoder.Frames.Add(listBitmapFrame[iconIndex]);
                bitmapEncoder.Save(imageMemoryStream);
                imageMemoryStream.Seek(0, SeekOrigin.Begin);
                return imageMemoryStream;
            }
            catch
            {
                //Debug.WriteLine("Failed to load icon from executable: " + ex.Message);
                return null;
            }
            finally
            {
                if (hModule != IntPtr.Zero)
                {
                    FreeLibrary(hModule);
                }
            }
        }
    }
}