using ArnoldVinkCode;
using Foundation;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using UIKit;
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
                Debug.WriteLine("Apple image length: " + imageStream.Length);

                //Decode image
                NSData imageNsData = NSData.FromStream(imageStream);
                UIImage originalImage = UIImage.LoadFromData(imageNsData);

                //Calculate size
                uint resizeWidth = 0;
                uint resizeHeight = 0;
                uint originalWidth = (uint)originalImage.Size.Width;
                uint originalHeight = (uint)originalImage.Size.Height;
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
                UIGraphics.BeginImageContext(new SizeF(resizeWidth, resizeHeight));
                originalImage.Draw(new RectangleF(0, 0, resizeWidth, resizeHeight));
                UIImage resizeImage = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();

                //Convert stream
                Stream resizeStream = resizeImage.AsPNG().AsStream();

                //Dispose resources
                imageStream.Dispose();
                imageNsData.Dispose();
                originalImage.Dispose();
                resizeImage.Dispose();

                //Return stream
                return resizeStream;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to download and resize image: " + ex.Message);
                return null;
            }
        }
    }
}