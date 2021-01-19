using System;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace ArnoldVinkCode
{
    public partial class AVImage
    {
        public static MemoryStream GetIconMemoryStreamFromIcoFile(string icoFilePath, ref MemoryStream imageMemoryStream)
        {
            try
            {
                Uri iconUri = new Uri(icoFilePath, UriKind.RelativeOrAbsolute);
                IconBitmapDecoder iconBitmapDecoder = new IconBitmapDecoder(iconUri, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                BitmapFrame bitmapFrameLargest = iconBitmapDecoder.Frames.OrderBy(x => x.Width).ThenBy(x => x.Thumbnail.Format.BitsPerPixel).LastOrDefault();

                PngBitmapEncoder bitmapEncoder = new PngBitmapEncoder();
                bitmapEncoder.Frames.Add(bitmapFrameLargest);
                bitmapEncoder.Save(imageMemoryStream);
                imageMemoryStream.Seek(0, SeekOrigin.Begin);
                return imageMemoryStream;
            }
            catch { }
            return null;
        }
    }
}