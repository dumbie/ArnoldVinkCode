using System;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcessLaunch
    {
        //Enumerations
        public enum SHELL_EXECUTE_SEE_MASK : int
        {
            SEE_MASK_DEFAULT = 0x00000000,
            SEE_MASK_CLASSNAME = 0x00000001,
            SEE_MASK_CLASSKEY = 0x00000003,
            SEE_MASK_IDLIST = 0x00000004,
            SEE_MASK_INVOKEIDLIST = 0x0000000c,
            SEE_MASK_ICON = 0x00000010,
            SEE_MASK_HOTKEY = 0x00000020,
            SEE_MASK_NOCLOSEPROCESS = 0x00000040,
            SEE_MASK_CONNECTNETDRV = 0x00000080,
            SEE_MASK_NOASYNC = 0x00000100,
            SEE_MASK_FLAG_DDEWAIT = 0x00000100,
            SEE_MASK_DOENVSUBST = 0x00000200,
            SEE_MASK_FLAG_NO_UI = 0x00000400,
            SEE_MASK_UNICODE = 0x00004000,
            SEE_MASK_NO_CONSOLE = 0x00008000,
            SEE_MASK_ASYNCOK = 0x00100000,
            SEE_MASK_NOQUERYCLASSSTORE = 0x01000000,
            SEE_MASK_HMONITOR = 0x00200000,
            SEE_MASK_NOZONECHECKS = 0x00800000,
            SEE_MASK_WAITFORINPUTIDLE = 0x02000000,
            SEE_MASK_FLAG_LOG_USAGE = 0x04000000,
            SEE_MASK_FLAG_HINST_IS_SITE = 0x08000000
        }

        //Classes
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public class ShellExecuteInfo
        {
            public ShellExecuteInfo()
            {
                cbSize = Marshal.SizeOf(this);
            }

            public int cbSize { get; set; }
            public SHELL_EXECUTE_SEE_MASK fMask { get; set; }
            public IntPtr hWnd { get; set; }
            public string lpVerb { get; set; }
            public string lpFile { get; set; }
            public string lpParameters { get; set; }
            public string lpDirectory { get; set; }
            public WindowShowCommand nShow { get; set; }
            public IntPtr hInstApp { get; set; }
            public IntPtr lpIDList { get; set; }
            public string lpClass { get; set; }
            public IntPtr hkeyClass { get; set; }
            public int dwHotKey { get; set; }
            public ShellExecuteInfoUnion dUnion { get; set; }
            public IntPtr hProcess { get; set; }
        }

        public class ShellExecuteInfoUnion
        {
            public IntPtr hIcon { get; set; }
            public IntPtr hMonitor { get; set; }
        }

        //Imports
        [DllImport("shell32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool ShellExecuteExW([In, Out] ShellExecuteInfo pExecInfo);
    }
}