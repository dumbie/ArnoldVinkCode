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
        //Get icon from file type
        public static BitmapImage GetBitmapImageFromFileType(string fileType, int imageWidth, int imageHeight)
        {
            IntPtr iconHandle = IntPtr.Zero;
            try
            {
                //Adjust file type
                fileType = "*." + fileType;

                //Get file information
                SHFILEINFO shFileInfo = new SHFILEINFO();
                SHGetFileInfo(fileType, 0, ref shFileInfo, (uint)Marshal.SizeOf(shFileInfo), SHGFI.SHGFI_SYSICONINDEX | SHGFI.SHGFI_USEFILEATTRIBUTES);

                //Get image list
                IImageList iImageList = null;
                SHGetImageList(IMAGELISTTYPE.SHIL_JUMBO, IID_IImageList2, ref iImageList);
                if (iImageList == null)
                {
                    Debug.WriteLine("Failed to get file type image list.");
                    return null;
                }

                //Get icon from list
                iImageList.GetIcon(shFileInfo.iIcon, IMAGELISTDRAWFLAGS.ILD_TRANSPARENT | IMAGELISTDRAWFLAGS.ILD_IMAGE, ref iconHandle);
                if (iconHandle == IntPtr.Zero)
                {
                    Debug.WriteLine("Failed to get icon from image list.");
                    return null;
                }

                //Convert to bitmap source
                BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHIcon(iconHandle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                //Convert to bitmap image
                return BitmapSourceToBitmapImage(bitmapSource, imageWidth, imageHeight);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get icon from file type: " + fileType + " / " + ex.Message);
                return null;
            }
            finally
            {
                SafeCloseImage(ref iconHandle);
            }
        }

        //Guids
        private static readonly Guid IID_IImageList2 = new Guid("192B9D83-50FC-457B-90A0-2B82A8B5DAE1");

        //Interop
        [DllImport("shell32.dll")]
        private extern static int SHGetImageList(IMAGELISTTYPE iImageList, Guid riid, ref IImageList ppv);

        [DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, SHGFI uFlags);

        //Enumerators
        private enum IMAGELISTDRAWFLAGS : int
        {
            ILD_NORMAL = 0x00000000,
            ILD_TRANSPARENT = 0x00000001,
            ILD_IMAGE = 0x00000020
        }

        private enum IMAGELISTTYPE : int
        {
            SHIL_LARGE = 0x0,
            SHIL_SMALL = 0x1,
            SHIL_EXTRALARGE = 0x2,
            SHIL_SYSSMALL = 0x3,
            SHIL_JUMBO = 0x4,
            SHIL_LAST = 0x4
        }

        private enum SHGFI : uint
        {
            SHGFI_ICON = 0x000000100,
            SHGFI_DISPLAYNAME = 0x000000200,
            SHGFI_TYPENAME = 0x000000400,
            SHGFI_ATTRIBUTES = 0x000000800,
            SHGFI_ICONLOCATION = 0x000001000,
            SHGFI_EXETYPE = 0x000002000,
            SHGFI_SYSICONINDEX = 0x000004000,
            SHGFI_LINKOVERLAY = 0x000008000,
            SHGFI_SELECTED = 0x000010000,
            SHGFI_ATTR_SPECIFIED = 0x000020000,
            SHGFI_LARGEICON = 0x000000000,
            SHGFI_SMALLICON = 0x000000001,
            SHGFI_OPENICON = 0x000000002,
            SHGFI_SHELLICONSIZE = 0x000000004,
            SHGFI_PIDL = 0x000000008,
            SHGFI_USEFILEATTRIBUTES = 0x000000010,
            SHGFI_ADDOVERLAYS = 0x000000020,
            SHGFI_OVERLAYINDEX = 0x000000040
        }

        //Structures
        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct IMAGELISTDRAWPARAMS
        {
            public int cbSize;
            public IntPtr himl;
            public int i;
            public IntPtr hdcDst;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public int xBitmap;
            public int yBitmap;
            public int rgbBk;
            public int rgbFg;
            public int fStyle;
            public int dwRop;
            public int fState;
            public int Frame;
            public int crEffect;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct IMAGEINFO
        {
            public IntPtr hbmImage;
            public IntPtr hbmMask;
            public int Unused1;
            public int Unused2;
            public WindowRectangle rcImage;
        }

        //Interfaces
        [ComImport, Guid("46EB5926-582E-4017-9FDF-E8998DAA0950"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IImageList
        {
            int Add(IntPtr hbmImage, IntPtr hbmMask, ref int pi);
            int ReplaceIcon(int i, IntPtr hicon, ref int pi);
            int SetOverlayImage(int iImage, int iOverlay);
            int Replace(int i, IntPtr hbmImage, IntPtr hbmMask);
            int AddMasked(IntPtr hbmImage, int crMask, ref int pi);
            int Draw(ref IMAGELISTDRAWPARAMS pimldp);
            int Remove(int i);
            int GetIcon(int i, IMAGELISTDRAWFLAGS flags, ref IntPtr picon);
            int GetImageInfo(int i, ref IMAGEINFO pImageInfo);
            int Copy(int iDst, IImageList punkSrc, int iSrc, int uFlags);
            int Merge(int i1, IImageList punk2, int i2, int dx, int dy, ref Guid riid, ref IntPtr ppv);
            int Clone(ref Guid riid, ref IntPtr ppv);
            int GetImageRect(int i, ref WindowRectangle prc);
            int GetIconSize(ref int cx, ref int cy);
            int SetIconSize(int cx, int cy);
            int GetImageCount(ref int pi);
            int SetImageCount(int uNewCount);
            int SetBkColor(int clrBk, ref int pclr);
            int GetBkColor(ref int pclr);
            int BeginDrag(int iTrack, int dxHotspot, int dyHotspot);
            int EndDrag();
            int DragEnter(IntPtr hWndLock, int x, int y);
            int DragLeave(IntPtr hWndLock);
            int DragMove(int x, int y);
            int SetDragCursorImage(ref IImageList punk, int iDrag, int dxHotspot, int dyHotspot);
            int DragShowNolock(int fShow);
            int GetDragImage(ref WindowPoint ppt, ref WindowPoint pptHotspot, ref Guid riid, ref IntPtr ppv);
            int GetItemFlags(int i, ref int dwFlags);
            int GetOverlayImage(int iOverlay, ref int piIndex);
        }
    }
}