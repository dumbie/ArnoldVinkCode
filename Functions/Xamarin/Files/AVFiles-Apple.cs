using ArnoldVinkCode;
using Foundation;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
                File.WriteAllText(GetPathToFileName(FileName, true), FileText, Encoding.UTF8);
            }
            catch { Debug.WriteLine("Failed saving Text to file: " + FileName); }
        }

        //Save Bytes to file
        public async Task SaveBytes(string FileName, byte[] FileBytes)
        {
            try
            {
                File.WriteAllBytes(GetPathToFileName(FileName, true), FileBytes);
            }
            catch { Debug.WriteLine("Failed saving Bytes to file: " + FileName); }
        }

        //Load Text from file
        public async Task<string> LoadText(string FileName)
        {
            try
            {
                return File.ReadAllText(GetPathToFileName(FileName, true));
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
                return File.ReadAllBytes(GetPathToFileName(FileName, true));
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
                File.Delete(GetPathToFileName(FileName, true));
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
                return File.Exists(GetPathToFileName(FileName, true));
            }
            catch
            {
                Debug.WriteLine("Could not find a requested local file: " + FileName);
                return false;
            }
        }

        //Get the file path
        string GetPathToFileName(string FileName, bool Library)
        {
            try
            {
                string FolderPath = String.Empty;
                    if(Library)
                {
                    FolderPath = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.LibraryDirectory, NSSearchPathDomain.User).Last().Path;
                }
                else
                {
                    FolderPath = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User).Last().Path;
                }

                return Path.Combine(FolderPath, FileName);
            }
            catch { return String.Empty; }
        }
    }
}