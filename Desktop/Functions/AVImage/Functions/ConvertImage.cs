using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace ArnoldVinkCode
{
    public partial class AVImage
    {
        //Convert Bitmap to MemoryStream
        public static MemoryStream BitmapToMemoryStream(ref Bitmap bitmap)
        {
            try
            {
                //Create memory stream
                MemoryStream imageMemoryStream = new MemoryStream();

                //Save bitmap frame
                bitmap.Save(imageMemoryStream, ImageFormat.Png);

                //Dispose source bitmap
                bitmap.Dispose();

                //Return memory stream
                return imageMemoryStream;
            }
            catch { }
            return null;
        }

        //Convert BitmapSource to MemoryStream
        public static MemoryStream BitmapSourceToMemoryStream(BitmapSource sourceBitmap)
        {
            try
            {
                //Create memory stream
                MemoryStream imageMemoryStream = new MemoryStream();

                //Create and save bitmap frame
                BitmapFrame bitmapFrame = BitmapFrame.Create(sourceBitmap);
                PngBitmapEncoder bitmapEncoder = new PngBitmapEncoder();
                bitmapEncoder.Frames.Add(bitmapFrame);
                bitmapEncoder.Save(imageMemoryStream);

                //Return memory stream
                return imageMemoryStream;
            }
            catch { }
            return null;
        }

        //Convert BitmapSource to BitmapImage
        public static BitmapImage BitmapSourceToBitmapImage(BitmapSource sourceBitmap, int imageWidth, int imageHeight)
        {
            try
            {
                //Prepare application bitmap image
                BitmapImage imageToBitmapImage = BeginBitmapImage(imageWidth, imageHeight);
                MemoryStream imageMemoryStream = new MemoryStream();

                //Create and save bitmap frame
                BitmapFrame bitmapFrame = BitmapFrame.Create(sourceBitmap);
                PngBitmapEncoder bitmapEncoder = new PngBitmapEncoder();
                bitmapEncoder.Frames.Add(bitmapFrame);
                bitmapEncoder.Save(imageMemoryStream);

                //Set bitmap image stream source
                imageToBitmapImage.StreamSource = imageMemoryStream;

                //Return application bitmap image
                return EndBitmapImage(imageToBitmapImage, ref imageMemoryStream);
            }
            catch { }
            return null;
        }

        //Convert BitmapFrame to BitmapImage
        public static BitmapImage BitmapFrameToBitmapImage(BitmapFrame sourceBitmap, int imageWidth, int imageHeight)
        {
            try
            {
                //Prepare application bitmap image
                BitmapImage imageToBitmapImage = BeginBitmapImage(imageWidth, imageHeight);
                MemoryStream imageMemoryStream = new MemoryStream();

                //Create and save bitmap frame
                PngBitmapEncoder bitmapEncoder = new PngBitmapEncoder();
                bitmapEncoder.Frames.Add(sourceBitmap);
                bitmapEncoder.Save(imageMemoryStream);

                //Set bitmap image stream source
                imageToBitmapImage.StreamSource = imageMemoryStream;

                //Return application bitmap image
                return EndBitmapImage(imageToBitmapImage, ref imageMemoryStream);
            }
            catch { }
            return null;
        }

        //Convert Bitmap to BitmapImage
        public static BitmapImage BitmapToBitmapImage(ref Bitmap sourceBitmap, int imageWidth, int imageHeight)
        {
            try
            {
                //Prepare application bitmap image
                BitmapImage imageToBitmapImage = BeginBitmapImage(imageWidth, imageHeight);

                //Save bitmap to memorystream
                MemoryStream imageMemoryStream = BitmapToMemoryStream(ref sourceBitmap);

                //Set bitmap image stream source
                imageToBitmapImage.StreamSource = imageMemoryStream;

                //Return application bitmap image
                return EndBitmapImage(imageToBitmapImage, ref imageMemoryStream);
            }
            catch { }
            return null;
        }

        //Convert Bytes to BitmapImage
        public static BitmapImage BytesToBitmapImage(byte[] byteArray, int imageWidth, int imageHeight)
        {
            try
            {
                //Prepare application bitmap image
                BitmapImage imageToBitmapImage = BeginBitmapImage(imageWidth, imageHeight);
                MemoryStream imageMemoryStream = new MemoryStream(byteArray);

                //Set bitmap image stream source
                imageToBitmapImage.StreamSource = imageMemoryStream;

                //Return application bitmap image
                return EndBitmapImage(imageToBitmapImage, ref imageMemoryStream);
            }
            catch { }
            return null;
        }

        //Save BitmapImage to PNG file
        public static bool BitmapImageToFile(BitmapImage sourceImage, string targetPath, bool overwrite)
        {
            try
            {
                if (overwrite || !File.Exists(targetPath))
                {
                    BitmapFrame bitmapFrame = BitmapFrame.Create(sourceImage);
                    PngBitmapEncoder bitmapEncoder = new PngBitmapEncoder();
                    bitmapEncoder.Frames.Add(bitmapFrame);

                    using (FileStream fileStream = new FileStream(targetPath, FileMode.Create))
                    {
                        bitmapEncoder.Save(fileStream);
                    }

                    return true;
                }
            }
            catch { }
            return false;
        }
    }
}