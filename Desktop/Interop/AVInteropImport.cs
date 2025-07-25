using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using static ArnoldVinkCode.AVInputOutputClass;

namespace ArnoldVinkCode
{
    [SuppressUnmanagedCodeSecurity]
    public partial class AVInteropDll
    {
        [DllImport("kernel32.dll")]
        public static extern bool Wow64DisableWow64FsRedirection(IntPtr ptr);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetClassName(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpClassName, int nMaxCount);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetWindowText(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder caption, int count);
        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(IntPtr hWnd);
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
        [DllImport("kernel32.dll")]
        public static extern bool IsWow64Process(IntPtr processHandle, out bool wow64Process);

        //Close Handle
        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hHandle);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr hIcon);

        //Window get
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowFlags flags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetLastActivePopup(IntPtr hWnd);

        //Window show
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

        //Window create
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateWindowEx(WindowStylesEx dwExStyle, [MarshalAs(UnmanagedType.LPWStr)] string lpClassName, [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName, WindowStyles dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr RegisterClassEx(ref WindowClassEx lpwcx);
        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndParent);
        [DllImport("user32.dll")]
        public static extern IntPtr GetParent(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool UpdateWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool DestroyWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, LayeredWindowAttributes dwFlags);

        //Window
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, ShowWindowFlags nCmdShow);
        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, ShowWindowFlags nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out WindowRectangle rectangle);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, SetWindowPosOrder hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr DefWindowProcW(IntPtr hWnd, IntPtr uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr CallWindowProcW(IntPtr lpPrevWndFunc, IntPtr hWnd, IntPtr uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out WindowRectangle lpRect);

        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        //Window event hook
        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(WinEventHooks eventMin, WinEventHooks eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, WinEventFlags dwFlags);

        //Window redraw
        [DllImport("user32.dll")]
        public static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, RedrawWindowFlags flags);

        //Window display affinity
        [DllImport("user32.dll")]
        public static extern bool SetWindowDisplayAffinity(IntPtr hWnd, DisplayAffinityFlags affinity);

        //UWP interop
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetApplicationUserModelId(IntPtr hProcess, ref int appUserModelIdLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder sbAppUserModelID);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetPackageFullName(IntPtr hProcess, ref int packageFullNameLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder sbPackageFullName);

        //Load library
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, LoadLibraryFlags dwFlags);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr hModule);

        //Open and close process
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(PROCESS_DESIRED_ACCESS processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll")]
        public static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        //Open and close thread
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenThread(THREAD_DESIRED_ACCESS dwDesiredAccess, bool bInheritHandle, int dwThreadId);

        [DllImport("kernel32.dll")]
        public static extern int GetProcessIdOfThread(IntPtr tHandle);

        //Enumerate windows
        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsDelegate lpEnumFunc, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowsDelegate lpEnumFunc, IntPtr lParam);

        //Get process details
        [DllImport("psapi.dll", CharSet = CharSet.Unicode)]
        public static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, [In, MarshalAs(UnmanagedType.U4)] int nSize);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool QueryFullProcessImageName([In] IntPtr hProcess, [In] int dwFlags, [Out] StringBuilder lpExeName, ref int lpdwSize);

        //Get window ancestor
        [DllImport("user32.dll")]
        public static extern IntPtr GetAncestor(IntPtr hWnd, GetAncestorFlags flags);

        //Get window dwm attribute
        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, DWM_WINDOW_ATTRIBUTE dwAttribute, out WindowRectangle pvAttribute, int cbAttributeSize);
        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, DWM_WINDOW_ATTRIBUTE dwAttribute, out DWM_CLOAKED_FLAGS pvAttribute, int cbAttributeSize);

        //Get Class Long
        [DllImport("user32.dll", EntryPoint = "GetClassLong")]
        private static extern IntPtr GetClassLong32(IntPtr hWnd, ClassLong nIndex);
        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
        private static extern IntPtr GetClassLongPtr64(IntPtr hWnd, ClassLong nIndex);
        public static IntPtr GetClassLongAuto(IntPtr hWnd, ClassLong nIndex)
        {
            if (IntPtr.Size > 4)
            {
                return GetClassLongPtr64(hWnd, nIndex);
            }
            else
            {
                return GetClassLong32(hWnd, nIndex);
            }
        }

        //Get and Set Window Long
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern IntPtr GetWindowLong32(IntPtr hWnd, WindowLongFlags nIndex);
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, WindowLongFlags nIndex);
        public static IntPtr GetWindowLongAuto(IntPtr hWnd, WindowLongFlags nIndex)
        {
            try
            {
                if (IntPtr.Size > 4)
                {
                    return GetWindowLongPtr64(hWnd, nIndex);
                }
                else
                {
                    return GetWindowLong32(hWnd, nIndex);
                }
            }
            catch
            {
                Debug.WriteLine("Failed to get window long.");
                return IntPtr.Zero;
            }
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern IntPtr SetWindowLong32(IntPtr hWnd, WindowLongFlags nIndex, IntPtr dwNewLong);
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, WindowLongFlags nIndex, IntPtr dwNewLong);
        public static IntPtr SetWindowLongAuto(IntPtr hWnd, WindowLongFlags nIndex, IntPtr dwNewLong)
        {
            try
            {
                if (IntPtr.Size > 4)
                {
                    return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
                }
                else
                {
                    return SetWindowLong32(hWnd, nIndex, dwNewLong);
                }
            }
            catch
            {
                Debug.WriteLine("Failed to set window long.");
                return IntPtr.Zero;
            }
        }

        //Window Placement
        [DllImport("user32.dll")]
        public static extern bool SetWindowPlacement(IntPtr hWnd, ref WindowPlacement lpwndpl);

        [DllImport("user32.dll")]
        public static extern bool GetWindowPlacement(IntPtr hWnd, out WindowPlacement lpwndpl);

        //Register Hotkeys
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, KeysModifierInput keysModifier, KeysVirtual keysVirtual);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        //Send message
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, WindowMessages Msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, WindowMessages Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageTimeout(IntPtr hWnd, WindowMessages Msg, IntPtr wParam, IntPtr lParam, SendMessageTimeoutFlags flags, int timeout, out IntPtr pdwResult);

        //Post message
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hWnd, WindowMessages Msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hWnd, WindowMessages Msg, IntPtr wParam, IntPtr lParam);

        //Get message
        [DllImport("user32.dll")]
        public static extern bool GetMessage(out WindowMessage lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("user32.dll")]
        public static extern bool GetMessageW(out WindowMessage lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("user32.dll")]
        public static extern bool TranslateMessage(ref WindowMessage lpMsg);

        [DllImport("user32.dll")]
        public static extern IntPtr DispatchMessageW(ref WindowMessage lpMsg);

        [DllImport("user32.dll")]
        public static extern void PostQuitMessage(int nExitCode);

        [DllImport("user32.dll")]
        public static extern bool ChangeWindowMessageFilterEx(IntPtr hWnd, WindowMessages message, ChangeWindowMessageFilterAction action, IntPtr pChangeFilterStruct);

        [DllImport("shell32.dll")]
        public static extern void DragAcceptFiles(IntPtr hWnd, bool fAccept);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern int DragQueryFile(IntPtr hDrop, uint iFile, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpszFile, int cch);

        [DllImport("shell32.dll")]
        public static extern void DragFinish(IntPtr hDrop);

        //Windows Hook
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(WindowHookTypes idHook, LowLevelKeyboardDelegate lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(WindowHookTypes idHook, LowLevelHookDelegate lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, KBDLLHOOKSTRUCT lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr hInstance);

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
        public static extern bool SystemParametersInfo(SystemParametersInfoAction uiAction, uint uiParam, uint pvParam, SystemParametersInfoWinIni fWinIni);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool SystemParametersInfo(SystemParametersInfoAction uiAction, uint uiParam, string pvParam, SystemParametersInfoWinIni fWinIni);

        //Set timer
        [DllImport("winmm.dll")]
        public static extern uint timeSetEvent(uint uDelay, uint uResolution, TimeSetEventDelegate lpTimeProc, UIntPtr dwUser, TimerEventFlags fuEvent);

        [DllImport("winmm.dll")]
        public static extern uint timeKillEvent(uint uTimerID);

        //Waitable timer
        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateWaitableTimerEx(IntPtr lpTimerAttributes, IntPtr lpTimerName, CreateWaitableTimerFlags dwFlags, CreateWaitableTimerAccess dwDesiredAccess);

        [DllImport("kernel32.dll")]
        public static extern bool SetWaitableTimerEx(IntPtr hTimer, ref long lpDueTime, long lPeriod, WaitableTimerDelegate pfnCompletionRoutine, IntPtr lpArgToCompletionRoutine, IntPtr wakeContext, ulong tolerableDelay);

        [DllImport("kernel32.dll")]
        public static extern bool CancelWaitableTimer(IntPtr hTimer);

        //Kernel events
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

        [DllImport("kernel32.dll")]
        public static extern bool SetEvent(IntPtr hEvent);

        [DllImport("kernel32.dll")]
        public static extern WaitForSingleObjectResult WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        //High resolution delay (NT)
        [DllImport("ntdll.dll")]
        public static extern unsafe uint NtDelayExecution(bool alertable, ref long nanoSecondsDelay);

        [DllImport("ntdll.dll")]
        public static extern unsafe void NtSetTimerResolution(uint desiredResolution, bool adjustResolution, out uint currentResolution);

        //Prevent sleep or monitor timeout
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        //Process times
        [DllImport("kernel32.dll")]
        public static extern bool GetProcessTimes(IntPtr hProcess, out long lpCreationTime, out long lpExitTime, out long lpKernelTime, out long lpUserTime);

        //Process priority
        [DllImport("kernel32.dll")]
        public static extern bool SetPriorityClass(IntPtr hProcess, ProcessPriorityClasses dwPriorityClass);

        [DllImport("kernel32.dll")]
        public static extern ProcessPriorityClasses GetPriorityClass(IntPtr hProcess);

        //Process not responding
        [DllImport("user32.dll")]
        public static extern bool IsHungAppWindow(IntPtr hWnd);

        //Lock computer
        [DllImport("user32.dll")]
        public static extern bool LockWorkStation();
    }
}