using System;
using System.Linq;
using System.Windows.Media.Imaging;

namespace ArnoldVinkCode
{
    public partial class AVImage
    {
        public static BitmapImage GetBitmapImageFromIcoFile(string icoFilePath, int imageWidth, int imageHeight)
        {
            try
            {
                Uri iconUri = new Uri(icoFilePath, UriKind.RelativeOrAbsolute);
                BitmapDecoder iconBitmapDecoder = BitmapDecoder.Create(iconUri, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                BitmapFrame bitmapFrameLargest = iconBitmapDecoder.Frames.OrderBy(x => x.Width).ThenBy(x => x.Thumbnail.Format.BitsPerPixel).LastOrDefault();

                //Convert BitmapFrame to BitmapImage
                return BitmapFrameToBitmapImage(bitmapFrameLargest, imageWidth, imageHeight);
            }
            catch { }
            return null;
        }
    }
}