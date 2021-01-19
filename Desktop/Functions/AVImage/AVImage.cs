using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

                //Freeze bitmap image
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
        public static BitmapImage FileToBitmapImage(string[] sourceImages, ImageSourceFolders[] sourceFolders, string sourceBackup, IntPtr windowHandle, int pixelWidth, int iconIndex)
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
                        if (!loadFileLower.Contains("/") && !loadFileLower.Contains("\\"))
                        {
                            loadFileLower = string.Join(string.Empty, loadFileLower.Split(Path.GetInvalidFileNameChars()));
                        }
                        //Debug.WriteLine("Loading image: " + loadFileLower);

                        if (loadFileLower.StartsWith("pack://"))
                        {
                            imageToBitmapImage.UriSource = new Uri(loadFileLower, UriKind.RelativeOrAbsolute);
                        }
                        else if (File.Exists(loadFileLower) && loadFileLower.EndsWith(".ico"))
                        {
                            imageToBitmapImage.StreamSource = GetIconMemoryStreamFromIcoFile(loadFileLower, ref imageMemoryStream);
                        }
                        else if (File.Exists(loadFileLower) && !loadFileLower.EndsWith(".exe") && !loadFileLower.EndsWith(".dll") && !loadFileLower.EndsWith(".bin") && !loadFileLower.EndsWith(".tmp") && !loadFileLower.EndsWith(".bat"))
                        {
                            imageToBitmapImage.UriSource = new Uri(loadFileLower, UriKind.RelativeOrAbsolute);
                        }
                        else if (File.Exists(loadFileLower) && (loadFileLower.EndsWith(".exe") || loadFileLower.EndsWith(".dll") || loadFileLower.EndsWith(".bin") || loadFileLower.EndsWith(".tmp")))
                        {
                            imageToBitmapImage.StreamSource = GetIconMemoryStreamFromExeFile(loadFileLower, iconIndex, ref imageMemoryStream);
                        }
                        else if (sourceFolders != null)
                        {
                            foreach (ImageSourceFolders sourceFolder in sourceFolders)
                            {
                                try
                                {
                                    if (Directory.Exists(sourceFolder.SourcePath))
                                    {
                                        string[] pngImages = Directory.GetFiles(sourceFolder.SourcePath, "*.png", sourceFolder.SearchOption);
                                        string[] jpgImages = Directory.GetFiles(sourceFolder.SourcePath, "*.jpg", sourceFolder.SearchOption);
                                        IEnumerable<string> foundImages = pngImages.Concat(jpgImages);
                                        string foundImage = foundImages.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x).ToLower() == loadFileLower);
                                        if (!string.IsNullOrWhiteSpace(foundImage))
                                        {
                                            imageToBitmapImage.UriSource = new Uri(foundImage, UriKind.RelativeOrAbsolute);
                                            break;
                                        }
                                    }
                                }
                                catch { }
                            }
                        }

                        //Return application bitmap image
                        if (imageToBitmapImage.UriSource != null)
                        {
                            return EndBitmapImage(imageToBitmapImage, ref imageMemoryStream);
                        }
                        else if (imageToBitmapImage.StreamSource != null && imageToBitmapImage.StreamSource.Length > 75)
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
                    MemoryStream windowImage = GetIconMemoryStreamFromWindow(windowHandle, ref imageMemoryStream);
                    if (windowImage != null)
                    {
                        imageToBitmapImage.StreamSource = windowImage;
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