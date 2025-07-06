using System;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVShell
    {
        //Guids
        public static readonly Guid CLSID_LocalThumbnailCache = new Guid("50EF4544-AC9F-4A8E-B21B-8A26180DB13F");

        //Interfaces
        [ComImport, Guid("BCC18B79-BA16-442F-80C4-8A59C30C463B"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IShellItemImageFactory
        {
            WTS_EXCEPTIONS GetImage(WindowSize size, SIIGBF flags, out IntPtr phbm);
        }

        [ComImport, Guid("F676C15D-596A-4CE2-8234-33996F445DB1"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IThumbnailCache
        {
            uint GetThumbnail([In] IShellItem2 pShellItem, [In] int cxyRequestedThumbSize, [In] WTS_FLAGS flags, [Out, MarshalAs(UnmanagedType.Interface)] out ISharedBitmap ppvThumb, [Out] out WTS_CACHEFLAGS pOutFlags, [Out] out WTS_THUMBNAILID pThumbnailID);
            uint GetThumbnailByID([In, MarshalAs(UnmanagedType.Struct)] WTS_THUMBNAILID thumbnailID, [In] int cxyRequestedThumbSize, [Out, MarshalAs(UnmanagedType.Interface)] out ISharedBitmap ppvThumb, [Out] out WTS_CACHEFLAGS pOutFlags);
        }

        [ComImport, Guid("091162A4-BC96-411F-AAE8-C5122CD03363"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ISharedBitmap
        {
            uint GetSharedBitmap([Out] out IntPtr phbm);
            uint GetSize([Out, MarshalAs(UnmanagedType.Struct)] out WindowSize pSize);
            uint GetFormat([Out] out WTS_ALPHATYPE pat);
            uint InitializeBitmap([In] IntPtr hbm, [In] WTS_ALPHATYPE wtsAT);
            uint Detach([Out] out IntPtr phbm);
        }

        //Enumerators
        public enum SIIGBF : uint
        {
            SIIGBF_RESIZETOFIT = 0x00000000,
            SIIGBF_BIGGERSIZEOK = 0x00000001,
            SIIGBF_MEMORYONLY = 0x00000002,
            SIIGBF_ICONONLY = 0x00000004,
            SIIGBF_THUMBNAILONLY = 0x00000008,
            SIIGBF_INCACHEONLY = 0x00000010,
            SIIGBF_CROPTOSQUARE = 0x00000020,
            SIIGBF_WIDETHUMBNAILS = 0x00000040,
            SIIGBF_ICONBACKGROUND = 0x00000080,
            SIIGBF_SCALEUP = 0x00000100
        }

        public enum WTS_EXCEPTIONS : uint
        {
            WTS_E_SUCCESSFULEXTRACTION = 0x00000000,
            WTS_E_FAILEDEXTRACTION = 0x8004B200,
            WTS_E_EXTRACTIONTIMEDOUT = 0x8004B201,
            WTS_E_SURROGATEUNAVAILABLE = 0x8004B202,
            WTS_E_FASTEXTRACTIONNOTSUPPORTED = 0x8004B203,
            WTS_E_DATAFILEUNAVAILABLE = 0x8004B204,
            WTS_E_EXTRACTIONPENDING = 0x8004B205,
            WTS_E_EXTRACTIONBLOCKED = 0x8004B205
        }

        public enum WTS_FLAGS : uint
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

        public enum WTS_ALPHATYPE : uint
        {
            WTSAT_UNKNOWN = 0,
            WTSAT_RGB = 1,
            WTSAT_ARGB = 2
        }

        public enum WTS_CACHEFLAGS : uint
        {
            WTS_DEFAULT = 0x00000000,
            WTS_LOWQUALITY = 0x00000001,
            WTS_CACHED = 0x00000002
        }

        //Structures
        public struct WTS_THUMBNAILID
        {
            public ulong RgbKey;
            public ulong Reserved;
        }
    }
}