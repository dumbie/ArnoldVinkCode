using System;
using System.Collections.Generic;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Enumerate all windows by process id (including uwp and fullscreen)
        public static List<IntPtr> Get_WindowHandlesByProcessId(int targetProcessId)
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
        public static List<IntPtr> Get_WindowHandlesByThreadId(int targetThreadId)
        {
            //AVDebug.WriteLine("Getting window handles by thread id: " + targetThreadId);
            List<IntPtr> listWindows = new List<IntPtr>();
            try
            {
                IntPtr childWindow = IntPtr.Zero;
                while ((childWindow = FindWindowEx(IntPtr.Zero, childWindow, null, null)) != IntPtr.Zero)
                {
                    try
                    {
                        if (GetWindowThreadProcessId(childWindow, out int foundProcessId) == targetThreadId)
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

        //Enumerate all windows by appusermodelid (including uwp and fullscreen)
        public static List<IntPtr> Get_WindowHandlesByAppUserModelId(string targetAppUserModelId)
        {
            //AVDebug.WriteLine("Getting window handles by appusermodelid: " + targetAppUserModelId);
            List<IntPtr> listWindows = new List<IntPtr>();
            try
            {
                IntPtr childWindow = IntPtr.Zero;
                while ((childWindow = FindWindowEx(IntPtr.Zero, childWindow, null, null)) != IntPtr.Zero)
                {
                    try
                    {
                        string targetAppUserModelIdLower = targetAppUserModelId.ToLower();
                        string foundAppUserModelIdLower = Detail_AppUserModelIdByWindowHandle(childWindow).ToLower();
                        if (targetAppUserModelIdLower == foundAppUserModelIdLower)
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