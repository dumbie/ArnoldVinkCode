using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
        public static bool Check_RunningProcessByProcessId(int targetProcessId)
        {
            try
            {
                return Get_AllProcessesMulti().Any(x => x.Identifier == targetProcessId);
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to check process by id: " + ex.Message);
                return false;
            }
        }

        //Check if process is running by window handle
        public static bool Check_RunningProcessByWindowHandle(IntPtr targetWindowHandle)
        {
            try
            {
                return Detail_ProcessIdByWindowHandle(targetWindowHandle) > 0;
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to check process by window handle: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Check if process is running by name
        /// </summary>
        /// <param name="targetProcessName">Process name without extension</param>
        /// <param name="exactName">Search for exact process name</param>
        public static bool Check_RunningProcessByName(string targetProcessName, bool exactName)
        {
            try
            {
                return Get_ProcessesMultiByName(targetProcessName, exactName).Any();
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to check running process by name: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Check if process is running by AppUserModelId
        /// </summary>
        /// <param name="targetAppUserModelId">UWP or Win32Store AppUserModelId</param>
        public static bool Check_RunningProcessByAppUserModelId(string targetAppUserModelId)
        {
            try
            {
                return Get_ProcessesMultiByAppUserModelId(targetAppUserModelId).Any();
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to check running process by AppUserModelId: " + ex.Message);
                return false;
            }
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
                string[] classNamesUwp = { "ApplicationFrameWindow", "Windows.UI.Core.CoreWindow" };
                foreach (string className in classNamesUwp)
                {
                    if (targetClassName == className) { return true; }
                }
            }
            catch { }
            return false;
        }

        //Check if class name is valid window
        public static bool Check_ClassNameIsValid(string targetClassName)
        {
            try
            {
                string[] classNamesInvalid = { "Windows.Internal.Shell.TabProxyWindow" };
                foreach (string className in classNamesInvalid)
                {
                    if (targetClassName == className) { return false; }
                }
            }
            catch { }
            return true;
        }

        //Check if window handle is a window
        public static bool Check_WindowHandleValid(IntPtr targetWindowHandle)
        {
            try
            {
                //Check if handle is empty
                if (targetWindowHandle == IntPtr.Zero)
                {
                    //AVDebug.WriteLine("Window handle is empty.");
                    return false;
                }

                //Check if handle is a window
                if (!IsWindow(targetWindowHandle))
                {
                    //AVDebug.WriteLine("Window handle is not a window.");
                    return false;
                }

                //Check if window is visible
                if (!IsWindowVisible(targetWindowHandle))
                {
                    //AVDebug.WriteLine("Window handle is not visible.");
                    return false;
                }

                //Check if window is hidden to tray
                GetWindowPlacement(targetWindowHandle, out WindowPlacement windowPlacement);
                if (windowPlacement.windowShowCommand <= 0)
                {
                    //AVDebug.WriteLine("Window is in the tray and can't be shown or hidden.");
                    return false;
                }

                //Check window size
                GetWindowRect(targetWindowHandle, out WindowRectangle windowRectangle);
                if (windowRectangle.Width <= 0 || windowRectangle.Height <= 0)
                {
                    //AVDebug.WriteLine("Window has no size and can't be shown or hidden.");
                    return false;
                }

                //Check window styles
                WindowStyles windowStyle = (WindowStyles)GetWindowLongAuto(targetWindowHandle, (int)WindowLongFlags.GWL_STYLE).ToInt64();
                if (windowStyle.HasFlag(WindowStyles.WS_DISABLED))
                {
                    //AVDebug.WriteLine("Window has disabled style and can't be shown or hidden.");
                    return false;
                }

                //Check window styles ex
                WindowStylesEx windowStyleEx = (WindowStylesEx)GetWindowLongAuto(targetWindowHandle, (int)WindowLongFlags.GWL_EXSTYLE).ToInt64();
                if (windowStyleEx.HasFlag(WindowStylesEx.WS_EX_TOOLWINDOW))
                {
                    //AVDebug.WriteLine("Window has tool style and can't be shown or hidden.");
                    return false;
                }

                //Validate the window class name
                string windowClassName = Detail_ClassNameByWindowHandle(targetWindowHandle);
                if (!Check_ClassNameIsValid(windowClassName))
                {
                    Debug.WriteLine("Window class name is not valid and can't be shown or hidden.");
                    return false;
                }

                //Check if window is cloaked
                int dwmSuccess = DwmGetWindowAttribute(targetWindowHandle, DWMWINDOWATTRIBUTE.DWMWA_CLOAKED, out bool windowCloaked, Marshal.SizeOf(typeof(bool)));
                if (dwmSuccess == 0 && windowCloaked)
                {
                    //AVDebug.WriteLine("Window is cloaked and can't be shown or hidden.");
                    return false;
                }

                return true;
            }
            catch { }
            return false;
        }
    }
}