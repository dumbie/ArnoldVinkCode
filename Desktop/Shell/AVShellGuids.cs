using System;

namespace ArnoldVinkCode
{
    public partial class AVShell
    {
        //Interface identifiers
        public const string IID_ShellItem = "43826D1E-E718-42EE-BC55-A1E261C37BFE";
        public const string IID_ShellItem2 = "7E9FB0D3-919F-4307-AB2E-9B1860310C93";

        //Service identifiers
        public static readonly Guid SID_STopLevelBrowser = new Guid("4C96BE40-915C-11CF-99D3-00AA004AE837");

        //Class identifiers
        public static readonly Guid CLSID_ShellWindows = new Guid("9BA05972-F6A8-11CF-A442-00A0C90A8F39");
        public static readonly Guid CLSID_LocalThumbnailCache = new Guid("50EF4544-AC9F-4A8E-B21B-8A26180DB13F");
    }
}