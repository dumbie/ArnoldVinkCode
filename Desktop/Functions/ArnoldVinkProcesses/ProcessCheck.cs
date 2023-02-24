using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Check if path is uwp application
        public static bool Check_PathUwpApplication(string targetPath)
        {
            try
            {
                return !targetPath.Contains("\\") && !targetPath.Contains("/") && targetPath.Contains("!") && targetPath.Contains("_");
            }
            catch { }
            return false;
        }

        //Check if path is url protocol
        public static bool Check_PathUrlProtocol(string targetPath)
        {
            try
            {
                bool dividerPosition = targetPath.IndexOf(":") > 1;
                bool urlProtocol = targetPath.Contains(":/") || targetPath.Contains(":\\");
                return urlProtocol && dividerPosition;
            }
            catch { }
            return false;
        }

        //Check if a specific process is running by id
        public static bool CheckRunningProcessById(int processId)
        {
            try
            {
                return Process.GetProcesses().Any(x => x.Id == processId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check process by id: " + ex.Message);
                return false;
            }
        }

        //Check if a specific process is running by window handle
        public static bool CheckRunningProcessByWindowHandle(IntPtr windowHandle)
        {
            try
            {
                return Process.GetProcesses().Any(x => x.MainWindowHandle == windowHandle);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check process by handle: " + ex.Message);
                return false;
            }
        }

        //Check if a specific process is running by name
        public static bool CheckRunningProcessByNameOrTitle(string processName, bool windowTitle, bool exactName)
        {
            try
            {
                if (windowTitle)
                {
                    return Process.GetProcesses().Any(x => x.MainWindowTitle.ToLower().Contains(processName.ToLower()));
                }
                else
                {
                    processName = Path.GetFileNameWithoutExtension(processName);
                    if (exactName)
                    {
                        return Process.GetProcessesByName(processName).Any();
                    }
                    else
                    {
                        return Process.GetProcesses().Where(x => x.ProcessName.ToLower().Contains(processName.ToLower())).Any();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check process by name: " + ex.Message);
                return false;
            }
        }

        //Check if process is in suspended state
        public static bool CheckProcessSuspended(ProcessThreadCollection threadCollection)
        {
            try
            {
                //Debug.WriteLine("Checking suspend state for process: " + targetProcess.ProcessName + "/" + targetProcess.Id);
                ProcessThread processThread = threadCollection[0];
                if (processThread.ThreadState == ThreadState.Wait && processThread.WaitReason == ThreadWaitReason.Suspended)
                {
                    //Debug.WriteLine("The process main thread is currently suspended.");
                    return true;
                }
            }
            catch { }
            return false;
        }

        //Check if process is active
        public static bool CheckValidProcess(Process targetProcess, bool checkSuspended, bool checkWin32)
        {
            try
            {
                //Check if the application is suspended
                if (checkSuspended)
                {
                    if (CheckProcessSuspended(targetProcess.Threads))
                    {
                        //Debug.WriteLine("Application is suspended and can't be shown or hidden.");
                        return false;
                    }
                }

                //Check if the application is win32
                if (checkWin32)
                {
                    if (CheckProcessIsUwp(targetProcess.MainWindowHandle))
                    {
                        //Debug.WriteLine("Application is an uwp application.");
                        return false;
                    }
                }

                return true;
            }
            catch { }
            return false;
        }

        //Check if window handle is a window
        public static bool CheckValidWindowHandle(IntPtr targetWindowHandle)
        {
            try
            {
                //Check if is a window
                if (!IsWindow(targetWindowHandle))
                {
                    //Debug.WriteLine("Window handle is not a Window.");
                    return false;
                }

                //Check if window is visible
                if (!IsWindowVisible(targetWindowHandle))
                {
                    //Debug.WriteLine("Window handle is not visible.");
                    return false;
                }

                //Check if application is hidden to the tray
                WindowPlacement ProcessWindowState = new WindowPlacement();
                GetWindowPlacement(targetWindowHandle, ref ProcessWindowState);
                if (ProcessWindowState.windowShowCommand <= 0)
                {
                    //Debug.WriteLine("Application is in the tray and can't be shown or hidden.");
                    return false;
                }

                return true;
            }
            catch { }
            return false;
        }
    }
}