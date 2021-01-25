using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVImage
    {
        //Variables
        private static List<IntPtr> iconGroups = new List<IntPtr>();

        //DllImport
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

        //Methods
        private static bool ReadStuctureFromStream<T>(Stream readStream, out T readStructure)
        {
            IntPtr intPtr = IntPtr.Zero;
            try
            {
                int byteSize = Marshal.SizeOf(typeof(T));
                byte[] byteBuffer = new byte[byteSize];
                readStream.Read(byteBuffer, 0, byteSize);
                intPtr = Marshal.AllocHGlobal(byteSize);
                Marshal.Copy(byteBuffer, 0, intPtr, byteSize);
                readStructure = Marshal.PtrToStructure<T>(intPtr);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to ReadStuctureFromStream: " + ex.Message);
                readStructure = default(T);
                return false;
            }
            finally
            {
                if (intPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(intPtr);
                }
            }
        }

        private static bool WriteStructureToStream<T>(Stream writeStream, T writeStructure)
        {
            IntPtr intPtr = IntPtr.Zero;
            try
            {
                int byteSize = Marshal.SizeOf(typeof(T));
                byte[] byteBuffer = new byte[byteSize];
                intPtr = Marshal.AllocHGlobal(byteSize);
                Marshal.StructureToPtr(writeStructure, intPtr, true);
                Marshal.Copy(intPtr, byteBuffer, 0, byteSize);
                writeStream.Write(byteBuffer, 0, byteSize);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to WriteStructureToStream: " + ex.Message);
                return false;
            }
            finally
            {
                if (intPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(intPtr);
                }
            }
        }

        private static byte[] GetResourceDataBytes(IntPtr hModule, IntPtr lpName, ResourceTypes lpType)
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

        private static T GetResourceDataStruct<T>(IntPtr hModule, IntPtr lpName, ResourceTypes lpType)
        {
            try
            {
                IntPtr findResource = FindResource(hModule, lpName, lpType);
                if (findResource == IntPtr.Zero)
                {
                    return default(T);
                }

                IntPtr loadResource = LoadResource(hModule, findResource);
                if (loadResource == IntPtr.Zero)
                {
                    return default(T);
                }

                IntPtr lockResource = LockResource(loadResource);
                if (lockResource == IntPtr.Zero)
                {
                    return default(T);
                }

                return Marshal.PtrToStructure<T>(lockResource);
            }
            catch { }
            return default(T);
        }

        private static IntPtr GetResourceDataIntPtr(IntPtr hModule, IntPtr lpName, ResourceTypes lpType)
        {
            try
            {
                IntPtr findResource = FindResource(hModule, lpName, lpType);
                if (findResource == IntPtr.Zero)
                {
                    return IntPtr.Zero;
                }

                uint sizeResource = SizeofResource(hModule, findResource);
                if (sizeResource == 0)
                {
                    return IntPtr.Zero;
                }

                IntPtr loadResource = LoadResource(hModule, findResource);
                if (loadResource == IntPtr.Zero)
                {
                    return IntPtr.Zero;
                }

                IntPtr lockResource = LockResource(loadResource);
                if (lockResource == IntPtr.Zero)
                {
                    return IntPtr.Zero;
                }
                else
                {
                    return lockResource;
                }
            }
            catch { }
            return IntPtr.Zero;
        }

        private static bool EnumResourceNamesProc(IntPtr hModule, ResourceTypes lpszType, IntPtr lpEnumFunc, IntPtr lParam)
        {
            try
            {
                iconGroups.Add(lpEnumFunc);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static MemoryStream GetIconMemoryStreamFromExeFile(string exeFilePath, int iconIndex, ref MemoryStream imageMemoryStream)
        {
            IntPtr hLibrary = IntPtr.Zero;
            try
            {
                //Load executable file library
                hLibrary = LoadLibraryEx(exeFilePath, IntPtr.Zero, LoadLibraryFlags.LOAD_LIBRARY_AS_DATAFILE);
                if (hLibrary == IntPtr.Zero)
                {
                    Debug.WriteLine("Failed to load icon from exe: " + exeFilePath);
                    return null;
                }

                //Enumerate all icon groups
                EnumResourceNames(hLibrary, ResourceTypes.GROUP_ICON, EnumResourceNamesProc, IntPtr.Zero);
                foreach (IntPtr iconGroup in iconGroups)
                {
                    try
                    {
                        //Get all icons from group
                        MEMICONDIR memICONDIR = GetResourceDataStruct<MEMICONDIR>(hLibrary, iconGroup, ResourceTypes.GROUP_ICON);
                        foreach (MEMICONDIRENTRY entry in memICONDIR.arEntries)
                        {
                            try
                            {
                                //Get icon bitmap data
                                byte[] iconBytes = GetResourceDataBytes(hLibrary, (IntPtr)entry.wIdentifier, ResourceTypes.ICON);

                                //Encode icon bitmap frame
                                byte signatureByte = iconBytes[0];
                                if (signatureByte == 0x28)
                                {
                                    Debug.WriteLine("BMP image: " + iconBytes.Length);
                                    using (MemoryStream memoryStream = new MemoryStream(iconBytes))
                                    {
                                        //Read bitmap info header
                                        ReadStuctureFromStream(memoryStream, out BITMAPINFOHEADER infoHeader);

                                        Debug.WriteLine("BMP OK");
                                        return imageMemoryStream;
                                    }
                                }
                                else if (signatureByte == 0x89)
                                {
                                    Debug.WriteLine("PNG image: " + iconBytes.Length);
                                    using (MemoryStream memoryStream = new MemoryStream(iconBytes))
                                    {
                                        //Seek to image data array
                                        memoryStream.Seek(0, SeekOrigin.Begin);

                                        //Convert image data to bitmap
                                        Bitmap bitmapImage = new Bitmap(memoryStream);
                                        bitmapImage.Save(imageMemoryStream, ImageFormat.Png);
                                        imageMemoryStream.Seek(0, SeekOrigin.Begin);

                                        Debug.WriteLine("w/" + bitmapImage.Width + " h/" + bitmapImage.Height);
                                        Debug.WriteLine("PNG OK");
                                        return imageMemoryStream;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("No icon for you!" + ex.Message);
                            }
                        }
                    }
                    catch { }
                }

                Debug.WriteLine("No icon exe icon found to load.");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load exe icon: " + ex.Message);
                return null;
            }
            finally
            {
                if (hLibrary != IntPtr.Zero)
                {
                    FreeLibrary(hLibrary);
                }
            }
        }
    }
}