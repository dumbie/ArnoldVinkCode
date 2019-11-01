using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public interface ArnoldVinkFiles
    {
        //Saving files
        Task SaveText(string FileName, string FileText);
        Task SaveBytes(string FileName, byte[] FileBytes);

        //Loading files
        Task<string> LoadText(string FileName);
        Task<byte[]> LoadBytes(string FileName);

        //Rename files
        //Task<bool> FileRename(string FileNameOld, string FileNameNew);

        //Delete files
        Task<bool> FileDelete(string FileName);

        //Check files
        Task<bool> FileExists(string FileName);

        //List files
        //Task<string[]> FileList(string Directory);
    }
}