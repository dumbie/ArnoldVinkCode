using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVShell;

namespace ArnoldVinkCode
{
    public partial class AVImage
    {
        //Functions
        public static BitmapImage GetBitmapImageFromCachedFile(string filePath, int imageWidth, int imageHeight, bool extractIcon)
        {
            IntPtr bitmapPointer = IntPtr.Zero;
            try
            {
                //Create shellitem instance
                SHCreateItemFromParsingName(filePath, IntPtr.Zero, typeof(IShellItemImageFactory).GUID, out object shellObject);
                if (shellObject == null)
                {
                    Debug.WriteLine("Thumbnail failed to create shellitem image instance.");
                    return null;
                }

                //Cast shellitem instance
                IShellItemImageFactory shellItem = (IShellItemImageFactory)shellObject;

                //Set bitmap target size
                WindowSize bitmapSize = new WindowSize();
                bitmapSize.cx = imageWidth;
                bitmapSize.cy = imageHeight;

                //Set bitmap flags
                SIIGBF extractFlags = SIIGBF.SIIGBF_BIGGERSIZEOK | (extractIcon ? SIIGBF.SIIGBF_ICONONLY : SIIGBF.SIIGBF_THUMBNAILONLY);

                //Get bitmap pointer
                try
                {
                    shellItem.GetImage(bitmapSize, extractFlags, out bitmapPointer);
                }
                catch (COMException ex)
                {
                    //Fix WTS_E_FAILEDEXTRACTION error when loading files from OneDrive
                    //Fix retry to load thumbnail when result is WTS_E_EXTRACTIONPENDING
                    Debug.WriteLine("Thumbnail failed to extract: " + (WTS_EXCEPTIONS)ex.HResult + " / " + filePath);
                }

                //Check bitmap thumbnail
                if (!extractIcon && bitmapPointer == IntPtr.Zero)
                {
                    try
                    {
                        extractFlags = SIIGBF.SIIGBF_BIGGERSIZEOK | SIIGBF.SIIGBF_ICONONLY;
                        shellItem.GetImage(bitmapSize, extractFlags, out bitmapPointer);
                    }
                    catch { }
                }

                //Check bitmap pointer
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
    }
}