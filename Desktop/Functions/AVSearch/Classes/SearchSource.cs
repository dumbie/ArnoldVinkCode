using System.IO;

namespace ArnoldVinkCode
{
    public partial class AVSearch
    {
        public class SearchSource
        {
            public string SearchPath { get; set; }
            public string[] SearchPatterns { get; set; } = new string[] { "*" };
            public SearchOption SearchOption { get; set; } = SearchOption.TopDirectoryOnly;
        }
    }
}