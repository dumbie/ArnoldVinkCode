using System;
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

        //Check process is foreground uwp application
        public static bool Check_ProcessIsForegroundUwpApp(int targetProcessId)
        {
            try
            {
                foreach (IntPtr windowHandle in Get_WindowHandlesByProcessId(targetProcessId))
                {
                    string classNameString = Detail_ClassNameByWindowHandle(windowHandle);
                    if (classNameString == "MSCTFIME UI")
                    {
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }

        //Check if window class name is from uwp application
        public static bool Check_WindowClassNameIsUwpApp(IntPtr targetWindowHandle)
        {
            try
            {
                string classNameString = Detail_ClassNameByWindowHandle(targetWindowHandle);
                return Check_WindowClassNameIsUwpApp(classNameString);
            }
            catch { }
            return false;
        }

        //Check if window class name is from uwp application
        public static bool Check_WindowClassNameIsUwpApp(string targetClassName)
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

        //Check if window class name is valid
        public static bool Check_WindowClassNameIsValid(IntPtr targetWindowHandle)
        {
            try
            {
                string classNameString = Detail_ClassNameByWindowHandle(targetWindowHandle);
                return Check_WindowClassNameIsValid(classNameString);
            }
            catch { }
            return false;
        }

        //Check if window class name is valid
        public static bool Check_WindowClassNameIsValid(string targetClassName)
        {
            try
            {
                string[] classNamesInvalid = { "ApplicationFrameWindow", "ApplicationManager_ImmersiveShellWindow", "Windows.Internal.Shell.TabProxyWindow" };
                foreach (string className in classNamesInvalid)
                {
                    if (targetClassName == className) { return false; }
                }
            }
            catch { }
            return true;
        }

        //Check if window handle is a valid window
        public static bool Check_WindowHandleValid(IntPtr targetWindowHandle)
        {
            try
            {
                //Check if handle is empty
                if (targetWindowHandle == IntPtr.Zero)
                {
                    //Debug.WriteLine("Window handle is empty.");
                    return false;
                }

                //Check window styles
                WindowStyles windowStyle = (WindowStyles)GetWindowLongAuto(targetWindowHandle, (int)WindowLongFlags.GWL_STYLE).ToInt64();
                if (!windowStyle.HasFlag(WindowStyles.WS_VISIBLE))
                {
                    //Debug.WriteLine("Window missing visible style and can't be shown or hidden: " + targetWindowHandle);
                    return false;
                }
                if (windowStyle.HasFlag(WindowStyles.WS_DISABLED))
                {
                    //Debug.WriteLine("Window has disabled style and can't be shown or hidden: " + targetWindowHandle);
                    return false;
                }

                //Check window styles ex
                WindowStylesEx windowStyleEx = (WindowStylesEx)GetWindowLongAuto(targetWindowHandle, (int)WindowLongFlags.GWL_EXSTYLE).ToInt64();
                if (windowStyleEx.HasFlag(WindowStylesEx.WS_EX_TOOLWINDOW))
                {
                    //Debug.WriteLine("Window has tool style and can't be shown or hidden: " + targetWindowHandle);
                    return false;
                }

                //Check window class name
                if (!Check_WindowClassNameIsValid(targetWindowHandle))
                {
                    //Debug.WriteLine("Window class name is invalid: " + targetWindowHandle);
                    return false;
                }

                //Check window title length
                if (GetWindowTextLength(targetWindowHandle) <= 0)
                {
                    //Debug.WriteLine("Window has no title and can't be shown or hidden: " + targetWindowHandle);
                    return false;
                }
            }
            catch { }
            return true;
        }
    }
}