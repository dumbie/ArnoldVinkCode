using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        /// <summary>
        /// Get process handle by identifier
        /// </summary>
        /// <param name="targetProcessId">Process identifier</param>
        public static IntPtr Get_ProcessHandleById(int targetProcessId)
        {
            try
            {
                IntPtr hProcess = OpenProcess(PROCESS_DESIRED_ACCESS.PROCESS_MAXIMUM_ALLOWED, false, targetProcessId);
                if (hProcess == IntPtr.Zero)
                {
                    AVDebug.WriteLine("Failed opening process id: " + targetProcessId + "/" + Marshal.GetLastWin32Error());
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

        //Get all processes multi
        public static List<ProcessMulti> Get_AllProcessesMulti()
        {
            //AVDebug.WriteLine("Getting all processes multi.");
            IntPtr toolSnapShot = IntPtr.Zero;
            List<ProcessMulti> listProcesses = new List<ProcessMulti>();
            try
            {
                toolSnapShot = CreateToolhelp32Snapshot(SNAPSHOT_FLAGS.TH32CS_SNAPPROCESS, 0);
                if (toolSnapShot == IntPtr.Zero)
                {
                    AVDebug.WriteLine("Get AllProcessesMulti failed: Zero snapshot.");
                    return listProcesses;
                }

                PROCESSENTRY32 processEntry = new PROCESSENTRY32();
                processEntry.dwSize = (uint)Marshal.SizeOf(processEntry);

                while (Process32Next(toolSnapShot, ref processEntry))
                {
                    try
                    {
                        ProcessMulti processMulti = Get_ProcessMultiByProcessId(processEntry.th32ProcessID);
                        if (processMulti != null)
                        {
                            listProcesses.Add(processMulti);
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get all processes multi: " + ex.Message);
            }
            finally
            {
                CloseHandleAuto(toolSnapShot);
            }
            return listProcesses;
        }

        /// <summary>
        /// Get processes by name
        /// </summary>
        /// <param name="targetProcessName">Process name without extension</param>
        /// <param name="exactName">Search for exact process name</param>
        public static ProcessMulti[] Get_ProcessesByName(string targetProcessName, bool exactName)
        {
            try
            {
                List<ProcessMulti> foundProcesses = new List<ProcessMulti>();
                foreach (ProcessMulti foundProcess in Get_AllProcessesMulti())
                {
                    try
                    {
                        string targetExecutableNameLower = Path.GetFileNameWithoutExtension(targetProcessName).ToLower();
                        string foundExecutableNameLower = foundProcess.ExeNameNoExt.ToLower();
                        if (exactName)
                        {
                            if (foundExecutableNameLower == targetExecutableNameLower)
                            {
                                foundProcesses.Add(foundProcess);
                            }
                        }
                        else
                        {
                            if (foundExecutableNameLower.Contains(targetExecutableNameLower))
                            {
                                foundProcesses.Add(foundProcess);
                            }
                        }
                    }
                    catch { }
                }

                //Sort processes by main window handle
                return foundProcesses.OrderByDescending(x => x.WindowHandle != IntPtr.Zero).ToArray();
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get processes by name: " + ex.Message);
                return new ProcessMulti[0];
            }
        }

        /// <summary>
        /// Get processes by AppUserModelId
        /// </summary>
        /// <param name="targetAppUserModelId">UWP or Win32Store AppUserModelId</param>
        public static ProcessMulti[] Get_ProcessesByAppUserModelId(string targetAppUserModelId)
        {
            try
            {
                List<ProcessMulti> foundProcesses = new List<ProcessMulti>();
                foreach (ProcessMulti foundProcess in Get_AllProcessesMulti())
                {
                    try
                    {
                        string targetAppUserModelIdLower = targetAppUserModelId.ToLower();
                        string foundAppUserModelIdLower = foundProcess.AppUserModelId.ToLower();
                        if (foundAppUserModelIdLower == targetAppUserModelIdLower)
                        {
                            foundProcesses.Add(foundProcess);
                        }
                    }
                    catch { }
                }

                //Sort processes by main window handle
                return foundProcesses.OrderByDescending(x => x.WindowHandle != IntPtr.Zero).ToArray();
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get processes by AppUserModelId: " + ex.Message);
                return new ProcessMulti[0];
            }
        }

        /// <summary>
        /// Get processes by window title
        /// </summary>
        /// <param name="targetWindowTitle">Search for window title</param>
        /// <param name="exactName">Search for exact window title</param>
        public static ProcessMulti[] Get_ProcessesByWindowTitle(string targetWindowTitle, bool exactName)
        {
            try
            {
                if (exactName)
                {
                    return Get_AllProcessesMulti().Where(x => x.WindowTitle.ToLower() == targetWindowTitle.ToLower()).OrderByDescending(x => x.WindowHandle != IntPtr.Zero).ToArray();
                }
                else
                {
                    return Get_AllProcessesMulti().Where(x => x.WindowTitle.ToLower().Contains(targetWindowTitle.ToLower())).OrderByDescending(x => x.WindowHandle != IntPtr.Zero).ToArray();
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get processes by window title: " + ex.Message);
                return new ProcessMulti[0];
            }
        }

        /// <summary>
        /// Get process multi by handle
        /// </summary>
        /// <param name="targetProcessHandle">Search for handle</param>
        public static ProcessMulti Get_ProcessMultiByHandle(IntPtr targetProcessHandle)
        {
            try
            {
                //Fix replace with ProcessHandle
                return Get_AllProcessesMulti().Where(x => x.Handle == targetProcessHandle).FirstOrDefault();
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get process by handle: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Get process multi by window handle
        /// </summary>
        /// <param name="targetWindowHandle">Search for window handle</param>
        public static ProcessMulti Get_ProcessMultiWindowHandle(IntPtr targetWindowHandle)
        {
            try
            {
                return Get_AllProcessesMulti().Where(x => x.WindowHandle == targetWindowHandle).FirstOrDefault();
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get process by window handle: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Get process id by window handle
        /// </summary>
        /// <param name="targetWindowHandle">Search for window handle</param>
        public static int Get_ProcessIdByWindowHandle(IntPtr targetWindowHandle)
        {
            try
            {
                return Detail_ProcessIdByWindowHandle(targetWindowHandle);
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get process by window handle: " + ex.Message);
                return 0;
            }
        }

        //Get all uwp application processes
        public static List<ProcessMulti> Get_ProcessesUwp()
        {
            List<ProcessMulti> processList = new List<ProcessMulti>();
            try
            {
                ProcessMulti frameHostProcess = Get_ProcessesByName("ApplicationFrameHost", true).FirstOrDefault();
                if (frameHostProcess != null)
                {
                    foreach (int threadId in frameHostProcess.GetProcessThreads())
                    {
                        try
                        {
                            //Process variables
                            bool processInterfaceChecked = false;
                            IntPtr processWindowHandle = IntPtr.Zero;

                            foreach (IntPtr threadWindowHandle in Thread_GetWindowHandles(threadId))
                            {
                                try
                                {
                                    //Get window class name
                                    string classNameString = Detail_ClassNameByWindowHandle(threadWindowHandle);

                                    //Get application process
                                    if (classNameString == "ApplicationFrameWindow")
                                    {
                                        processWindowHandle = threadWindowHandle;
                                    }

                                    //Check if process has interface
                                    if (classNameString == "MSCTFIME UI")
                                    {
                                        processInterfaceChecked = true;
                                    }
                                }
                                catch { }
                            }

                            //Add uwp process
                            if (processInterfaceChecked && processWindowHandle != IntPtr.Zero)
                            {
                                ProcessMulti processMulti = Get_ProcessMultiByWindowHandle(processWindowHandle);
                                if (processMulti != null)
                                {
                                    processList.Add(processMulti);
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }
            return processList;
        }
    }
}