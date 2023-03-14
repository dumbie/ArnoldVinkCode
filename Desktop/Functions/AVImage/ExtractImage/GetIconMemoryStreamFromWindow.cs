using System;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVImage
    {
        //Get the window icon from process
        public static MemoryStream GetIconMemoryStreamFromWindow(IntPtr windowHandle, ref MemoryStream imageMemoryStream)
        {
            try
            {
                int GCL_HICON = -14;
                int GCL_HICONSM = -34;
                int ICON_SMALL = 0;
                int ICON_BIG = 1;
                int ICON_SMALL2 = 2;

                //Locks thread when target window is not responding
                IntPtr iconHandle = SendMessage(windowHandle, WindowMessages.WM_GETICON, ICON_BIG, 0);
                if (iconHandle == IntPtr.Zero)
                {
                    iconHandle = SendMessage(windowHandle, WindowMessages.WM_GETICON, ICON_SMALL, 0);
                }
                if (iconHandle == IntPtr.Zero)
                {
                    iconHandle = SendMessage(windowHandle, WindowMessages.WM_GETICON, ICON_SMALL2, 0);
                }
                if (iconHandle == IntPtr.Zero)
                {
                    iconHandle = GetClassLongAuto(windowHandle, GCL_HICON);
                }
                if (iconHandle == IntPtr.Zero)
                {
                    iconHandle = GetClassLongAuto(windowHandle, GCL_HICONSM);
                }
                if (iconHandle == IntPtr.Zero)
                {
                    return null;
                }

                BitmapFrame windowImage = BitmapFrame.Create(Imaging.CreateBitmapSourceFromHIcon(iconHandle, new Int32Rect(), BitmapSizeOptions.FromEmptyOptions()));
                PngBitmapEncoder bitmapEncoder = new PngBitmapEncoder();
                bitmapEncoder.Frames.Add(windowImage);
                bitmapEncoder.Save(imageMemoryStream);
                imageMemoryStream.Seek(0, SeekOrigin.Begin);
                return imageMemoryStream;
            }
            catch { }
            return null;
        }
    }
}