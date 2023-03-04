using System;
using System.Collections.Generic;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Enumerate all windows by process id (including uwp and fullscreen)
        public static List<IntPtr> Window_GetWindowHandlesByProcessId(int targetProcessId)
        {
            //AVDebug.WriteLine("Getting window handles by process id: " + processId);
            List<IntPtr> listWindows = new List<IntPtr>();
            try
            {
                IntPtr childWindow = IntPtr.Zero;
                while ((childWindow = FindWindowEx(IntPtr.Zero, childWindow, null, null)) != IntPtr.Zero)
                {
                    try
                    {
                        GetWindowThreadProcessId(childWindow, out int foundProcessId);
                        if (foundProcessId == targetProcessId)
                        {
                            listWindows.Add(childWindow);
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return listWindows;
        }

        //Enumerate all windows by thread id (including uwp and fullscreen)
        public static List<IntPtr> Window_GetWindowHandlesByThreadId(int targetThreadId)
        {
            //AVDebug.WriteLine("Getting window handles by thread id: " + threadId);
            List<IntPtr> listWindows = new List<IntPtr>();
            try
            {
                IntPtr childWindow = IntPtr.Zero;
                while ((childWindow = FindWindowEx(IntPtr.Zero, childWindow, null, null)) != IntPtr.Zero)
                {
                    try
                    {
                        int foundThreadId = GetWindowThreadProcessId(childWindow, out int foundProcessId);
                        if (foundThreadId == targetThreadId)
                        {
                            listWindows.Add(childWindow);
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return listWindows;
        }
    }
}