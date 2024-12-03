using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVImage
    {
        //Functions
        public static BitmapImage GetBitmapImageFromCachedFileThumb(string filePath, int imageWidth, int imageHeight)
        {
            IntPtr bitmapPointer = IntPtr.Zero;
            try
            {
                //Create shellitem instance
                SHCreateItemFromParsingName(filePath, IntPtr.Zero, typeof(IShellItem).GUID, out IShellItem shellItemInstance);
                if (shellItemInstance == null)
                {
                    Debug.WriteLine("Thumbnail failed to create shellitem instance.");
                    return null;
                }

                //Create thumbnail instance
                IThumbnailCache thumbnailCacheInstance = (IThumbnailCache)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_LocalThumbnailCache));
                if (thumbnailCacheInstance == null)
                {
                    Debug.WriteLine("Thumbnail failed to create cache instance.");
                    return null;
                }

                //Get bitmap instance
                thumbnailCacheInstance.GetThumbnail(shellItemInstance, imageWidth, WTS_FLAGS.WTS_EXTRACT, out ISharedBitmap sharedBitmapInstance, out _, out _);
                if (sharedBitmapInstance == null)
                {
                    Debug.WriteLine("Thumbnail failed to create bitmap instance.");
                    return null;
                }

                //Get bitmap pointer
                sharedBitmapInstance.GetSharedBitmap(out bitmapPointer);
                if (bitmapPointer == IntPtr.Zero)
                {
                    Debug.WriteLine("Thumbnail failed bitmap pointer is empty.");
                    return null;
                }

                //Convert to bitmap source
                BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(bitmapPointer, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                //Convert to bitmap image
                return BitmapSourceToBitmapImage(bitmapSource, imageWidth, imageHeight);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Thumbnail failed to load: " + filePath + " / " + ex.Message);
                return null;
            }
            finally
            {
                SafeCloseIcon(ref bitmapPointer);
            }
        }

        //Guids
        private static readonly Guid CLSID_LocalThumbnailCache = new Guid("50EF4544-AC9F-4A8E-B21B-8A26180DB13F");

        //Interop
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern void SHCreateItemFromParsingName
        (
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszPath,
            [In] IntPtr pbc,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [Out, MarshalAs(UnmanagedType.Interface)] out IShellItem ppv
        );

        //Interfaces
        [ComImport, Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellItem { };

        [ComImport, Guid("F676C15D-596A-4CE2-8234-33996F445DB1"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IThumbnailCache
        {
            uint GetThumbnail([In] IShellItem pShellItem, [In] int cxyRequestedThumbSize, [In] WTS_FLAGS flags, [Out, MarshalAs(UnmanagedType.Interface)] out ISharedBitmap ppvThumb, [Out] out WTS_CACHEFLAGS pOutFlags, [Out] out WTS_THUMBNAILID pThumbnailID);
            uint GetThumbnailByID([In, MarshalAs(UnmanagedType.Struct)] WTS_THUMBNAILID thumbnailID, [In] uint cxyRequestedThumbSize, [Out, MarshalAs(UnmanagedType.Interface)] out ISharedBitmap ppvThumb, [Out] out WTS_CACHEFLAGS pOutFlags);
        }

        [ComImport, Guid("091162A4-BC96-411F-AAE8-C5122CD03363"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ISharedBitmap
        {
            uint GetSharedBitmap([Out] out IntPtr phbm);
            uint GetSize([Out, MarshalAs(UnmanagedType.Struct)] out WindowSize pSize);
            uint GetFormat([Out] out WTS_ALPHATYPE pat);
            uint InitializeBitmap([In] IntPtr hbm, [In] WTS_ALPHATYPE wtsAT);
            uint Detach([Out] out IntPtr phbm);
        }

        //Enumerators
        private enum WTS_FLAGS : uint
        {
            WTS_NONE = 0x0,
            WTS_EXTRACT = 0x0,
            WTS_INCACHEONLY = 0x1,
            WTS_FASTEXTRACT = 0x2,
            WTS_FORCEEXTRACTION = 0x4,
            WTS_SLOWRECLAIM = 0x8,
            WTS_EXTRACTDONOTCACHE = 0x20,
            WTS_SCALETOREQUESTEDSIZE = 0x40,
            WTS_SKIPFASTEXTRACT = 0x80,
            WTS_EXTRACTINPROC = 0x100,
            WTS_CROPTOSQUARE = 0x200,
            WTS_INSTANCESURROGATE = 0x400,
            WTS_REQUIRESURROGATE = 0x800,
            WTS_APPSTYLE = 0x2000,
            WTS_WIDETHUMBNAILS = 0x4000,
            WTS_IDEALCACHESIZEONLY = 0x8000,
            WTS_SCALEUP = 0x10000
        }

        private enum WTS_ALPHATYPE : uint
        {
            WTSAT_UNKNOWN = 0,
            WTSAT_RGB = 1,
            WTSAT_ARGB = 2
        }

        private enum WTS_CACHEFLAGS : uint
        {
            WTS_DEFAULT = 0x00000000,
            WTS_LOWQUALITY = 0x00000001,
            WTS_CACHED = 0x00000002
        }

        //Structures
        [StructLayout(LayoutKind.Sequential)]
        private struct WTS_THUMBNAILID
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] rgbKey;
        }
    }
}