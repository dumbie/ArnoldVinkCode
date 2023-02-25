using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                Debug.WriteLine("Failed to get process by id: " + ex.Message);
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
                if (exactName)
                {
                    return Process.GetProcesses().Where(x => x.ProcessName.ToLower() == targetProcessName.ToLower()).OrderByDescending(x => x.MainWindowHandle != IntPtr.Zero).ToArray();
                }
                else
                {
                    return Process.GetProcesses().Where(x => x.ProcessName.ToLower().Contains(targetProcessName.ToLower())).OrderByDescending(x => x.MainWindowHandle != IntPtr.Zero).ToArray();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get processes by name: " + ex.Message);
                return new Process[0];
            }
        }

        /// <summary>
        /// Get processes by window title
        /// </summary>
        /// <param name="targetWindowTitle">Search for window title</param>
        /// <param name="exactName">Search for exact window title</param>
        public static Process[] Get_ProcessesByWindowTitle(string targetWindowTitle, bool exactName)
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
                Debug.WriteLine("Failed to get processes by window title: " + ex.Message);
                return new Process[0];
            }
        }

        /// <summary>
        /// Get processes by window handle
        /// </summary>
        /// <param name="targetWindowHandle">Search for window handle</param>
        public static Process[] Get_ProcessesByWindowHandle(IntPtr targetWindowHandle)
        {
            try
            {
                return Process.GetProcesses().Where(x => x.MainWindowHandle == targetWindowHandle).OrderByDescending(x => x.MainWindowHandle != IntPtr.Zero).ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get processes by window handle: " + ex.Message);
                return new Process[0];
            }
        }

        /// <summary>
        /// Get processes by handle
        /// </summary>
        /// <param name="targetProcessHandle">Search for handle</param>
        public static Process[] Get_ProcessesByHandle(IntPtr targetProcessHandle)
        {
            try
            {
                return Process.GetProcesses().Where(x => x.Handle == targetProcessHandle).OrderByDescending(x => x.MainWindowHandle != IntPtr.Zero).ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get processes by handle: " + ex.Message);
                return new Process[0];
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