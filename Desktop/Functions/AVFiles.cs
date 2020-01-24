using System.Diagnostics;
using System.IO;

namespace ArnoldVinkCode
{
    public partial class AVFiles
    {
        //Directory remove
        public static void Directory_Remove(string removeDirectoryPath)
        {
            try
            {
                if (Directory.Exists(removeDirectoryPath))
                {
                    Debug.WriteLine("Removing: " + removeDirectoryPath);
                    Directory.Delete(removeDirectoryPath, true);
                }
            }
            catch
            {
                Debug.WriteLine("Failed removing directory: " + removeDirectoryPath);
            }
        }

        //File remove
        public static void File_Remove(string removeFilePath)
        {
            try
            {
                if (File.Exists(removeFilePath))
                {
                    Debug.WriteLine("Removing: " + removeFilePath);
                    File.Delete(removeFilePath);
                }
            }
            catch
            {
                Debug.WriteLine("Failed removing file: " + removeFilePath);
            }
        }

        //File rename
        public static void File_Rename(string oldFilePath, string newFilePath, bool overWrite)
        {
            try
            {
                if (File.Exists(oldFilePath))
                {
                    Debug.WriteLine("Renaming: " + oldFilePath + " to " + newFilePath);
                    if (overWrite) { File_Remove(newFilePath); }
                    File.Move(oldFilePath, newFilePath);
                }
            }
            catch
            {
                Debug.WriteLine("Failed renaming file: " + oldFilePath + " to" + newFilePath);
            }
        }
    }
}