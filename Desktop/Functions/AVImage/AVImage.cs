using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVSearch;

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

        //Convert BitmapImage to png file
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

        //Convert file to a BitmapImage
        public static BitmapImage FileToBitmapImage(string[] fileNames, SearchSource[] searchSources, string sourceBackup, IntPtr windowHandle, int pixelWidth, int iconIndex)
        {
            try
            {
                //Prepare application bitmap image
                BitmapImage imageToBitmapImage = BeginBitmapImage(pixelWidth);
                MemoryStream imageMemoryStream = new MemoryStream();

                //Load application bitmap image
                foreach (string fileName in fileNames)
                {
                    try
                    {
                        //Validate the file name
                        if (string.IsNullOrWhiteSpace(fileName)) { continue; }

                        //Adjust the file name
                        string loadFileLower = fileName.ToLower();
                        Debug.WriteLine("Loading image: " + loadFileLower);

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
                        else if (searchSources != null)
                        {
                            try
                            {
                                string[] foundImages = Search_Files(new string[] { fileName }, searchSources, false);
                                if (foundImages.Any())
                                {
                                    imageToBitmapImage.UriSource = new Uri(foundImages.FirstOrDefault(), UriKind.RelativeOrAbsolute);
                                }
                            }
                            catch { }
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
                        Debug.WriteLine("Failed loading image: " + fileName + "/" + ex.Message);
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