using System;
using System.Diagnostics;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVDisplayMonitor
    {
        public static bool MonitorStateSetSleep()
        {
            try
            {
                int MONITOR_STATE = 2;
                int SC_MONITORPOWER = 0xF170;
                IntPtr HWND_BROADCAST = new IntPtr(0xFFFF);
                IntPtr result = SendMessage(HWND_BROADCAST, WindowMessages.WM_SYSCOMMAND, SC_MONITORPOWER, MONITOR_STATE);
                Debug.WriteLine("Set display monitor state to sleep: " + result);
            }
            catch { }
            return false;
        }
    }
}