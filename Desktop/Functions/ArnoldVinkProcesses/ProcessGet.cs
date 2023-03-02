using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        /// <summary>
        /// Get process by identifier
        /// </summary>
        /// <param name="targetProcessId">Process identifier</param>
        public static Process Get_ProcessById(int targetProcessId)
        {
            try
            {
                return Process.GetProcessById(targetProcessId);
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get process by id: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Get processes by name
        /// </summary>
        /// <param name="targetProcessName">Process name without extension</param>
        /// <param name="exactName">Search for exact process name</param>
        public static Process[] Get_ProcessesByName(string targetProcessName, bool exactName)
        {
            try
            {
                List<Process> foundProcesses = new List<Process>();
                foreach (Process foundProcess in Process.GetProcesses())
                {
                    try
                    {
                        string targetExecutableNameLower = Path.GetFileNameWithoutExtension(targetProcessName).ToLower();
                        string foundExecutableNameLower = Path.GetFileNameWithoutExtension(foundProcess.ProcessName).ToLower();
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
                return foundProcesses.OrderByDescending(x => x.MainWindowHandle != IntPtr.Zero).ToArray();
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get processes by name: " + ex.Message);
                return new Process[0];
            }
        }

        /// <summary>
        /// Get processes by AppUserModelId
        /// </summary>
        /// <param name="targetAppUserModelId">UWP or Win32Store AppUserModelId</param>
        public static Process[] Get_ProcessesByAppUserModelId(string targetAppUserModelId)
        {
            try
            {
                List<Process> foundProcesses = new List<Process>();
                foreach (Process foundProcess in Process.GetProcesses())
                {
                    try
                    {
                        string targetAppUserModelIdLower = targetAppUserModelId.ToLower();
                        string foundAppUserModelIdLower = Detail_AppUserModelIdByProcess(foundProcess).ToLower();
                        if (foundAppUserModelIdLower == targetAppUserModelIdLower)
                        {
                            foundProcesses.Add(foundProcess);
                        }
                    }
                    catch { }
                }

                //Sort processes by main window handle
                return foundProcesses.OrderByDescending(x => x.MainWindowHandle != IntPtr.Zero).ToArray();
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get processes by AppUserModelId: " + ex.Message);
                return new Process[0];
            }
        }

        /// <summary>
        /// Get processes by main window title
        /// </summary>
        /// <param name="targetWindowTitle">Search for main window title</param>
        /// <param name="exactName">Search for exact main window title</param>
        public static Process[] Get_ProcessesByMainWindowTitle(string targetWindowTitle, bool exactName)
        {
            try
            {
                if (exactName)
                {
                    return Process.GetProcesses().Where(x => x.MainWindowTitle.ToLower() == targetWindowTitle.ToLower()).OrderByDescending(x => x.MainWindowHandle != IntPtr.Zero).ToArray();
                }
                else
                {
                    return Process.GetProcesses().Where(x => x.MainWindowTitle.ToLower().Contains(targetWindowTitle.ToLower())).OrderByDescending(x => x.MainWindowHandle != IntPtr.Zero).ToArray();
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get processes by window title: " + ex.Message);
                return new Process[0];
            }
        }

        /// <summary>
        /// Get process by main window handle
        /// </summary>
        /// <param name="targetWindowHandle">Search for main window handle</param>
        public static Process Get_ProcessByMainWindowHandle(IntPtr targetWindowHandle)
        {
            try
            {
                return Process.GetProcesses().Where(x => x.MainWindowHandle == targetWindowHandle).FirstOrDefault();
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get process by main window handle: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Get process by window handle
        /// </summary>
        /// <param name="targetWindowHandle">Search for window handle</param>
        public static Process Get_ProcessByWindowHandle(IntPtr targetWindowHandle)
        {
            try
            {
                int foundProcessId = Detail_ProcessIdByWindowHandle(targetWindowHandle);
                return Process.GetProcessById(foundProcessId);
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get process by window handle: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Get process by handle
        /// </summary>
        /// <param name="targetProcessHandle">Search for handle</param>
        public static Process Get_ProcessByHandle(IntPtr targetProcessHandle)
        {
            try
            {
                return Process.GetProcesses().Where(x => x.Handle == targetProcessHandle).FirstOrDefault();
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get process by handle: " + ex.Message);
                return null;
            }
        }

        //Get all uwp application processes
        public static List<ProcessMulti> Get_ProcessesUwp()
        {
            List<ProcessMulti> processList = new List<ProcessMulti>();
            try
            {
                Process frameHostProcess = Get_ProcessesByName("ApplicationFrameHost", true).FirstOrDefault();
                if (frameHostProcess != null)
                {
                    foreach (ProcessThread threadProcess in frameHostProcess.Threads)
                    {
                        try
                        {
                            //Process variables
                            bool processInterfaceChecked = false;
                            IntPtr processWindowHandle = IntPtr.Zero;

                            foreach (IntPtr threadWindowHandle in Thread_GetWindowHandles(threadProcess.Id))
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