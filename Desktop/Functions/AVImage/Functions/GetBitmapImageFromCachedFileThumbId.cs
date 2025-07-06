using System;
using System.Diagnostics;
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
        public static BitmapImage GetBitmapImageFromCachedFileThumbId(string filePath, int imageWidth, int imageHeight)
        {
            IntPtr bitmapPointer = IntPtr.Zero;
            try
            {
                //Create shellitem instance
                SHCreateItemFromParsingName(filePath, IntPtr.Zero, typeof(IShellItem2).GUID, out object shellObject);
                if (shellObject == null)
                {
                    Debug.WriteLine("Thumbnail failed to create shellitem instance.");
                    return null;
                }

                //Cast shellitem instance
                IShellItem2 shellItem = (IShellItem2)shellObject;

                //Get property variant
                PropertyVariant propVariant = shellItem.GetProperty(PKEY_ThumbnailCacheId);

                //Set rgb key from property variant
                WTS_THUMBNAILID thumbnailId = new WTS_THUMBNAILID();
                thumbnailId.RgbKey = propVariant.ulVal;

                //Create thumbnail instance
                IThumbnailCache thumbnailCacheInstance = (IThumbnailCache)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_LocalThumbnailCache));
                if (thumbnailCacheInstance == null)
                {
                    Debug.WriteLine("Thumbnail failed to create cache instance.");
                    return null;
                }

                //Get bitmap instance
                thumbnailCacheInstance.GetThumbnailByID(thumbnailId, imageWidth, out ISharedBitmap sharedBitmapInstance, out _);
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
    }
}