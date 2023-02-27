using System;
using System.Collections.Generic;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Enumerate all thread windows including fullscreen
        public static List<IntPtr> Thread_GetWindowHandles(int threadId)
        {
            AVDebug.WriteLine("Getting thread window handles: " + threadId);
            List<IntPtr> listWindows = new List<IntPtr>();
            try
            {
                IntPtr childWindow = IntPtr.Zero;
                while ((childWindow = FindWindowEx(IntPtr.Zero, childWindow, null, null)) != IntPtr.Zero)
                {
                    try
                    {
                        int foundProcessId = 0;
                        int foundThreadId = GetWindowThreadProcessId(childWindow, out foundProcessId);
                        if (foundThreadId == threadId)
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