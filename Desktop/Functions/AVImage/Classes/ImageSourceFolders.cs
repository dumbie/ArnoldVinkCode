using System.IO;

namespace LibraryShared
{
    public partial class Classes
    {
        public class ImageSourceFolders
        {
            public string SourcePath { get; set; }
            public SearchOption SearchOption { get; set; } = SearchOption.TopDirectoryOnly;
        }
    }
}