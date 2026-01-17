using System;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Get main window handle by process id
        public static IntPtr Get_WindowHandleMainByProcessId(int targetProcessId, bool checkVisibility)
        {
            try
            {
                foreach (IntPtr windowHandle in Get_WindowHandlesByProcessId(targetProcessId))
                {
                    try
                    {
                        if (Check_WindowHandleValid(windowHandle, true, checkVisibility))
                        {
                            return windowHandle;
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return IntPtr.Zero;
        }

        //Get main window handle by thread id
        public static IntPtr Get_WindowHandleMainByThreadId(int targetThreadId, bool checkVisibility)
        {
            try
            {
                foreach (IntPtr windowHandle in Get_WindowHandlesByThreadId(targetThreadId))
                {
                    try
                    {
                        if (Check_WindowHandleValid(windowHandle, true, checkVisibility))
                        {
                            return windowHandle;
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return IntPtr.Zero;
        }

        //Get main window handle by AppUserModelId
        public static IntPtr Get_WindowHandleMainByAppUserModelId(string targetAppUserModelId, bool checkVisibility)
        {
            try
            {
                foreach (IntPtr windowHandle in Get_WindowHandlesByAppUserModelId(targetAppUserModelId))
                {
                    try
                    {
                        if (Check_WindowHandleValid(windowHandle, true, checkVisibility))
                        {
                            return windowHandle;
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Get process handle by process identifier
        /// </summary>
        /// <param name="targetProcessId">Process identifier</param>
        public static IntPtr Get_ProcessHandleByProcessId(int targetProcessId)
        {
            try
            {
                IntPtr hProcess = OpenProcess(PROCESS_DESIRED_ACCESS.PROCESS_MAXIMUM_ALLOWED, false, targetProcessId);
                if (hProcess == IntPtr.Zero)
                {
                    //AVDebug.WriteLine("Failed opening process id: " + targetProcessId + "/" + Marshal.GetLastWin32Error());
                    return IntPtr.Zero;
                }
                else
                {
                    //AVDebug.WriteLine("Opened process id: " + targetProcessId + "/" + Marshal.GetLastWin32Error());
                    return hProcess;
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get process handle by id: " + targetProcessId + "/" + ex.Message);
                return IntPtr.Zero;
            }
        }
    }
}