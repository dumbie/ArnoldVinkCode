using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace ArnoldVinkCode
{
    public partial class AVShell
    {
        //Interop
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern int SHCreateItemFromParsingName([In, MarshalAs(UnmanagedType.LPWStr)] string pszPath, [In] IntPtr pbc, [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out object ppv);
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern int SHGetPropertyStoreForWindow(IntPtr hWnd, ref Guid riidPropStore, out IPropertyStore propertyStore);
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern int SHGetPropertyStoreFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath, IntPtr pbc, GETPROPERTYSTORE_FLAGS flags, ref Guid riidPropStore, out IPropertyStore propertyStore);
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern int SHLoadIndirectString(string pszSource, StringBuilder pszOutBuf, int cchOutBuf, IntPtr ppvReserved);
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern int SHCreateStreamOnFileEx(string pszFile, STGM_MODES grfMode, int dwAttributes, bool fCreate, IntPtr pstmTemplate, out IStream ppstm);
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern int SHFileOperation(ref SHFILEOPSTRUCT shFileOpstruct);
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern int SHEmptyRecycleBin(IntPtr hWnd, string pszRootPath, RecycleBin_FLAGS dwFlags);
    }
}