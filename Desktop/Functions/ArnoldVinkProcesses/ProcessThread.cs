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
            IntPtr childWindow = IntPtr.Zero;
            List<IntPtr> listWindows = new List<IntPtr>();
            try
            {
                while ((childWindow = FindWindowEx(IntPtr.Zero, childWindow, null, null)) != IntPtr.Zero)
                {
                    try
                    {
                        if (GetWindowThreadProcessId(childWindow, out int processId) == threadId)
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