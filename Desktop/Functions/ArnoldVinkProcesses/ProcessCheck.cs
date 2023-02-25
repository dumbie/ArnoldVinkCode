using System;
using System.Diagnostics;
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

        //Check if process is running by process id
        public static bool Check_RunningProcessById(int targetProcessId)
        {
            try
            {
                return Process.GetProcesses().Any(x => x.Id == targetProcessId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check process by id: " + ex.Message);
                return false;
            }
        }

        //Check if process is running by window handle
        public static bool Check_RunningProcessByWindowHandle(IntPtr targetWindowHandle)
        {
            try
            {
                return Process.GetProcesses().Any(x => x.MainWindowHandle == targetWindowHandle);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check process by handle: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Check if process is running by process name
        /// </summary>
        /// <param name="targetProcessName">Process name without extension</param>
        /// <param name="exactName">Search for exact process name</param>
        public static bool Check_RunningProcessByName(string targetProcessName, bool exactName)
        {
            try
            {
                if (exactName)
                {
                    return Process.GetProcesses().Any(x => x.ProcessName.ToLower() == targetProcessName.ToLower());
                }
                else
                {
                    return Process.GetProcesses().Any(x => x.ProcessName.ToLower().Contains(targetProcessName.ToLower()));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check running process by name: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Check if process is running by window title
        /// </summary>
        /// <param name="targetWindowTitle">Search for window title</param>
        /// <param name="exactName">Search for exact window title</param>
        public static bool Check_RunningProcessByWindowTitle(string targetWindowTitle, bool exactName)
        {
            try
            {
                if (exactName)
                {
                    return Process.GetProcesses().Any(x => x.MainWindowTitle.ToLower() == targetWindowTitle.ToLower());
                }
                else
                {
                    return Process.GetProcesses().Any(x => x.MainWindowTitle.ToLower().Contains(targetWindowTitle.ToLower()));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check running process by window title: " + ex.Message);
                return false;
            }
        }

        //Check if process is suspended
        public static bool Check_ProcessSuspended(ProcessThreadCollection targetThreadCollection)
        {
            try
            {
                //Debug.WriteLine("Checking suspend state for process: " + targetProcess.ProcessName + "/" + targetProcess.Id);
                ProcessThread processThread = targetThreadCollection[0];
                if (processThread.ThreadState == ThreadState.Wait && processThread.WaitReason == ThreadWaitReason.Suspended)
                {
                    //Debug.WriteLine("The process main thread is currently suspended.");
                    return true;
                }
            }
            catch { }
            return false;
        }

        //Check if window handle is from uwp application
        public static bool Check_WindowHandleIsUwpApp(IntPtr targetWindowHandle)
        {
            try
            {
                string classNamestring = Detail_ClassNameByWindowHandle(targetWindowHandle);
                return Check_ClassNameIsUwpApp(classNamestring);
            }
            catch { }
            return false;
        }

        //Check if class name is from uwp application
        public static bool Check_ClassNameIsUwpApp(string targetClassName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(targetClassName) || targetClassName == "ApplicationFrameWindow" || targetClassName == "Windows.UI.Core.CoreWindow")
                {
                    return true;
                }
            }
            catch { }
            return false;
        }

        //Check if window handle is a window
        public static bool Check_ValidWindowHandle(IntPtr targetWindowHandle)
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