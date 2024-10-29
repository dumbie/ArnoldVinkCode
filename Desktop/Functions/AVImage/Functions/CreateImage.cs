using System.IO;
using System.Windows.Media.Imaging;

namespace ArnoldVinkCode
{
    public partial class AVImage
    {
        //Begin bitmap image
        private static BitmapImage BeginBitmapImage(int imageWidth, int imageHeight)
        {
            try
            {
                //Begin bitmap initialization
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();

                //Set bitmap options
                bitmapImage.CreateOptions = BitmapCreateOptions.None;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;

                //Set bitmap size
                if (imageWidth > 0)
                {
                    bitmapImage.DecodePixelWidth = imageWidth;
                }

                if (imageHeight > 0)
                {
                    bitmapImage.DecodePixelHeight = imageHeight;
                }

                //Return bitmap image
                return bitmapImage;
            }
            catch { }
            return null;
        }

        //End bitmap image
        private static BitmapImage EndBitmapImage(BitmapImage bitmapImage, ref MemoryStream imageMemoryStream)
        {
            try
            {
                //End bitmap initialization
                bitmapImage.EndInit();

                //Freeze bitmap image
                if (bitmapImage.CanFreeze)
                {
                    bitmapImage.Freeze();
                }

                //Clear memory stream
                if (imageMemoryStream != null)
                {
                    imageMemoryStream.Close();
                    imageMemoryStream.Dispose();
                }

                //Return bitmap image
                return bitmapImage;
            }
            catch { }
            return null;
        }
    }
}