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
                //Check local cache
                Stream imageStream = null;
                string cacheFile = System.IO.Path.Combine("Cache", AVFunctions.StringToHash(downloadUri.ToString()));
                if (AVFiles.File_Exists(cacheFile, true))
                {
                    //Load cached image
                    imageStream = AVFiles.File_LoadStream(cacheFile, true);

                    Debug.WriteLine("Android cache image length: " + imageStream.Length);
                }
                else
                {
                    //Download image
                    imageStream = await AVDownloader.DownloadStreamAsync(8000, null, null, downloadUri);

                    //Save cache image
                    AVFiles.File_SaveStream(cacheFile, imageStream, true, true);

                    Debug.WriteLine("Android download image length: " + imageStream.Length);
                }

                //Decode image
                if (imageStream.CanSeek) { imageStream.Position = 0; }
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
                if (memoryStream.CanSeek) { memoryStream.Position = 0; }

                //Dispose resources
                imageStream.Dispose();
                originalImage.Dispose();
                resizeImage.Dispose();

                //Return stream
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