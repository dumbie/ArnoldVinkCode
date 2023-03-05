using System;
using System.Collections.Generic;
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

        //Get all processes connect
        public static List<ProcessConnect> Get_AllProcessesConnect(bool openProcess)
        {
            //AVDebug.WriteLine("Getting all processes connect.");
            IntPtr toolSnapShot = IntPtr.Zero;
            List<ProcessConnect> listProcesses = new List<ProcessConnect>();
            try
            {
                toolSnapShot = CreateToolhelp32Snapshot(SNAPSHOT_FLAGS.TH32CS_SNAPPROCESS, 0);
                if (toolSnapShot == IntPtr.Zero)
                {
                    AVDebug.WriteLine("Get AllProcessesConnect failed: Zero snapshot.");
                    return listProcesses;
                }

                PROCESSENTRY32 processEntry = new PROCESSENTRY32();
                processEntry.dwSize = (uint)Marshal.SizeOf(processEntry);

                while (Process32Next(toolSnapShot, ref processEntry))
                {
                    try
                    {
                        bool addProcessHandle = true;
                        ProcessConnect processHandle = new ProcessConnect();
                        processHandle.Identifier = processEntry.th32ProcessID;
                        processHandle.ParentIdentifier = processEntry.th32ParentProcessID;
                        if (openProcess)
                        {
                            processHandle.Handle = Get_ProcessHandleById(processHandle.Identifier);
                            addProcessHandle = processHandle.Handle != IntPtr.Zero;
                        }
                        if (addProcessHandle)
                        {
                            listProcesses.Add(processHandle);
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get all processes connect: " + ex.Message);
            }
            finally
            {
                CloseHandleAuto(toolSnapShot);
            }
            return listProcesses;
        }

        //Get all processes multi
        public static List<ProcessMulti> Get_AllProcessesMulti()
        {
            //AVDebug.WriteLine("Getting all processes multi.");
            List<ProcessMulti> listProcesses = new List<ProcessMulti>();
            try
            {
                foreach (ProcessConnect processHandle in Get_AllProcessesConnect(true))
                {
                    listProcesses.Add(Get_ProcessMultiByProcessId(processHandle.Identifier, processHandle.ParentIdentifier, processHandle.Handle));
                }
            }
            catch { }
            return listProcesses;
        }

        /// <summary>
        /// Get processes by name
        /// </summary>
        /// <param name="targetName">Process name or executable name</param>
        /// <param name="exactName">Search for exact process name</param>
        public static List<ProcessConnect> Get_ProcessesByName(string targetName, bool exactName)
        {
            //AVDebug.WriteLine("Getting processes by name: " + targetName);
            List<ProcessConnect> foundProcesses = new List<ProcessConnect>();
            try
            {
                foreach (ProcessConnect foundProcess in Get_AllProcessesConnect(true))
                {
                    try
                    {
                        string targetNameLower = targetName.ToLower();
                        string foundExecutablePath = Detail_ExecutablePathByProcessHandle(foundProcess.Handle);
                        string foundExecutableNameLower = Path.GetFileName(foundExecutablePath).ToLower();
                        string foundProcessNameLower = Path.GetFileNameWithoutExtension(foundExecutablePath).ToLower();
                        if (exactName)
                        {
                            if (foundExecutableNameLower == targetNameLower || foundProcessNameLower == targetNameLower)
                            {
                                foundProcesses.Add(foundProcess);
                            }
                        }
                        else
                        {
                            if (foundExecutableNameLower.Contains(targetNameLower) || foundProcessNameLower.Contains(targetNameLower))
                            {
                                foundProcesses.Add(foundProcess);
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
        /// Get processes by AppUserModelId
        /// </summary>
        /// <param name="targetAppUserModelId">UWP or Win32Store AppUserModelId</param>
        public static List<ProcessConnect> Get_ProcessesByAppUserModelId(string targetAppUserModelId)
        {
            //AVDebug.WriteLine("Getting processes by AppUserModelId: " + targetName);
            List<ProcessConnect> foundProcesses = new List<ProcessConnect>();
            try
            {
                foreach (ProcessConnect foundProcess in Get_AllProcessesConnect(true))
                {
                    try
                    {
                        string targetAppUserModelIdLower = targetAppUserModelId.ToLower();
                        string foundAppUserModelIdLower = Detail_AppUserModelIdByProcessHandle(foundProcess.Handle).ToLower();
                        if (foundAppUserModelIdLower == targetAppUserModelIdLower)
                        {
                            foundProcesses.Add(foundProcess);
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

        /// <summary>
        /// Get processes by window title
        /// </summary>
        /// <param name="targetWindowTitle">Search for window title</param>
        /// <param name="exactName">Search for exact window title</param>
        public static List<ProcessConnect> Get_ProcessesByWindowTitle(string targetWindowTitle, bool exactName)
        {
            AVDebug.WriteLine("Getting processes by window title: " + targetWindowTitle);
            List<ProcessConnect> foundProcesses = new List<ProcessConnect>();
            try
            {
                foreach (ProcessConnect foundProcess in Get_AllProcessesConnect(true))
                {
                    try
                    {
                        IntPtr mainWindowHandle = Detail_MainWindowHandleByProcessId(foundProcess.Identifier);
                        string targetWindowTitleLower = targetWindowTitle.ToLower();
                        string foundWindowTitleLower = Detail_WindowTitleByWindowHandle(mainWindowHandle).ToLower();
                        if (exactName)
                        {
                            if (foundWindowTitleLower == targetWindowTitleLower)
                            {
                                foundProcesses.Add(foundProcess);
                            }
                        }
                        else
                        {
                            if (foundWindowTitleLower.Contains(targetWindowTitleLower))
                            {
                                foundProcesses.Add(foundProcess);
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
    }
}