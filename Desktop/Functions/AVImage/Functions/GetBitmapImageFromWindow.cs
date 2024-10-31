using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVImage
    {
        //Get window icon from process window
        public static BitmapImage GetBitmapImageFromWindow(IntPtr windowHandle, int imageWidth, int imageHeight)
        {
            IntPtr iconHandle = IntPtr.Zero;
            try
            {
                int GCL_HICON = -14;
                int GCL_HICONSM = -34;
                int ICON_SMALL = 0;
                int ICON_BIG = 1;
                int ICON_SMALL2 = 2;

                //Locks thread when target window is not responding
                iconHandle = SendMessage(windowHandle, WindowMessages.WM_GETICON, ICON_BIG, 0);
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

                //Convert to bitmap source
                BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHIcon(iconHandle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                //Convert to bitmap image
                return BitmapSourceToBitmapImage(bitmapSource, imageWidth, imageHeight);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get icon from window: " + windowHandle + " / " + ex.Message);
                return null;
            }
            finally
            {
                SafeCloseIcon(ref iconHandle);
            }
        }
    }
}