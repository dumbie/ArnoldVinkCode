using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(AVFiles))]
namespace ArnoldVinkCode
{
    public class AVFiles : ArnoldVinkFiles
    {
        //Save Text to file
        public async Task SaveText(string FileName, string FileText)
        {
            try
            {
                File.WriteAllText(GetPathToFileName(FileName), FileText, Encoding.UTF8);
            }
            catch { Debug.WriteLine("Failed saving Text to file: " + FileName); }
        }

        //Save Bytes to file
        public async Task SaveBytes(string FileName, byte[] FileBytes)
        {
            try
            {
                File.WriteAllBytes(GetPathToFileName(FileName), FileBytes);
            }
            catch { Debug.WriteLine("Failed saving Bytes to file: " + FileName); }
        }

        //Load Text from file
        public async Task<string> LoadText(string FileName)
        {
            try
            {
                return File.ReadAllText(GetPathToFileName(FileName));
            }
            catch
            {
                Debug.WriteLine("Failed loading text: " + FileName);
                return String.Empty;
            }
        }

        //Load Bytes from file
        public async Task<byte[]> LoadBytes(string FileName)
        {
            try
            {
                return File.ReadAllBytes(GetPathToFileName(FileName));
            }
            catch
            {
                Debug.WriteLine("Failed loading bytes: " + FileName);
                return null;
            }
        }

        //Delete a file
        public async Task<bool> FileDelete(string FileName)
        {
            try
            {
                File.Delete(GetPathToFileName(FileName));
                return true;
            }
            catch
            {
                Debug.WriteLine("Could not delete a local file: " + FileName);
                return false;
            }
        }

        //Check if a file exists
        public async Task<bool> FileExists(string FileName)
        {
            try
            {
                return File.Exists(GetPathToFileName(FileName));
            }
            catch
            {
                Debug.WriteLine("Could not find a requested local file: " + FileName);
                return false;
            }
        }

        //Get the file path
        string GetPathToFileName(string FileName)
        {
            try
            {
                string FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                return Path.Combine(FolderPath, FileName);
            }
            catch { return String.Empty; }
        }
    }
}