using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ArnoldVinkCode
{
    public partial class AVFiles
    {
        //Directory exists check with file path support
        public static bool Directory_Exists(string targetPath)
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(targetPath);
                if (string.IsNullOrWhiteSpace(directoryPath)) { return false; }
                if (!Directory.Exists(directoryPath))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch { return false; }
        }

        //Directory create
        public static bool Directory_Create(string createDirectoryPath, bool overWrite)
        {
            try
            {
                if (overWrite && Directory.Exists(createDirectoryPath))
                {
                    Directory_Delete(createDirectoryPath);
                }
                else if (!overWrite && Directory.Exists(createDirectoryPath))
                {
                    Debug.WriteLine("Failed creating directory: " + createDirectoryPath + " already exist");
                    return true;
                }

                Debug.WriteLine("Creating directory: " + createDirectoryPath);
                Directory.CreateDirectory(createDirectoryPath);
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed creating directory: " + createDirectoryPath);
                return false;
            }
        }

        //Directory delete
        public static bool Directory_Delete(string deleteDirectoryPath)
        {
            try
            {
                if (Directory.Exists(deleteDirectoryPath))
                {
                    Debug.WriteLine("Deleting directory: " + deleteDirectoryPath);
                    Directory.Delete(deleteDirectoryPath, true);
                    return true;
                }
                else
                {
                    Debug.WriteLine("Failed deleting directory: " + deleteDirectoryPath + " does not exist");
                    return true;
                }
            }
            catch
            {
                Debug.WriteLine("Failed deleting directory: " + deleteDirectoryPath);
                return false;
            }
        }

        //Directory move
        public static bool Directory_Move(string oldDirPath, string newDirPath, bool overWrite)
        {
            try
            {
                if (oldDirPath == newDirPath)
                {
                    Debug.WriteLine("Failed moving directory: targeting the same path.");
                    return false;
                }

                if (Directory.Exists(oldDirPath))
                {
                    Debug.WriteLine("Moving: " + oldDirPath + " to " + newDirPath);
                    if (overWrite) { Directory_Delete(newDirPath); }
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

        //Directory copy
        public static bool Directory_Copy(string oldDirPath, string newDirPath, bool overWrite)
        {
            try
            {
                if (oldDirPath == newDirPath)
                {
                    Debug.WriteLine("Failed copying directory: targeting the same path.");
                    return false;
                }

                if (Directory.Exists(oldDirPath))
                {
                    Debug.WriteLine("Copying: " + oldDirPath + " to " + newDirPath);

                    //Get old directory info
                    DirectoryInfo dirInfo = new DirectoryInfo(oldDirPath);
                    DirectoryInfo[] directories = dirInfo.GetDirectories();

                    //Create new directory
                    if (!Directory_Create(newDirPath, overWrite))
                    {
                        Debug.WriteLine("Failed copying directory: failed to create new directory.");
                        return false;
                    }

                    //Get old directory files (root)
                    foreach (FileInfo file in dirInfo.GetFiles())
                    {
                        file.CopyTo(Path.Combine(newDirPath, file.Name), overWrite);
                    }

                    //Get old directory files (subdir)
                    foreach (DirectoryInfo subDir in directories)
                    {
                        Directory_Copy(subDir.FullName, Path.Combine(newDirPath, subDir.Name), overWrite);
                    }

                    return true;
                }
                else
                {
                    Debug.WriteLine("Failed copying directory: " + oldDirPath + " does not exist");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed copying directory: " + oldDirPath + " to " + newDirPath + " (" + ex.Message + ")");
                return false;
            }
        }

        //Get files from directory with depth level
        public static List<string> GetFilesLevel(string directory, string searchPattern, int depthLevel)
        {
            List<string> fileList = new List<string>();
            try
            {
                foreach (string fileInfo in Directory.GetFiles(directory, searchPattern, SearchOption.TopDirectoryOnly))
                {
                    fileList.Add(fileInfo);
                }

                foreach (string directoryInfo in Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly))
                {
                    if (depthLevel > 0)
                    {
                        GetFilesLevel(directoryInfo, searchPattern, depthLevel--);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed getting files with level: " + ex.Message);
            }
            return fileList;
        }

        //File delete
        public static bool File_Delete(string deleteFilePath)
        {
            try
            {
                if (File.Exists(deleteFilePath))
                {
                    Debug.WriteLine("Deleting file: " + deleteFilePath);
                    File.Delete(deleteFilePath);
                    return true;
                }
                else
                {
                    Debug.WriteLine("Failed deleting file: " + deleteFilePath + " does not exist");
                    return true;
                }
            }
            catch
            {
                Debug.WriteLine("Failed deleting file: " + deleteFilePath);
                return false;
            }
        }

        //File move
        public static bool File_Move(string oldFilePath, string newFilePath, bool overWrite)
        {
            try
            {
                if (oldFilePath == newFilePath)
                {
                    Debug.WriteLine("Failed moving file: targeting the same path.");
                    return false;
                }

                if (File.Exists(oldFilePath))
                {
                    Debug.WriteLine("Moving: " + oldFilePath + " to " + newFilePath);
                    if (overWrite) { File_Delete(newFilePath); }
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
        public static bool File_Copy(string oldFilePath, string newFilePath, bool overWrite)
        {
            try
            {
                if (oldFilePath == newFilePath)
                {
                    Debug.WriteLine("Failed copying file: targeting the same path.");
                    return false;
                }

                if (File.Exists(oldFilePath))
                {
                    Debug.WriteLine("Copying: " + oldFilePath + " to " + newFilePath);
                    File.Copy(oldFilePath, newFilePath, overWrite);
                    return true;
                }
                else
                {
                    Debug.WriteLine("Failed copying file: " + oldFilePath + " does not exist");
                    return false;
                }
            }
            catch
            {
                Debug.WriteLine("Failed copying file: " + oldFilePath + " to" + newFilePath);
                return false;
            }
        }

        //Convert file to a string
        public static string FileToString(string[] filePaths)
        {
            try
            {
                foreach (string filePath in filePaths)
                {
                    try
                    {
                        //Validate file name
                        if (string.IsNullOrWhiteSpace(filePath)) { continue; }

                        //Adjust file name
                        string filePathLower = filePath.ToLower().Trim();
                        //Debug.WriteLine("Loading text: " + loadFileLower);

                        //Read the text file
                        if (File.Exists(filePathLower))
                        {
                            return File.ReadAllText(filePathLower);
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return string.Empty;
        }

        //Write string to file
        public static bool StringToFile(string filePath, string writeString)
        {
            try
            {
                //Check and create directory
                if (!Directory_Exists(filePath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                }

                //Write to file
                File.WriteAllText(filePath, writeString);

                //Return result
                return true;
            }
            catch
            {
                //Return result
                return false;
            }
        }

        //Write bytes to file
        public static bool BytesToFile(string filePath, byte[] writeBytes)
        {
            try
            {
                //Check and create directory
                if (!Directory_Exists(filePath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                }

                //Write to file
                File.WriteAllBytes(filePath, writeBytes);

                //Return result
                return true;
            }
            catch
            {
                //Return result
                return false;
            }
        }

        //Replace invalid file characters
        public static string FileNameReplaceInvalidChars(string fileName, string replaceWith)
        {
            try
            {
                return string.Join(replaceWith, fileName.Split(Path.GetInvalidFileNameChars())).Trim();
            }
            catch { }
            return fileName;
        }

        //Convert environment variables in file path
        public static string ConvertEnvironmentPath(string filePath)
        {
            try
            {
                //Custom variables
                filePath = filePath.Replace("%DESKTOPUSER%", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                filePath = filePath.Replace("%DESKTOPPUBLIC%", Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory));
                filePath = filePath.Replace("%PICTURES%", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
                filePath = filePath.Replace("%VIDEOS%", Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));

                //Windows variables
                filePath = Environment.ExpandEnvironmentVariables(filePath);
            }
            catch { }
            return filePath;
        }
    }
}