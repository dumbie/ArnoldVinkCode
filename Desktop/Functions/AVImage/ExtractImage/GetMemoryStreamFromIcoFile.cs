using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace ArnoldVinkCode
{
    public partial class ExtractImage
    {
        public static MemoryStream GetMemoryStreamFromIcoFileLarge(string icoFilePath, ref MemoryStream imageMemoryStream)
        {
            try
            {
                using (Stream iconStream = new FileStream(icoFilePath, FileMode.Open, FileAccess.Read))
                {
                    IconBitmapDecoder iconBitmapDecoder = new IconBitmapDecoder(iconStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    BitmapFrame bitmapFrameLargest = iconBitmapDecoder.Frames.OrderBy(x => x.Width).LastOrDefault();

                    PngBitmapEncoder bitmapEncoder = new PngBitmapEncoder();
                    bitmapEncoder.Frames.Add(bitmapFrameLargest);
                    bitmapEncoder.Save(imageMemoryStream);
                    imageMemoryStream.Seek(0, SeekOrigin.Begin);
                    return imageMemoryStream;
                }
            }
            catch { }
            return null;
        }
    }
}