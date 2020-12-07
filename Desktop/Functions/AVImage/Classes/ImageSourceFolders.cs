using System.IO;

namespace ArnoldVinkCode
{
    public partial class AVImage
    {
        public class ImageSourceFolders
        {
            public string SourcePath { get; set; }
            public SearchOption SearchOption { get; set; } = SearchOption.TopDirectoryOnly;
        }
    }
}