using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace ArnoldVinkCode
{
    public partial class AVImage
    {
        //Begin bitmap image
        private static BitmapImage BeginBitmapImage(int pixelWidth)
        {
            try
            {
                BitmapImage imageToBitmapImage = new BitmapImage();
                imageToBitmapImage.BeginInit();
                imageToBitmapImage.CreateOptions = BitmapCreateOptions.None;
                imageToBitmapImage.CacheOption = BitmapCacheOption.OnLoad;

                if (pixelWidth > 0)
                {
                    imageToBitmapImage.DecodePixelWidth = pixelWidth;
                }

                return imageToBitmapImage;
            }
            catch { }
            return null;
        }

        //End bitmap image
        private static BitmapImage EndBitmapImage(BitmapImage imageToBitmapImage, ref MemoryStream imageMemoryStream)
        {
            try
            {
                imageToBitmapImage.EndInit();
                if (imageToBitmapImage.CanFreeze)
                {
                    imageToBitmapImage.Freeze();
                }

                //Clear memory stream
                if (imageMemoryStream != null)
                {
                    imageMemoryStream.Close();
                    imageMemoryStream.Dispose();
                }

                return imageToBitmapImage;
            }
            catch { }
            return null;
        }

        //Convert uri to a BitmapImage
        public static BitmapImage UriToBitmapImage(Uri sourceUri, int pixelWidth)
        {
            try
            {
                //Prepare application bitmap image
                BitmapImage imageToBitmapImage = BeginBitmapImage(pixelWidth);
                MemoryStream imageMemoryStream = null;

                //Set the stream source
                imageToBitmapImage.UriSource = sourceUri;

                //Return application bitmap image
                return EndBitmapImage(imageToBitmapImage, ref imageMemoryStream);
            }
            catch { }
            return null;
        }

        //Convert bytes to a BitmapImage
        public static BitmapImage BytesToBitmapImage(byte[] byteArray, int pixelWidth)
        {
            try
            {
                //Prepare application bitmap image
                BitmapImage imageToBitmapImage = BeginBitmapImage(pixelWidth);
                MemoryStream imageMemoryStream = new MemoryStream(byteArray);

                //Set the stream source
                imageToBitmapImage.StreamSource = imageMemoryStream;

                //Return application bitmap image
                return EndBitmapImage(imageToBitmapImage, ref imageMemoryStream);
            }
            catch { }
            return null;
        }

        //Convert file to a BitmapImage
        public static BitmapImage FileToBitmapImage(string[] sourceImages, string[] sourceFolders, string sourceBackup, IntPtr windowHandle, int pixelWidth, int iconIndex)
        {
            try
            {
                //Prepare application bitmap image
                BitmapImage imageToBitmapImage = BeginBitmapImage(pixelWidth);
                MemoryStream imageMemoryStream = new MemoryStream();

                //Load application bitmap image
                foreach (string loadFile in sourceImages)
                {
                    try
                    {
                        //Validate the load path
                        if (string.IsNullOrWhiteSpace(loadFile)) { continue; }

                        //Adjust the load path
                        string loadFileLower = loadFile.ToLower();
                        loadFileLower = AVFunctions.StringRemoveStart(loadFileLower, " ");
                        loadFileLower = AVFunctions.StringRemoveEnd(loadFileLower, " ");
                        string loadFileSafe = string.Join(string.Empty, loadFileLower.Split(Path.GetInvalidFileNameChars()));
                        //Debug.WriteLine("Loading image: " + loadFileLower + "/" + loadFileSafe);

                        if (loadFileLower.StartsWith("pack://application:,,,") || loadFileLower.StartsWith("pack://siteoforigin:,,,"))
                        {
                            imageToBitmapImage.UriSource = new Uri(loadFileLower, UriKind.RelativeOrAbsolute);
                        }
                        else if (File.Exists(loadFileLower) && !loadFileLower.EndsWith(".exe") && !loadFileLower.EndsWith(".dll") && !loadFileLower.EndsWith(".bin") && !loadFileLower.EndsWith(".tmp"))
                        {
                            imageToBitmapImage.UriSource = new Uri(loadFileLower, UriKind.RelativeOrAbsolute);
                        }
                        else if (File.Exists(loadFileLower) && (loadFileLower.EndsWith(".exe") || loadFileLower.EndsWith(".dll") || loadFileLower.EndsWith(".bin") || loadFileLower.EndsWith(".tmp")))
                        {
                            Bitmap executableImage = ExtractImage.GetBitmapFromExecutable(loadFileLower, iconIndex);
                            if (executableImage != null)
                            {
                                executableImage.Save(imageMemoryStream, ImageFormat.Png);
                                imageMemoryStream.Seek(0, SeekOrigin.Begin);
                                imageToBitmapImage.StreamSource = imageMemoryStream;
                                executableImage.Dispose();
                            }
                        }
                        else if (sourceFolders != null)
                        {
                            foreach (string loadFolder in sourceFolders)
                            {
                                try
                                {
                                    string folderFilePng = loadFolder + "/" + loadFileSafe + ".png";
                                    if (File.Exists(folderFilePng))
                                    {
                                        imageToBitmapImage.UriSource = new Uri(folderFilePng, UriKind.RelativeOrAbsolute);
                                        break;
                                    }
                                    string folderFileJpg = loadFolder + "/" + loadFileSafe + ".jpg";
                                    if (File.Exists(folderFileJpg))
                                    {
                                        imageToBitmapImage.UriSource = new Uri(folderFileJpg, UriKind.RelativeOrAbsolute);
                                        break;
                                    }
                                }
                                catch { }
                            }
                        }

                        //Return application bitmap image
                        if (imageToBitmapImage.UriSource != null || imageToBitmapImage.StreamSource != null)
                        {
                            return EndBitmapImage(imageToBitmapImage, ref imageMemoryStream);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Failed loading image: " + loadFile + "/" + ex.Message);
                    }
                }

                //Image source not found, loading backup or window icon
                if (windowHandle == IntPtr.Zero)
                {
                    imageToBitmapImage.UriSource = new Uri(sourceBackup, UriKind.RelativeOrAbsolute);
                }
                else
                {
                    BitmapSource windowImage = ExtractImage.GetBitmapSourceFromWinow(windowHandle);
                    if (windowImage != null)
                    {
                        PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
                        pngEncoder.Frames.Add(BitmapFrame.Create(windowImage));
                        pngEncoder.Save(imageMemoryStream);
                        imageMemoryStream.Seek(0, SeekOrigin.Begin);
                        imageToBitmapImage.StreamSource = imageMemoryStream;
                    }
                    else
                    {
                        imageToBitmapImage.UriSource = new Uri(sourceBackup, UriKind.RelativeOrAbsolute);
                    }
                }

                //Return application bitmap image
                return EndBitmapImage(imageToBitmapImage, ref imageMemoryStream);
            }
            catch { }
            return null;
        }
    }
}