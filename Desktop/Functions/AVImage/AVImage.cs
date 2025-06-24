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
        //Get BitmapImage from cached file
        public static BitmapImage FileCacheToBitmapImage(string filePath, string fileBackupPath, int imageWidth, int imageHeight, bool extractIcon)
        {
            try
            {
                //Load BitmapImage from cached file
                BitmapImage bitmapImage = GetBitmapImageFromCachedFile(filePath, imageWidth, imageHeight, extractIcon);
                if (bitmapImage != null) { return bitmapImage; }

                //Image not found, load backup
                return GetBitmapImageFromUri(new Uri(fileBackupPath, UriKind.RelativeOrAbsolute), imageWidth, imageHeight);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed loading image cache: " + filePath + " / " + ex.Message);
                return null;
            }
        }

        //Get BitmapImage from file or window
        public static BitmapImage FileToBitmapImage(string[] filePaths, SearchSource[] searchSources, string fileBackupPath, int imageWidth, int imageHeight, IntPtr windowHandle, int iconIndex)
        {
            try
            {
                foreach (string filePath in filePaths)
                {
                    try
                    {
                        //Validate file path
                        if (string.IsNullOrWhiteSpace(filePath)) { continue; }

                        //Adjust file path
                        string filePathLower = filePath.ToLower().Trim();
                        if (filePathLower.Contains("/") && filePathLower.Contains("\\"))
                        {
                            filePathLower = filePathLower.Replace("/", "\\");
                        }
                        Debug.WriteLine("Loading image: " + filePathLower);

                        //Check file exists
                        bool fileExists = File.Exists(filePathLower);

                        if (filePathLower.StartsWith("pack://") || filePathLower.StartsWith("http://") || filePathLower.StartsWith("https://"))
                        {
                            BitmapImage bitmapImage = GetBitmapImageFromUri(new Uri(filePathLower, UriKind.RelativeOrAbsolute), imageWidth, imageHeight);
                            if (bitmapImage != null) { return bitmapImage; }
                        }
                        if (fileExists && filePathLower.EndsWith(".ico"))
                        {
                            BitmapImage bitmapImage = GetBitmapImageFromIcoFile(filePathLower, imageWidth, imageHeight);
                            if (bitmapImage != null) { return bitmapImage; }
                        }
                        if (fileExists && (filePathLower.EndsWith(".exe") || filePathLower.EndsWith(".dll") || filePathLower.EndsWith(".bin")))
                        {
                            BitmapImage bitmapImage = GetBitmapImageFromExeFile(filePathLower, iconIndex, imageWidth, imageHeight);
                            if (bitmapImage != null) { return bitmapImage; }
                        }
                        if (fileExists)
                        {
                            BitmapImage bitmapImage = GetBitmapImageFromUri(new Uri(filePathLower, UriKind.RelativeOrAbsolute), imageWidth, imageHeight);
                            if (bitmapImage != null) { return bitmapImage; }
                        }
                        if (fileExists)
                        {
                            BitmapImage bitmapImage = GetBitmapImageFromCachedFile(filePathLower, imageWidth, imageHeight, true);
                            if (bitmapImage != null) { return bitmapImage; }
                        }
                        if (searchSources != null)
                        {
                            string foundImage = Search_Files(new string[] { filePathLower }, searchSources, false).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(foundImage))
                            {
                                return GetBitmapImageFromUri(new Uri(foundImage, UriKind.RelativeOrAbsolute), imageWidth, imageHeight);
                            }
                        }
                    }
                    catch { }
                }

                //Image not found, load window icon
                if (windowHandle != IntPtr.Zero)
                {
                    BitmapImage bitmapImage = GetBitmapImageFromWindow(windowHandle, imageWidth, imageHeight);
                    if (bitmapImage != null) { return bitmapImage; }
                }

                //Image not found, load backup
                return GetBitmapImageFromUri(new Uri(fileBackupPath, UriKind.RelativeOrAbsolute), imageWidth, imageHeight);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed loading image file: " + ex.Message);
                return null;
            }
        }
    }
}