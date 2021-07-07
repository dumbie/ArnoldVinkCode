using Android.Graphics;
using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(AVImagesOs))]
namespace ArnoldVinkCode
{
    public class AVImagesOs : AVImages
    {
        //Download and resize image
        public async Task<Stream> DownloadResizeImage(Uri downloadUri, uint maxWidth, uint maxHeight)
        {
            try
            {
                //Download image
                Stream imageStream = await AVDownloader.DownloadStreamAsync(8000, null, null, downloadUri);
                Debug.WriteLine("Android image length: " + imageStream.Length);

                //Decode image
                Bitmap originalImage = await BitmapFactory.DecodeStreamAsync(imageStream);

                //Calculate size
                uint resizeWidth = 0;
                uint resizeHeight = 0;
                uint originalWidth = (uint)originalImage.Width;
                uint originalHeight = (uint)originalImage.Height;
                float originalAspect = (float)originalWidth / (float)originalHeight;
                if (originalWidth > maxWidth)
                {
                    resizeWidth = maxWidth;
                    resizeHeight = (uint)(maxWidth / originalAspect);
                }
                else if (originalHeight > maxHeight)
                {
                    resizeWidth = (uint)(maxHeight / originalAspect);
                    resizeHeight = maxHeight;
                }
                else
                {
                    resizeWidth = originalWidth;
                    resizeHeight = originalHeight;
                }
                //Debug.WriteLine("Resizing image to: " + resizeWidth + "w/" + resizeHeight + "h/" + originalAspect + "a");

                //Resize image
                Bitmap resizeImage = Bitmap.CreateScaledBitmap(originalImage, (int)resizeWidth, (int)resizeHeight, true);
                MemoryStream memoryStream = new MemoryStream();
                resizeImage.Compress(Bitmap.CompressFormat.Png, 100, memoryStream);

                //Dispose resources
                imageStream.Dispose();
                originalImage.Dispose();
                resizeImage.Dispose();

                //Return stream
                memoryStream.Position = 0;
                return memoryStream;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to download and resize image: " + ex.Message);
                return null;
            }
        }
    }
}