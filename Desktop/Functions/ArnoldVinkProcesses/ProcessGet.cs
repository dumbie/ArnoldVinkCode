using System;
using System.Collections.Generic;
using System.Linq;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
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

        //Get multi process for current process
        public static ProcessMulti Get_ProcessMultiCurrent()
        {
            try
            {
                return Get_ProcessMultiByProcessId(GetCurrentProcessId());
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get multi process for current process: " + ex.Message);
                return null;
            }
        }

        //Get multi process by window handle
        public static ProcessMulti Get_ProcessMultiByWindowHandle(IntPtr targetWindowHandle)
        {
            try
            {
                //Check if window handle is UWP or Win32Store application
                string appUserModelId = Detail_AppUserModelIdByWindowHandle(targetWindowHandle);
                if (!string.IsNullOrWhiteSpace(appUserModelId))
                {
                    return Get_ProcessesMultiByAppUserModelId(appUserModelId).FirstOrDefault();
                }
                else
                {
                    int processId = Detail_ProcessIdByWindowHandle(targetWindowHandle);
                    return Get_ProcessMultiByProcessId(processId);
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get multi process by window handle: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Get processes multi by name
        /// </summary>
        /// <param name="targetName">Process name or executable name</param>
        /// <param name="exactName">Search for exact process name</param>
        public static List<ProcessMulti> Get_ProcessesMultiByName(string targetName, bool exactName)
        {
            //AVDebug.WriteLine("Getting processes by name: " + targetName);
            List<ProcessMulti> foundProcesses = new List<ProcessMulti>();
            try
            {
                string targetNameLower = targetName.ToLower();
                foreach (ProcessMulti checkProcess in Get_AllProcessesMulti())
                {
                    try
                    {
                        string foundExecutableNameLower = checkProcess.ExeName.ToLower();
                        string foundExecutableNameNoExtLower = checkProcess.ExeNameNoExt.ToLower();
                        if (exactName)
                        {
                            if (foundExecutableNameLower == targetNameLower || foundExecutableNameNoExtLower == targetNameLower)
                            {
                                foundProcesses.Add(checkProcess);
                            }
                        }
                        else
                        {
                            if (foundExecutableNameLower.Contains(targetNameLower) || foundExecutableNameNoExtLower.Contains(targetNameLower))
                            {
                                foundProcesses.Add(checkProcess);
                            }
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get multi processes by name: " + ex.Message);
            }
            return foundProcesses;
        }

        /// <summary>
        /// Get processes multi by executable path
        /// </summary>
        /// <param name="targetExecutablePath">Process executable path</param>
        public static List<ProcessMulti> Get_ProcessesMultiByExecutablePath(string targetExecutablePath)
        {
            //AVDebug.WriteLine("Getting processes by executable path: " + targetExecutablePath);
            List<ProcessMulti> foundProcesses = new List<ProcessMulti>();
            try
            {
                string targetExecutablePathLower = targetExecutablePath.ToLower();
                foreach (ProcessMulti checkProcess in Get_AllProcessesMulti())
                {
                    try
                    {
                        string foundExecutablePathLower = checkProcess.ExePath.ToLower();
                        if (foundExecutablePathLower == targetExecutablePathLower)
                        {
                            foundProcesses.Add(checkProcess);
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get multi processes by executable path: " + ex.Message);
            }
            return foundProcesses;
        }

        /// <summary>
        /// Get processes multi by AppUserModelId
        /// </summary>
        /// <param name="targetAppUserModelId">UWP or Win32Store AppUserModelId</param>
        public static List<ProcessMulti> Get_ProcessesMultiByAppUserModelId(string targetAppUserModelId)
        {
            //AVDebug.WriteLine("Getting processes by AppUserModelId: " + targetName);
            List<ProcessMulti> foundProcesses = new List<ProcessMulti>();
            try
            {
                string targetAppUserModelIdLower = targetAppUserModelId.ToLower();
                foreach (ProcessMulti checkProcess in Get_AllProcessesMulti())
                {
                    try
                    {
                        string foundAppUserModelIdLower = checkProcess.AppUserModelId.ToLower();
                        if (foundAppUserModelIdLower == targetAppUserModelIdLower)
                        {
                            foundProcesses.Add(checkProcess);
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get multi processes by AppUserModelId: " + ex.Message);
            }
            return foundProcesses;
        }

        /// <summary>
        /// Get processes by window title
        /// </summary>
        /// <param name="targetWindowTitle">Search for window title</param>
        /// <param name="exactName">Search for exact window title</param>
        public static List<ProcessMulti> Get_ProcessesMultiByWindowTitle(string targetWindowTitle, bool exactName)
        {
            //AVDebug.WriteLine("Getting processes by window title: " + targetWindowTitle);
            List<ProcessMulti> foundProcesses = new List<ProcessMulti>();
            try
            {
                string targetWindowTitleLower = targetWindowTitle.ToLower();
                foreach (ProcessMulti checkProcess in Get_AllProcessesMulti())
                {
                    try
                    {
                        string foundWindowTitleLower = checkProcess.WindowTitleMain.ToLower();
                        if (exactName)
                        {
                            if (foundWindowTitleLower == targetWindowTitleLower)
                            {
                                foundProcesses.Add(checkProcess);
                            }
                        }
                        else
                        {
                            if (foundWindowTitleLower.Contains(targetWindowTitleLower))
                            {
                                foundProcesses.Add(checkProcess);
                            }
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get multi processes by window title: " + ex.Message);
            }
            return foundProcesses;
        }

        //Get multi process by process id
        public static ProcessMulti Get_ProcessMultiByProcessId(int targetProcessId)
        {
            try
            {
                return new ProcessMulti(targetProcessId, 0, string.Empty);
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed get multi process by process id: " + targetProcessId + "/" + ex.Message);
                return null;
            }
        }
    }
}