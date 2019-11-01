using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
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
                StorageFile CreateFileAsync = await ApplicationData.Current.LocalFolder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(CreateFileAsync, FileText, UnicodeEncoding.Utf8);
            }
            catch { Debug.WriteLine("Failed saving Text to file: " + FileName); }
        }

        //Save Bytes to file
        public async Task SaveBytes(string FileName, byte[] FileBytes)
        {
            try
            {
                StorageFile CreateFileAsync = await ApplicationData.Current.LocalFolder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteBytesAsync(CreateFileAsync, FileBytes);
            }
            catch { Debug.WriteLine("Failed saving Bytes to file: " + FileName); }
        }

        //Load Text from file
        public async Task<string> LoadText(string FileName)
        {
            try
            {
                StorageFile LoadFileAsync = await ApplicationData.Current.LocalFolder.GetFileAsync(FileName);
                return await FileIO.ReadTextAsync(LoadFileAsync);
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
                StorageFile LoadFileAsync = await ApplicationData.Current.LocalFolder.GetFileAsync(FileName);
                return (await FileIO.ReadBufferAsync(LoadFileAsync)).ToArray();
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
                IStorageItem IStorageItem = await ApplicationData.Current.LocalFolder.GetItemAsync(FileName);
                await IStorageItem.DeleteAsync(StorageDeleteOption.PermanentDelete);
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
            try { return await ApplicationData.Current.LocalFolder.TryGetItemAsync(FileName) != null; }
            catch
            {
                Debug.WriteLine("Could not find a requested local file: " + FileName);
                return false;
            }
        }
    }
}