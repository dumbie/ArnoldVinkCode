using System;
using System.Security;

namespace ArnoldVinkCode
{
    [SuppressUnmanagedCodeSecurity]
    public partial class AVInteropDll
    {
        public delegate IntPtr WindowProcedureDelegate(IntPtr hWnd, IntPtr messageCode, IntPtr wParam, IntPtr lParam);
        public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hWnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
        public delegate bool EnumWindowsDelegate(IntPtr hWnd, IntPtr lParam);
        public delegate IntPtr LowLevelKeyboardDelegate(int nCode, IntPtr wParam, KBDLLHOOKSTRUCT lParam);
        public delegate IntPtr LowLevelHookDelegate(int nCode, IntPtr wParam, IntPtr lParam);
        public delegate void TimeSetEventDelegate(uint uTimerID, uint uMsg, UIntPtr dwUser, UIntPtr dw1, UIntPtr dw2);
        public delegate void WaitableTimerDelegate(IntPtr lpArgToCompletionRoutine, uint dwTimerLowValue, uint dwTimerHighValue);
    }
}