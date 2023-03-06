using System;
using System.Collections.Generic;
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
        public static bool Check_RunningProcessById(int targetProcessId)
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

        //Check if process is suspended by thread ids
        public static bool Check_ProcessSuspendedByThreadIds(List<int> targetThreadIds)
        {
            IntPtr threadHandle = IntPtr.Zero;
            try
            {
                int firstThreadId = targetThreadIds.FirstOrDefault();
                AVDebug.WriteLine("Checking suspend state for thread id: " + firstThreadId);

                //Open thread for reading
                threadHandle = OpenThread(THREAD_DESIRED_ACCESS.THREAD_MAXIMUM_ALLOWED, false, firstThreadId);
                if (threadHandle == IntPtr.Zero)
                {
                    AVDebug.WriteLine("Failed to open thread id: " + firstThreadId);
                    return false;
                }

                //Query thread information
                //Fix Add 32, 64 and WOW64 support
                SYSTEM_THREAD_INFORMATION32 systemThreadInformation = new SYSTEM_THREAD_INFORMATION32();
                int readResult = NtQueryInformationThread32(threadHandle, THREAD_INFO_CLASS.ThreadSystemThreadInformation, ref systemThreadInformation, (uint)Marshal.SizeOf(systemThreadInformation), out _);
                if (readResult != 0)
                {
                    AVDebug.WriteLine("Failed to get thread information: " + firstThreadId + "/Query failed.");
                    return false;
                }

                //Check thread state and wait reason
                AVDebug.WriteLine("Thread status: " + firstThreadId + "/" + systemThreadInformation.ThreadState + "/" + systemThreadInformation.ThreadWaitReason);
                return systemThreadInformation.ThreadState == ProcessThreadState.Waiting && systemThreadInformation.ThreadWaitReason == ProcessThreadWaitReason.Suspended;
            }
            catch { }
            finally
            {
                CloseHandleAuto(threadHandle);
            }
            return false;
        }

        //Check if window handle is from uwp application
        public static bool Check_WindowHandleIsUwpApp(IntPtr targetWindowHandle)
        {
            try
            {
                string appUserModelIdString = Detail_AppUserModelIdByWindowHandle(targetWindowHandle);
                string classNamestring = Detail_ClassNameByWindowHandle(targetWindowHandle);
                return !string.IsNullOrWhiteSpace(appUserModelIdString) && Check_ClassNameIsUwpApp(classNamestring);
            }
            catch { }
            return false;
        }

        //Check if class name is from uwp application
        public static bool Check_ClassNameIsUwpApp(string targetClassName)
        {
            try
            {
                return string.IsNullOrWhiteSpace(targetClassName) || targetClassName == "ApplicationFrameWindow" || targetClassName == "Windows.UI.Core.CoreWindow";
            }
            catch { }
            return false;
        }

        //Check if window handle is a window
        public static bool Check_ValidWindowHandle(IntPtr targetWindowHandle)
        {
            try
            {
                //Check if handle is empty
                if (targetWindowHandle == IntPtr.Zero)
                {
                    //AVDebug.WriteLine("Window handle is empty.");
                    return false;
                }

                //Check if is a window
                if (!IsWindow(targetWindowHandle))
                {
                    //AVDebug.WriteLine("Window handle is not a Window.");
                    return false;
                }

                //Check if window is visible
                if (!IsWindowVisible(targetWindowHandle))
                {
                    //AVDebug.WriteLine("Window handle is not visible.");
                    return false;
                }

                //Check if application is hidden to tray
                WindowPlacement ProcessWindowState = new WindowPlacement();
                GetWindowPlacement(targetWindowHandle, ref ProcessWindowState);
                if (ProcessWindowState.windowShowCommand <= 0)
                {
                    //AVDebug.WriteLine("Application is in the tray and can't be shown or hidden.");
                    return false;
                }

                return true;
            }
            catch { }
            return false;
        }
    }
}