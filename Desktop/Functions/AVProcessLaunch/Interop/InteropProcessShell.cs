using System;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVProcessLaunch
    {
        //Classes
        [StructLayout(LayoutKind.Sequential)]
        public class ShellExecuteInfo
        {
            public ShellExecuteInfo()
            {
                cbSize = Marshal.SizeOf(this);
            }

            public int cbSize { get; set; }
            public int fMask { get; set; }
            public IntPtr hWnd { get; set; }
            public IntPtr lpVerb { get; set; }
            public IntPtr lpFile { get; set; }
            public IntPtr lpParameters { get; set; }
            public IntPtr lpDirectory { get; set; }
            public int nShow { get; set; }
            public IntPtr hInstApp { get; set; }
            public IntPtr lpIDList { get; set; }
            public IntPtr lpClass { get; set; }
            public IntPtr hkeyClass { get; set; }
            public int dwHotKey { get; set; }
            public IntPtr hIcon { get; set; }
            public IntPtr hProcess { get; set; }
        }

        //Imports
        [DllImport("shell32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool ShellExecuteExW(ShellExecuteInfo info);
    }
}