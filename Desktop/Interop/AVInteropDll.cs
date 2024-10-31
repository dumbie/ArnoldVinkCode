using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Text;
using static ArnoldVinkCode.AVInteropCom;

namespace ArnoldVinkCode
{
    [SuppressUnmanagedCodeSecurity]
    public partial class AVInteropDll
    {
        //Application DllImports
        [DllImport("kernel32.dll")]
        public static extern bool Wow64DisableWow64FsRedirection(IntPtr ptr);
        [DllImport("user32.dll")]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetWindowText(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder caption, int count);
        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr GetParent(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(WindowPoint p);
        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();
        [DllImport("user32.dll")]
        public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);
        [DllImport("user32.dll")]
        public static extern uint GetDoubleClickTime();
        [DllImport("user32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
        [DllImport("oleacc.dll")]
        public static extern IntPtr GetProcessHandleFromHwnd(IntPtr hWnd);
        [DllImport("kernel32.dll")]
        public static extern int GetProcessId(IntPtr processHandle);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentProcess();
        [DllImport("kernel32.dll")]
        public static extern int GetCurrentProcessId();
        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool IsWindow(IntPtr hWnd);
        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        public static extern int SHGetPropertyStoreForWindow(IntPtr hwnd, ref Guid iid, out IPropertyStore propertyStore);
        [DllImport("kernel32.dll")]
        public static extern bool IsWow64Process(IntPtr processHandle, out bool wow64Process);

        //Close Handle
        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hHandle);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll")]
        public static extern bool DestroyIcon(IntPtr hIcon);

        //Window get
        public enum GetWindowFlags : int
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5
        }
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowFlags flags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetLastActivePopup(IntPtr hWnd);

        //Window show
        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, WindowShowCommand nCmdShow);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern bool BringWindowToTop(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool AllowSetForegroundWindow(int dwProcessId);
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        public const int ASFW_ANY = -1;

        //Window create
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern ushort RegisterClassEx(ref WNDCLASSEX windowClassEx);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateWindowEx(WindowStylesEx dwExStyle, string lpClassName, string lpWindowName, WindowStyles dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);
        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndParent);
        [DllImport("user32.dll")]
        public static extern bool UpdateWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool DestroyWindow(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, LayeredWindowAttributes dwFlags);

        public enum LayeredWindowAttributes : uint
        {
            LWA_NONE = 0x0,
            LWA_COLORKEY = 0x1,
            LWA_ALPHA = 0x2,
            LWA_OPAQUE = 0x4
        }

        public delegate IntPtr WndProcDelegate(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WNDCLASSEX
        {
            public static readonly uint classSize = (uint)Marshal.SizeOf(typeof(WNDCLASSEX));
            public uint cbSize;
            public int style;
            public WndProcDelegate lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public string lpszMenuName;
            public string lpszClassName;
            public IntPtr hIconSm;
        }

        //Window event hook
        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(WinEventHooks eventMin, WinEventHooks eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, WinEventFlags dwFlags);
        public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hWnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
        public enum WinEventFlags : int
        {
            WINEVENT_OUTOFCONTEXT = 0,
            WINEVENT_SKIPOWNTHREAD = 1,
            WINEVENT_SKIPOWNPROCESS = 2,
            WINEVENT_INCONTEXT = 4
        }
        public enum WinEventHooks : int
        {
            EVENT_SYSTEM_FOREGROUND = 3
        }

        //Window redraw
        [DllImport("user32.dll")]
        public static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, RedrawWindowFlags flags);
        public enum RedrawWindowFlags : uint
        {
            Invalidate = 0x1,
            InternalPaint = 0x2,
            Erase = 0x4,
            Validate = 0x8,
            NoInternalPaint = 0x10,
            NoErase = 0x20,
            NoChildren = 0x40,
            AllChildren = 0x80,
            UpdateNow = 0x100,
            EraseNow = 0x200,
            Frame = 0x400,
            NoFrame = 0x800
        }

        //Window display affinity
        [DllImport("user32.dll")]
        public static extern bool SetWindowDisplayAffinity(IntPtr hWnd, DisplayAffinityFlags affinity);
        public enum DisplayAffinityFlags : uint
        {
            WDA_NONE = 0x00,
            WDA_MONITOR = 0x01,
            WDA_EXCLUDEFROMCAPTURE = 0x11
        }

        //SHLW Api
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern int SHLoadIndirectString(string pszSource, StringBuilder pszOutBuf, int cchOutBuf, IntPtr ppvReserved);
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern int SHCreateStreamOnFileEx(string pszFile, STGM_MODES grfMode, int dwAttributes, bool fCreate, IntPtr pstmTemplate, out IStream ppstm);
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern int SHFileOperation(ref SHFILEOPSTRUCT shFileOpstruct);
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern uint SHEmptyRecycleBin(IntPtr hWnd, string pszRootPath, RecycleBin_FLAGS dwFlags);

        public enum RecycleBin_FLAGS : uint
        {
            SHRB_NOCONFIRMATION = 0x00000001,
            SHRB_NOPROGRESSUI = 0x00000002,
            SHRB_NOSOUND = 0x00000004
        }

        public enum FILEOP_FUNC : uint
        {
            FO_MOVE = 0x0001,
            FO_COPY = 0x0002,
            FO_DELETE = 0x0003,
            FO_RENAME = 0x0004
        }

        public enum FILEOP_FLAGS : ushort
        {
            FOF_MULTIDESTFILES = 0x1,
            FOF_CONFIRMMOUSE = 0x2,
            FOF_SILENT = 0x4,
            FOF_RENAMEONCOLLISION = 0x8,
            FOF_NOCONFIRMATION = 0x10,
            FOF_WANTMAPPINGHANDLE = 0x20,
            FOF_ALLOWUNDO = 0x40,
            FOF_FILESONLY = 0x80,
            FOF_SIMPLEPROGRESS = 0x100,
            FOF_NOCONFIRMMKDIR = 0x200,
            FOF_NOERRORUI = 0x400,
            FOF_NOCOPYSECURITYATTRIBS = 0x800,
            FOF_NORECURSION = 0x1000,
            FOF_NO_CONNECTED_ELEMENTS = 0x2000,
            FOF_WANTNUKEWARNING = 0x4000,
            FOF_NORECURSEREPARSE = 0x8000
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHFILEOPSTRUCT
        {
            public IntPtr hWnd;
            [MarshalAs(UnmanagedType.U4)]
            public FILEOP_FUNC wFunc;
            public string pFrom;
            public string pTo;
            public FILEOP_FLAGS fFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            public string lpszProgressTitle;
        }

        public enum STGM_MODES : int
        {
            STGM_READ = 0x00000000,
            STGM_WRITE = 0x00000001,
            STGM_READWRITE = 0x00000002,
            STGM_READWRITE_Bits = 0x00000003,
            STGM_SHARE_DENY_NONE = 0x00000040,
            STGM_SHARE_DENY_READ = 0x00000030,
            STGM_SHARE_DENY_WRITE = 0x00000020,
            STGM_SHARE_EXCLUSIVE = 0x00000010
        }

        //UWP interop
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetApplicationUserModelId(IntPtr hProcess, ref int appUserModelIdLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder sbAppUserModelID);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetPackageFullName(IntPtr hProcess, ref int packageFullNameLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder sbPackageFullName);

        //Load library
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, LoadLibraryFlags dwFlags);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);

        public enum LoadLibraryFlags : uint
        {
            DONT_RESOLVE_DLL_REFERENCES = 0x00000001,
            LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010,
            LOAD_LIBRARY_AS_DATAFILE = 0x00000002,
            LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE = 0x00000040,
            LOAD_LIBRARY_AS_IMAGE_RESOURCE = 0x00000020,
            LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008,
            LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR = 0x00000100,
            LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x00000800,
            LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000
        }

        //Open and close process
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(PROCESS_DESIRED_ACCESS processAccess, bool bInheritHandle, int processId);
        public enum PROCESS_DESIRED_ACCESS : uint
        {
            PROCESS_TERMINATE = 0x0001,
            PROCESS_CREATE_THREAD = 0x0002,
            PROCESS_SET_SESSIONID = 0x0004,
            PROCESS_VM_OPERATION = 0x0008,
            PROCESS_VM_READ = 0x0010,
            PROCESS_VM_WRITE = 0x0020,
            PROCESS_DUP_HANDLE = 0x0040,
            PROCESS_CREATE_PROCESS = 0x0080,
            PROCESS_SET_QUOTA = 0x0100,
            PROCESS_SET_INFORMATION = 0x0200,
            PROCESS_QUERY_INFORMATION = 0x0400,
            PROCESS_SUSPEND_RESUME = 0x0800,
            PROCESS_QUERY_LIMITED_INFORMATION = 0x1000,
            PROCESS_SET_LIMITED_INFORMATION = 0x2000,
            PROCESS_ALL_ACCESS = 0x001FFFFF,
            PROCESS_MAXIMUM_ALLOWED = 0x02000000
        }

        [DllImport("kernel32.dll")]
        public static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        //Open and close thread
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenThread(THREAD_DESIRED_ACCESS dwDesiredAccess, bool bInheritHandle, int dwThreadId);
        public enum THREAD_DESIRED_ACCESS : int
        {
            THREAD_TERMINATE = 0x0001,
            THREAD_SUSPEND_RESUME = 0x0002,
            THREAD_GET_CONTEXT = 0x0008,
            THREAD_SET_CONTEXT = 0x0010,
            THREAD_SET_INFORMATION = 0x0020,
            THREAD_QUERY_INFORMATION = 0x0040,
            THREAD_SET_THREAD_TOKEN = 0x0080,
            THREAD_IMPERSONATE = 0x0100,
            THREAD_DIRECT_IMPERSONATION = 0x0200,
            THREAD_SET_LIMITED_INFORMATION = 0x0400,
            THREAD_QUERY_LIMITED_INFORMATION = 0x0800,
            THREAD_RESUME = 0x1000,
            THREAD_ALL_ACCESS = 0x001FFFFF,
            THREAD_MAXIMUM_ALLOWED = 0x02000000
        }

        [DllImport("kernel32.dll")]
        public static extern int GetProcessIdOfThread(IntPtr tHandle);

        //Enumerate windows
        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsDelegate lpEnumFunc, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowsDelegate lpEnumFunc, IntPtr lParam);
        public delegate bool EnumWindowsDelegate(IntPtr hWnd, IntPtr lParam);

        //Get process details
        [DllImport("psapi.dll")]
        public static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, [In, MarshalAs(UnmanagedType.U4)] int nSize);
        [DllImport("kernel32.dll")]
        public static extern bool QueryFullProcessImageName([In] IntPtr hProcess, [In] int dwFlags, [Out] StringBuilder lpExeName, ref int lpdwSize);

        //Get window ancestor
        [DllImport("user32.dll")]
        public static extern IntPtr GetAncestor(IntPtr hWnd, GetAncestorFlags flags);
        public enum GetAncestorFlags : int
        {
            GetParent = 1,
            GetRoot = 2,
            GetRootOwner = 3
        }

        //Get window dwm attribute
        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, DWM_WINDOW_ATTRIBUTE dwAttribute, out WindowRectangle pvAttribute, int cbAttributeSize);
        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, DWM_WINDOW_ATTRIBUTE dwAttribute, out DWM_CLOAKED_FLAGS pvAttribute, int cbAttributeSize);

        public enum DWM_WINDOW_ATTRIBUTE : int
        {
            DWMWA_NCRENDERING_ENABLED = 1,
            DWMWA_NCRENDERING_POLICY = 2,
            DWMWA_TRANSITIONS_FORCEDISABLED = 3,
            DWMWA_ALLOW_NCPAINT = 4,
            DWMWA_CAPTION_BUTTON_BOUNDS = 5,
            DWMWA_NONCLIENT_RTL_LAYOUT = 6,
            DWMWA_FORCE_ICONIC_REPRESENTATION = 7,
            DWMWA_FLIP3D_POLICY = 8,
            DWMWA_EXTENDED_FRAME_BOUNDS = 9,
            DWMWA_HAS_ICONIC_BITMAP = 10,
            DWMWA_DISALLOW_PEEK = 11,
            DWMWA_EXCLUDED_FROM_PEEK = 12,
            DWMWA_CLOAK = 13,
            DWMWA_CLOAKED = 14,
            DWMWA_FREEZE_REPRESENTATION = 15,
            DWMWA_LAST = 16
        }

        public enum DWM_CLOAKED_FLAGS : int
        {
            DWM_CLOAKED_NONE = 0x0000000,
            DWM_CLOAKED_APP = 0x0000001,
            DWM_CLOAKED_SHELL = 0x0000002,
            DWM_CLOAKED_INHERITED = 0x0000004
        }

        //Get Class Long
        [DllImport("user32.dll", EntryPoint = "GetClassLong")]
        private static extern uint GetClassLong32(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
        private static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);
        public static IntPtr GetClassLongAuto(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size > 4)
            {
                return GetClassLongPtr64(hWnd, nIndex);
            }
            else
            {
                return new IntPtr(GetClassLong32(hWnd, nIndex));
            }
        }

        //Get and Set Window Long
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern int GetWindowLong32(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);
        public static IntPtr GetWindowLongAuto(IntPtr hWnd, int nIndex)
        {
            try
            {
                if (IntPtr.Size > 4)
                {
                    return GetWindowLongPtr64(hWnd, nIndex);
                }
                else
                {
                    return new IntPtr(GetWindowLong32(hWnd, nIndex));
                }
            }
            catch
            {
                Debug.WriteLine("Failed to get window long.");
                return IntPtr.Zero;
            }
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        public static IntPtr SetWindowLongAuto(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            try
            {
                if (IntPtr.Size > 4)
                {
                    return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
                }
                else
                {
                    return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
                }
            }
            catch
            {
                Debug.WriteLine("Failed to set window long.");
                return IntPtr.Zero;
            }
        }

        public enum WindowStyles : uint
        {
            WS_NONE = 0x00000000,
            WS_BORDER = 0x800000,
            WS_CAPTION = 0xc00000,
            WS_CHILD = 0x40000000,
            WS_CLIPCHILDREN = 0x2000000,
            WS_CLIPSIBLINGS = 0x4000000,
            WS_DISABLED = 0x8000000,
            WS_DLGFRAME = 0x400000,
            WS_GROUP = 0x20000,
            WS_HSCROLL = 0x100000,
            WS_MAXIMIZE = 0x1000000,
            WS_MAXIMIZEBOX = 0x10000,
            WS_MINIMIZE = 0x20000000,
            WS_MINIMIZEBOX = 0x20000,
            WS_OVERLAPPED = 0x0,
            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_SIZEFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_POPUP = 0x80000000u,
            WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
            WS_SIZEFRAME = 0x40000,
            WS_SYSMENU = 0x80000,
            WS_TABSTOP = 0x10000,
            WS_VISIBLE = 0x10000000,
            WS_VSCROLL = 0x200000
        }

        public enum WindowStylesEx : uint
        {
            WS_EX_NONE = 0x00000000,
            WS_EX_ACCEPTFILES = 0x00000010,
            WS_EX_APPWINDOW = 0x00040000,
            WS_EX_CLIENTEDGE = 0x00000200,
            WS_EX_COMPOSITED = 0x02000000,
            WS_EX_CONTEXTHELP = 0x00000400,
            WS_EX_CONTROLPARENT = 0x00010000,
            WS_EX_DLGMODALFRAME = 0x00000001,
            WS_EX_LAYERED = 0x00080000,
            WS_EX_LAYOUTRTL = 0x00400000,
            WS_EX_LEFT = 0x00000000,
            WS_EX_LEFTSCROLLBAR = 0x00004000,
            WS_EX_LTRREADING = 0x00000000,
            WS_EX_MDICHILD = 0x00000040,
            WS_EX_NOACTIVATE = 0x08000000,
            WS_EX_NOINHERITLAYOUT = 0x00100000,
            WS_EX_NOPARENTNOTIFY = 0x00000004,
            WS_EX_NOREDIRECTIONBITMAP = 0x00200000,
            WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,
            WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,
            WS_EX_RIGHT = 0x00001000,
            WS_EX_RIGHTSCROLLBAR = 0x00000000,
            WS_EX_RTLREADING = 0x00002000,
            WS_EX_STATICEDGE = 0x00020000,
            WS_EX_TOOLWINDOW = 0x00000080,
            WS_EX_TOPMOST = 0x00000008,
            WS_EX_TRANSPARENT = 0x00000020,
            WS_EX_WINDOWEDGE = 0x00000100
        }
        public enum WindowLongFlags : int
        {
            GWL_EXSTYLE = -20,
            GWLP_HINSTANCE = -6,
            GWLP_HWNDPARENT = -8,
            GWL_ID = -12,
            GWL_STYLE = -16,
            GWL_USERDATA = -21,
            GWL_WNDPROC = -4,
            DWLP_USER = 0x8,
            DWLP_MSGRESULT = 0x0,
            DWLP_DLGPROC = 0x4
        }

        //Show Window
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, ShowWindowFlags nCmdShow);
        public enum ShowWindowFlags
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_FORCEMINIMIZE = 11,
            SW_MAX = 11
        }

        //Get Window Position
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out WindowRectangle rectangle);

        //Window and size structures
        public struct WindowRectangle
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
            public int Width { get { return this.Right - this.Left; } }
            public int Height { get { return this.Bottom - this.Top; } }
        }

        public struct WindowPoint
        {
            public int X;
            public int Y;
        }

        public struct WindowSize
        {
            public int cx;
            public int cy;
        }

        //Set Window Position
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        public enum SWP_WindowPosition
        {
            Top = 0,
            Bottom = 1,
            TopMost = -1,
            NoTopMost = -2
        }

        public enum WindowPosFlags : int
        {
            NOSIZE = 0x0001,
            NOMOVE = 0x0002,
            NOZORDER = 0x0004,
            NOREDRAW = 0x0008,
            NOACTIVATE = 0x0010,
            DRAWFRAME = 0x0020,
            FRAMECHANGED = 0x0020,
            SHOWWINDOW = 0x0040,
            HIDEWINDOW = 0x0080,
            NOCOPYBITS = 0x0100,
            NOOWNERZORDER = 0x0200,
            NOREPOSITION = 0x0200,
            NOSENDCHANGING = 0x0400,
            DEFERERASE = 0x2000,
            ASYNCWINDOWPOS = 0x4000
        }

        public enum WindowMessages : int
        {
            WM_MOUSEMOVE = 0x200,
            WM_LBUTTONDOWN = 0x201,
            WM_LBUTTONUP = 0x202,
            WM_LBUTTONDBLCLK = 0x203,
            WM_RBUTTONDOWN = 0x204,
            WM_RBUTTONUP = 0x205,
            WM_RBUTTONDBLCLK = 0x206,
            WM_MBUTTONDOWN = 0x207,
            WM_MBUTTONUP = 0x208,
            WM_MBUTTONDBLCLK = 0x209,
            WM_MOUSEWHEEL = 0x20A,
            WM_MOUSEHWHEEL = 0x20E,
            WM_MOUSEACTIVATE = 0x0021,
            WM_MOUSEHOVER = 0x02A1,
            WM_MOUSELEAVE = 0x02A3,
            WM_SETCURSOR = 0x0020,
            WM_KEYDOWN = 0x100,
            WM_KEYUP = 0x101,
            WM_SYSKEYDOWN = 0x104,
            WM_SYSKEYUP = 0x105,
            WM_SYSCOMMAND = 0x112,
            WM_DROPFILES = 0x233,
            WM_COPYDATA = 0x004A,
            WM_COPYGLOBALDATA = 0x49,
            WM_HOTKEY = 0x312,
            WM_GETICON = 0x7F,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14,
            WM_CLOSE = 0x0010,
            WM_CANCELMODE = 0x001F,
            WM_QUIT = 0x0012
        }

        public enum WindowReturnMessages : int
        {
            MA_ACTIVATE = 0x0001,
            MA_ACTIVATEANDEAT = 0x0002,
            MA_NOACTIVATE = 0x0003,
            MA_NOACTIVATEANDEAT = 0x0004
        }

        public enum SystemCommand
        {
            SC_SIZE = 0xF000,
            SC_MOVE = 0xF010,
            SC_MINIMIZE = 0xF020,
            SC_MAXIMIZE = 0xF030,
            SC_NEXTWINDOW = 0xF040,
            SC_PREVWINDOW = 0xF050,
            SC_CLOSE = 0xF060,
            SC_VSCROLL = 0xF070,
            SC_HSCROLL = 0xF080,
            SC_MOUSEMENU = 0xF090,
            SC_KEYMENU = 0xF100,
            SC_ARRANGE = 0xF110,
            SC_RESTORE = 0xF120,
            SC_TASKLIST = 0xF130,
            SC_SCREENSAVE = 0xF140,
            SC_HOTKEY = 0xF150,
            SC_DEFAULT = 0xF160,
            SC_MONITORPOWER = 0xF170,
            SC_CONTEXTHELP = 0xF180,
            SC_SEPARATOR = 0xF00F,
            SCF_ISSECURE = 0x00000001
        }

        //Send message with timeout
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageTimeout(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam, SendMessageTimeoutFlags flags, int timeout, out IntPtr pdwResult);

        public enum SendMessageTimeoutFlags : int
        {
            SMTO_ABORTIFHUNG = 0x0002,
            SMTO_BLOCK = 0x0001,
            SMTO_NORMAL = 0x0000,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x0008,
            SMTO_ERRORONEXIT = 0x0020
        }

        //Send message
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, WindowMessages Msg, int wParam, int lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, WindowMessages Msg, IntPtr wParam, IntPtr lParam);

        //Post message
        [DllImport("user32.dll", EntryPoint = "PostMessage", CharSet = CharSet.Auto)]
        public static extern bool PostMessageAsync(IntPtr hWnd, WindowMessages Msg, int wParam, int lParam);
        [DllImport("user32.dll", EntryPoint = "PostMessage", CharSet = CharSet.Auto)]
        public static extern bool PostMessageAsync(IntPtr hWnd, WindowMessages Msg, IntPtr wParam, IntPtr lParam);

        //Get message
        [DllImport("user32.dll")]
        public static extern bool GetMessage(out WindowMessage lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [StructLayout(LayoutKind.Sequential)]
        public struct WindowMessage
        {
            public IntPtr hwnd { get; set; }
            public IntPtr lParam { get; set; }
            public int message { get; set; }
            public int pt_x { get; set; }
            public int pt_y { get; set; }
            public int time { get; set; }
            public IntPtr wParam { get; set; }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }

        //Windows Hook
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(WindowHookTypes idHook, LowLevelKeyboardDelegate lpfn, IntPtr hMod, uint dwThreadId);
        public delegate IntPtr LowLevelKeyboardDelegate(int nCode, IntPtr wParam, KBDLLHOOKSTRUCT lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(WindowHookTypes idHook, LowLevelHookDelegate lpfn, IntPtr hMod, uint dwThreadId);
        public delegate IntPtr LowLevelHookDelegate(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, KBDLLHOOKSTRUCT lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [StructLayout(LayoutKind.Sequential)]
        public struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public KBDLLHOOKSTRUCTFLAGS flags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        public enum KBDLLHOOKSTRUCTFLAGS : int
        {
            LLKHF_EXTENDED = 0x00000001,
            LLKHF_LOWER_IL_INJECTED = 0x00000002,
            LLKHF_INJECTED = 0x00000010,
            LLKHF_ALTDOWN = 0x00000020,
            LLKHF_UP = 0x00000080
        }

        public enum WindowHookTypes : int
        {
            WH_MSGFILTER = -1,
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        //Window Placement
        [DllImport("user32.dll")]
        public static extern bool SetWindowPlacement(IntPtr hWnd, ref WindowPlacement lpwndpl);
        [DllImport("user32.dll")]
        public static extern bool GetWindowPlacement(IntPtr hWnd, out WindowPlacement lpwndpl);
        public struct WindowPlacement
        {
            public int length;
            public WindowFlags windowFlags;
            public WindowShowCommand windowShowCommand;
            public WindowPoint ptMinPosition;
            public WindowPoint ptMaxPosition;
            public WindowRectangle rcNormalPosition;
            public WindowRectangle rcDevice;
        }
        public enum WindowFlags : int
        {
            NoRequest = 0,
            SetMinPosition = 1,
            RestoreToMaximized = 2,
            AsyncWindowPlacement = 3
        }
        public enum WindowShowCommand : int
        {
            None = -1,
            Hide = 0,
            Normal = 1,
            Minimized = 2,
            Maximized = 3,
            ShowMaximized = 3,
            ShowNoActivate = 4,
            Show = 5,
            Minimize = 6,
            ShowMinNoActive = 7,
            ShowNA = 8,
            Restore = 9,
            ShowDefault = 10,
            ForceMinimize = 11
        }

        //AutomationElement
        [DllImport("UIAutomationCore.dll", EntryPoint = "UiaNodeFromHandle")]
        private static extern int RawUiaNodeFromHandle(IntPtr hWnd, out IntPtr hNode);
        [DllImport("UIAutomationCore.dll", EntryPoint = "UiaNodeRelease")]
        private static extern bool RawUiaNodeRelease(IntPtr hNode);
        [DllImport("UIAutomationCore.dll", EntryPoint = "UiaSetFocus")]
        private static extern int RawUiaSetFocus(IntPtr hNode);
        public static void UiaFocusWindowHandle(IntPtr windowHandle)
        {
            try
            {
                RawUiaNodeFromHandle(windowHandle, out IntPtr hNode);
                RawUiaSetFocus(hNode);
                RawUiaNodeRelease(hNode);
            }
            catch
            {
                Debug.WriteLine("Failed to uia focus on window handle: " + windowHandle);
            }
        }

        //SystemParametersInfo
        [DllImport("user32.dll")]
        public static extern bool SystemParametersInfo(SPI uiAction, uint uiParam, uint pvParam, SPIF fWinIni);
        [DllImport("user32.dll")]
        public static extern bool SystemParametersInfo(SPI uiAction, uint uiParam, string pvParam, SPIF fWinIni);
        public enum SPI : int
        {
            SPI_SETDESKWALLPAPER = 0x0014,
            SPI_GETDESKWALLPAPER = 0x0073,
            SPI_SETDESKPATTERN = 0x0015,
            SPI_GETFOREGROUNDLOCKTIMEOUT = 0x2000,
            SPI_SETFOREGROUNDLOCKTIMEOUT = 0x2001,
            SPI_GETFOREGROUNDFLASHCOUNT = 0x2004,
            SPI_SETFOREGROUNDFLASHCOUNT = 0x2005
        }
        public enum SPIF : int
        {
            None = 0x00,
            SPIF_UPDATEINIFILE = 0x01,
            SPIF_SENDCHANGE = 0x02
        }

        //Time events
        [DllImport("winmm.dll")]
        public static extern uint timeSetEvent(uint uDelayMs, uint uResolution, MultimediaTimerDelegate lpTimeProc, UIntPtr dwUser, TimeSetEventFlags fuEvent);
        public delegate void MultimediaTimerDelegate(uint uTimerId, uint uMsg, UIntPtr dwUser, UIntPtr dw1, UIntPtr dw2);

        [DllImport("winmm.dll")]
        public static extern uint timeKillEvent(uint uTimerId);

        public enum TimeSetEventFlags : uint
        {
            TIME_ONESHOT = 0,
            TIME_PERIODIC = 1
        }

        //Kernel events
        public const uint INFINITE = 0xFFFFFFFF;

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

        [DllImport("kernel32.dll")]
        public static extern bool SetEvent(IntPtr hEvent);

        [DllImport("kernel32.dll")]
        public static extern WaitObjectResult WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        public enum WaitObjectResult : uint
        {
            WAIT_OBJECT_0 = 0x00000000,
            WAIT_ABANDONED = 0x00000080,
            WAIT_TIMEOUT = 0x00000102,
            WAIT_FAILED_INFINITE = 0xFFFFFFFF
        }

        //High resolution delay (timer)
        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateWaitableTimerEx(IntPtr lpTimerAttributes, IntPtr lpTimerName, CreateWaitableTimerFlags dwFlags, CreateWaitableTimerAccess dwDesiredAccess);

        [DllImport("kernel32.dll")]
        public static extern bool SetWaitableTimerEx(IntPtr hTimer, ref long lpDueTime, int lPeriod, IntPtr pfnCompletionRoutine, IntPtr lpArgToCompletionRoutine, IntPtr wakeContext, uint tolerableDelay);

        public enum CreateWaitableTimerFlags : uint
        {
            TIMER_MANUAL_RESET = 0x00000001,
            TIMER_HIGH_RESOLUTION = 0x00000002
        }

        public enum CreateWaitableTimerAccess : uint
        {
            TIMER_ALL_ACCESS = 0x1F0003,
            TIMER_MODIFY_STATE = 0x0002,
            TIMER_QUERY_STATE = 0x0001
        }

        //High resolution delay (NT)
        [DllImport("ntdll.dll")]
        public static extern unsafe uint NtDelayExecution(bool alertable, ref long nanoSecondsDelay);

        [DllImport("ntdll.dll")]
        public static extern unsafe void NtSetTimerResolution(uint desiredResolution, bool adjustResolution, out uint currentResolution);

        //Prevent sleep or monitor timeout
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        public enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
        }

        //Process times
        [DllImport("kernel32.dll")]
        public static extern bool GetProcessTimes(IntPtr hProcess, out long lpCreationTime, out long lpExitTime, out long lpKernelTime, out long lpUserTime);

        //Process priority
        [DllImport("kernel32.dll")]
        public static extern bool SetPriorityClass(IntPtr hProcess, ProcessPriority dwPriorityClass);

        [DllImport("kernel32.dll")]
        public static extern ProcessPriority GetPriorityClass(IntPtr hProcess);

        public enum ProcessPriority : uint
        {
            Unknown = 0x00,
            Normal = 0x20,
            Idle = 0x40,
            High = 0x80,
            RealTime = 0x100,
            BelowNormal = 0x4000,
            AboveNormal = 0x8000
        }

        //Process not responding
        [DllImport("user32.dll")]
        public static extern bool IsHungAppWindow(IntPtr hWnd);

        //Lock computer
        [DllImport("user32.dll")]
        public static extern bool LockWorkStation();
    }
}