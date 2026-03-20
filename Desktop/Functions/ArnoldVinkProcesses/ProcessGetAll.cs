using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Get all running processes
        public static List<AVProcess> Get_ProcessAll(int targetProcessId = -1)
        {
            List<AVProcess> listProcess = new List<AVProcess>();
            try
            {
                //AVDebug.WriteLine("Getting all processes.");

                //Query process information
                using AVFin spiQueryBuffer = new AVFin(AVFinMethod.FreeMarshal, Query_SystemProcessInformation());
                if (spiQueryBuffer.Get() == IntPtr.Zero)
                {
                    AVDebug.WriteLine("Failed getting all processes: query failed.");
                    return listProcess;
                }

                //Loop process information
                int systemProcessOffset = 0;
                while (true)
                {
                    try
                    {
                        //Get process information
                        SYSTEM_PROCESS_INFORMATION systemProcess = Marshal.PtrToStructure<SYSTEM_PROCESS_INFORMATION>(IntPtr.Add(spiQueryBuffer.Get(), systemProcessOffset));

                        //Add process to list
                        AVProcess process = new AVProcess(systemProcess.UniqueProcessId.ToInt32(), systemProcess.ParentProcessId.ToInt32(), systemProcess.ImageName.Buffer);

                        //Check target identifier
                        if (targetProcessId >= 0)
                        {
                            if (targetProcessId == process.Identifier)
                            {
                                return new() { process };
                            }
                        }
                        else
                        {
                            listProcess.Add(process);
                        }

                        //Move to next process
                        if (systemProcess.NextEntryOffset != 0)
                        {
                            systemProcessOffset += systemProcess.NextEntryOffset;
                        }
                        else
                        {
                            break;
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed getting all processes: " + ex.Message);
            }
            return listProcess;
        }

        //Get process by process id
        public static AVProcess Get_ProcessByProcessId(int targetProcessId)
        {
            try
            {
                return new AVProcess(targetProcessId);
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed get process by process id: " + targetProcessId + "/" + ex.Message);
                return null;
            }
        }

        //Get process for current process
        public static AVProcess Get_ProcessCurrent()
        {
            try
            {
                return Get_ProcessByProcessId(GetCurrentProcessId());
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get process for current process: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Get processes by AppUserModelId
        /// </summary>
        /// <param name="targetAppUserModelId">UWP or Win32Store AppUserModelId</param>
        public static List<AVProcess> Get_ProcessByAppUserModelId(string targetAppUserModelId)
        {
            //AVDebug.WriteLine("Getting processes by AppUserModelId: " + targetName);
            List<AVProcess> foundProcesses = new List<AVProcess>();
            try
            {
                string targetAppUserModelIdLower = targetAppUserModelId.ToLower();
                foreach (AVProcess checkProcess in Get_ProcessAll())
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
                AVDebug.WriteLine("Failed to get processes by AppUserModelId: " + ex.Message);
            }
            return foundProcesses;
        }

        //Get process by window handle
        public static AVProcess Get_ProcessByWindowHandle(IntPtr targetWindowHandle)
        {
            try
            {
                //Check if window handle is UWP or Win32Store application
                string appUserModelId = Detail_AppUserModelIdByWindowHandle(targetWindowHandle);
                if (!string.IsNullOrWhiteSpace(appUserModelId))
                {
                    return Get_ProcessByAppUserModelId(appUserModelId).FirstOrDefault();
                }
                else
                {
                    int processId = Detail_ProcessIdByWindowHandle(targetWindowHandle);
                    return Get_ProcessByProcessId(processId);
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get process by window handle: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Get processes by name
        /// </summary>
        /// <param name="targetName">Process name or executable name</param>
        /// <param name="exactName">Search for exact process name</param>
        public static List<AVProcess> Get_ProcessByName(string targetName, bool exactName)
        {
            //AVDebug.WriteLine("Getting processes by name: " + targetName);
            List<AVProcess> foundProcesses = new List<AVProcess>();
            try
            {
                //Lowercase executable name
                string targetNameLower = targetName.ToLower();

                //Look for executable name
                foreach (AVProcess checkProcess in Get_ProcessAll())
                {
                    try
                    {
                        //Lowercase executable name
                        string foundNameLower = checkProcess.ExeName.ToLower();
                        string foundNameNoExtLower = checkProcess.ExeNameNoExt.ToLower();

                        //Add process to list
                        if (exactName)
                        {
                            if (foundNameLower == targetNameLower || foundNameNoExtLower == targetNameLower)
                            {
                                foundProcesses.Add(checkProcess);
                            }
                        }
                        else
                        {
                            if (foundNameLower.Contains(targetNameLower) || foundNameNoExtLower.Contains(targetNameLower))
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
                AVDebug.WriteLine("Failed to get processes by name: " + ex.Message);
            }
            return foundProcesses;
        }

        /// <summary>
        /// Get processes by executable path
        /// </summary>
        /// <param name="targetExecutablePath">Process executable path</param>
        public static List<AVProcess> Get_ProcessByExecutablePath(string targetExecutablePath)
        {
            //AVDebug.WriteLine("Getting processes by executable path: " + targetExecutablePath);
            List<AVProcess> foundProcesses = new List<AVProcess>();
            try
            {
                string targetExecutablePathLower = targetExecutablePath.ToLower();
                foreach (AVProcess checkProcess in Get_ProcessAll())
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
                AVDebug.WriteLine("Failed to get processes by executable path: " + ex.Message);
            }
            return foundProcesses;
        }

        /// <summary>
        /// Get processes by window title
        /// </summary>
        /// <param name="targetWindowTitle">Search for window title</param>
        /// <param name="exactName">Search for exact window title</param>
        public static List<AVProcess> Get_ProcessByWindowTitle(string targetWindowTitle, bool exactName)
        {
            //AVDebug.WriteLine("Getting processes by window title: " + targetWindowTitle);
            List<AVProcess> foundProcesses = new List<AVProcess>();
            try
            {
                string targetWindowTitleLower = targetWindowTitle.ToLower();
                foreach (AVProcess checkProcess in Get_ProcessAll())
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
                AVDebug.WriteLine("Failed to get processes by window title: " + ex.Message);
            }
            return foundProcesses;
        }
    }
}