using System;
using System.Diagnostics;
using System.IO;
using Xamarin.Forms;

namespace ArnoldVinkCode
{
    public class AVFiles
    {
        //Directory create
        public static bool Directory_Create(string directoryPath, bool overWrite, bool localData)
        {
            try
            {
                if (localData)
                {
                    string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    directoryPath = Path.Combine(localFolder, directoryPath);
                }

                if (overWrite && Directory.Exists(directoryPath))
                {
                    Debug.WriteLine("Deleting directory: " + directoryPath);
                    Directory_Delete(directoryPath, false);
                }
                else if (!overWrite && Directory.Exists(directoryPath))
                {
                    Debug.WriteLine("Failed creating directory: " + directoryPath + " already exist");
                    return true;
                }

                Debug.WriteLine("Creating directory: " + directoryPath);
                Directory.CreateDirectory(directoryPath);
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed creating directory: " + directoryPath);
                return false;
            }
        }

        //Directory delete
        public static bool Directory_Delete(string directoryPath, bool localData)
        {
            try
            {
                if (localData)
                {
                    string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    directoryPath = Path.Combine(localFolder, directoryPath);
                }

                Debug.WriteLine("Deleting directory: " + directoryPath);
                Directory.Delete(directoryPath, true);
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed deleting directory: " + directoryPath);
                return false;
            }
        }

        //Directory move
        public static bool Directory_Move(string oldDirPath, string newDirPath, bool overWrite, bool localData)
        {
            try
            {
                if (localData)
                {
                    string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    oldDirPath = Path.Combine(localFolder, oldDirPath);
                    newDirPath = Path.Combine(localFolder, newDirPath);
                }

                if (Directory.Exists(oldDirPath))
                {
                    Debug.WriteLine("Moving directory: " + oldDirPath + " to " + newDirPath);
                    if (overWrite) { Directory_Delete(newDirPath, false); }
                    Directory.Move(oldDirPath, newDirPath);
                    return true;
                }
                else
                {
                    Debug.WriteLine("Failed moving directory: " + oldDirPath + " does not exist");
                    return false;
                }
            }
            catch
            {
                Debug.WriteLine("Failed moving directory: " + oldDirPath + " to" + newDirPath);
                return false;
            }
        }

        //Directory list files
        public static string[] Directory_ListFiles(string directoryPath, bool localData)
        {
            try
            {
                if (localData)
                {
                    string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    directoryPath = Path.Combine(localFolder, directoryPath);
                }

                Debug.WriteLine("Listing files from: " + directoryPath);
                return Directory.GetFiles(directoryPath);
            }
            catch
            {
                Debug.WriteLine("Failed listing files.");
                return null;
            }
        }

        //File exists
        public static bool File_Exists(string fileName, bool localData)
        {
            try
            {
                if (localData)
                {
                    string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    fileName = Path.Combine(localFolder, fileName);
                }

                return File.Exists(fileName);
            }
            catch
            {
                Debug.WriteLine("Failed checking file existance.");
                return false;
            }
        }

        //File delete
        public static bool File_Delete(string fileName, bool localData)
        {
            try
            {
                if (localData)
                {
                    string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    fileName = Path.Combine(localFolder, fileName);
                }

                Debug.WriteLine("Deleting file: " + fileName);
                File.Delete(fileName);
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed deleting file: " + fileName);
                return false;
            }
        }

        //File move
        public static bool File_Move(string oldFilePath, string newFilePath, bool overWrite, bool localData)
        {
            try
            {
                if (localData)
                {
                    string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    oldFilePath = Path.Combine(localFolder, oldFilePath);
                    newFilePath = Path.Combine(localFolder, newFilePath);
                }

                if (File.Exists(oldFilePath))
                {
                    Debug.WriteLine("Moving file: " + oldFilePath + " to " + newFilePath);
                    if (overWrite) { File_Delete(newFilePath, false); }
                    File.Move(oldFilePath, newFilePath);
                    return true;
                }
                else
                {
                    Debug.WriteLine("Failed moving file: " + oldFilePath + " does not exist");
                    return false;
                }
            }
            catch
            {
                Debug.WriteLine("Failed moving file: " + oldFilePath + " to" + newFilePath);
                return false;
            }
        }

        //File copy
        public static bool File_Copy(string oldFilePath, string newFilePath, bool overWrite, bool localData)
        {
            try
            {
                if (localData)
                {
                    string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    oldFilePath = Path.Combine(localFolder, oldFilePath);
                    newFilePath = Path.Combine(localFolder, newFilePath);
                }

                Debug.WriteLine("Copying file: " + oldFilePath + " to " + newFilePath);
                File.Copy(oldFilePath, newFilePath, overWrite);
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed copying file: " + oldFilePath + " to" + newFilePath);
                return false;
            }
        }

        //File save text
        public static bool File_SaveText(string fileName, string fileText, bool overWrite, bool localData)
        {
            try
            {
                if (localData)
                {
                    string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    fileName = Path.Combine(localFolder, fileName);
                }

                if (overWrite || !File.Exists(fileName))
                {
                    Debug.WriteLine("Writing text to: " + fileName);
                    File.WriteAllText(fileName, fileText);
                    return true;
                }
                else
                {
                    Debug.WriteLine("Failed writing text to: " + fileName + " file already exists.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed writing text to file: " + ex.Message);
                return false;
            }
        }

        //File save stream
        public static bool File_SaveStream(string fileName, Stream fileStream, bool overWrite, bool localData)
        {
            try
            {
                if (localData)
                {
                    string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    fileName = Path.Combine(localFolder, fileName);
                }

                if (overWrite || !File.Exists(fileName))
                {
                    Debug.WriteLine("Writing stream to: " + fileName);
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        if (fileStream.CanSeek) { fileStream.Position = 0; }
                        fileStream.CopyTo(memoryStream);
                        byte[] imageBytes = memoryStream.ToArray();
                        File.WriteAllBytes(fileName, imageBytes);
                        return true;
                    }
                }
                else
                {
                    Debug.WriteLine("Failed writing stream to: " + fileName + " file already exists.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed writing stream to file: " + ex.Message);
                return false;
            }
        }

        //File save bytes
        public static bool File_SaveBytes(string fileName, byte[] fileBytes, bool overWrite, bool localData)
        {
            try
            {
                if (localData)
                {
                    string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    fileName = Path.Combine(localFolder, fileName);
                }

                if (overWrite || !File.Exists(fileName))
                {
                    Debug.WriteLine("Writing bytes to: " + fileName);
                    File.WriteAllBytes(fileName, fileBytes);
                    return true;
                }
                else
                {
                    Debug.WriteLine("Failed writing bytes to: " + fileName + " file already exists.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed writing bytes to file: " + ex.Message);
                return false;
            }
        }

        //File load image
        public static ImageSource File_LoadImage(string fileName, bool localData)
        {
            try
            {
                if (localData)
                {
                    string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    fileName = Path.Combine(localFolder, fileName);
                }

                Debug.WriteLine("Loading image file: " + fileName);
                byte[] fileBytes = File.ReadAllBytes(fileName);
                MemoryStream imageStream = new MemoryStream(fileBytes);
                if (imageStream.CanSeek) { imageStream.Position = 0; }
                return ImageSource.FromStream(() => imageStream);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed loading image file: " + ex.Message);
                return null;
            }
        }

        //File load text
        public static string File_LoadText(string fileName, bool localData)
        {
            try
            {
                if (localData)
                {
                    string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    fileName = Path.Combine(localFolder, fileName);
                }

                Debug.WriteLine("Loading text from: " + fileName);
                return File.ReadAllText(fileName);
            }
            catch
            {
                Debug.WriteLine("Failed loading text from file.");
                return string.Empty;
            }
        }

        //File load stream
        public static Stream File_LoadStream(string fileName, bool localData)
        {
            try
            {
                if (localData)
                {
                    string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    fileName = Path.Combine(localFolder, fileName);
                }

                Debug.WriteLine("Loading stream from: " + fileName);
                byte[] fileBytes = File.ReadAllBytes(fileName);

                MemoryStream imageStream = new MemoryStream(fileBytes);
                if (imageStream.CanSeek) { imageStream.Position = 0; }
                return imageStream;
            }
            catch
            {
                Debug.WriteLine("Failed loading stream from file.");
                return null;
            }
        }

        //File load bytes
        public static byte[] File_LoadBytes(string fileName, bool localData)
        {
            try
            {
                if (localData)
                {
                    string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    fileName = Path.Combine(localFolder, fileName);
                }

                Debug.WriteLine("Loading bytes from: " + fileName);
                return File.ReadAllBytes(fileName);
            }
            catch
            {
                Debug.WriteLine("Failed loading bytes from file.");
                return null;
            }
        }

        //File check size
        public static long File_Size(string fileName, bool localData)
        {
            try
            {
                if (localData)
                {
                    string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    fileName = Path.Combine(localFolder, fileName);
                }

                FileInfo fileInfo = new FileInfo(fileName);
                return fileInfo.Length;
            }
            catch
            {
                Debug.WriteLine("Failed loading bytes from file.");
                return -1;
            }
        }

        //File check creation time
        public static DateTime File_CreationTime(string fileName, bool localData)
        {
            try
            {
                if (localData)
                {
                    string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    fileName = Path.Combine(localFolder, fileName);
                }

                FileInfo fileInfo = new FileInfo(fileName);
                return fileInfo.CreationTime;
            }
            catch
            {
                Debug.WriteLine("Failed loading creation time from file.");
                return DateTime.Now;
            }
        }
    }
}