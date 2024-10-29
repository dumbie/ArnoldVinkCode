using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace ArnoldVinkCode
{
    public partial class AVImage
    {
        //Get BitmapImage from URI
        public static BitmapImage GetBitmapImageFromUri(Uri sourceUri, int imageWidth, int imageHeight)
        {
            try
            {
                //Prepare application bitmap image
                BitmapImage imageToBitmapImage = BeginBitmapImage(imageWidth, imageHeight);
                MemoryStream imageMemoryStream = null;

                //Set bitmap image stream source
                imageToBitmapImage.UriSource = sourceUri;

                //Return application bitmap image
                return EndBitmapImage(imageToBitmapImage, ref imageMemoryStream);
            }
            catch { }
            return null;
        }
    }
}