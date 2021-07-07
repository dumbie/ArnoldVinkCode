using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
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
                Debug.WriteLine("Windows image length: " + imageStream.Length);

                //Decode image
                BitmapDecoder bitmapDecoder = await BitmapDecoder.CreateAsync(imageStream.AsRandomAccessStream());

                //Calculate size
                uint resizeWidth = 0;
                uint resizeHeight = 0;
                uint originalWidth = bitmapDecoder.PixelWidth;
                uint originalHeight = bitmapDecoder.PixelHeight;
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
                InMemoryRandomAccessStream resizeStream = new InMemoryRandomAccessStream();
                BitmapEncoder bitmapEncoder = await BitmapEncoder.CreateForTranscodingAsync(resizeStream, bitmapDecoder);
                bitmapEncoder.BitmapTransform.ScaledWidth = resizeWidth;
                bitmapEncoder.BitmapTransform.ScaledHeight = resizeHeight;
                await bitmapEncoder.FlushAsync();

                //Dispose resources
                imageStream.Dispose();

                //Convert stream
                return resizeStream.AsStream();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to download and resize image: " + ex.Message);
                return null;
            }
        }
    }
}