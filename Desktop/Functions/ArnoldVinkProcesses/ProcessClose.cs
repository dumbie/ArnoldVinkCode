using System;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Close process by process identifier
        public static bool Close_ProcessByProcessId(int targetProcessId)
        {
            IntPtr closeProcess = IntPtr.Zero;
            try
            {
                if (GetCurrentProcessId() == targetProcessId)
                {
                    AVDebug.WriteLine("Prevented closing process by id: " + targetProcessId + "/Process is application.");
                    return false;
                }

                closeProcess = OpenProcess(PROCESS_DESIRED_ACCESS.PROCESS_TERMINATE, false, targetProcessId);
                if (closeProcess == IntPtr.Zero)
                {
                    AVDebug.WriteLine("Failed closing process by id: " + targetProcessId + "/Process not found.");
                    return false;
                }
                else
                {
                    bool processClosed = TerminateProcess(closeProcess, 1);
                    AVDebug.WriteLine("Closed process by id: " + targetProcessId + "/" + processClosed);
                    return processClosed;
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed closing process by id: " + targetProcessId + "/" + ex.Message);
                return false;
            }
            finally
            {
                SafeCloseHandle(ref closeProcess);
            }
        }

        //Close process tree by process identifier
        public static bool Close_ProcessTreeByProcessId(int targetProcessId)
        {
            try
            {
                //Close child processes
                foreach (ProcessMulti childProcess in Get_AllProcessesMulti())
                {
                    try
                    {
                        if (childProcess.IdentifierParent == targetProcessId)
                        {
                            Close_ProcessByProcessId(childProcess.Identifier);
                        }
                    }
                    catch { }
                }

                //Close parent process
                Close_ProcessByProcessId(targetProcessId);

                AVDebug.WriteLine("Closed process tree by id: " + targetProcessId);
                return true;
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed closing process tree by id: " + targetProcessId + "/" + ex.Message);
                return false;
            }
        }

        //Close processes by name
        public static bool Close_ProcessesByName(string targetProcessName, bool exactName)
        {
            try
            {
                bool processClosed = false;
                foreach (ProcessMulti foundProcesses in Get_ProcessesMultiByName(targetProcessName, exactName))
                {
                    try
                    {
                        if (Close_ProcessTreeByProcessId(foundProcesses.Identifier))
                        {
                            processClosed = true;
                        }
                    }
                    catch { }
                }

                AVDebug.WriteLine("Closed processes by name: " + targetProcessName + "/" + processClosed);
                return processClosed;
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed closing processes by name: " + targetProcessName + "/" + ex.Message);
                return false;
            }
        }

        //Close processes by executable path
        public static bool Close_ProcessesByExecutablePath(string targetExecutablePath)
        {
            try
            {
                bool processClosed = false;
                foreach (ProcessMulti foundProcesses in Get_ProcessesMultiByExecutablePath(targetExecutablePath))
                {
                    try
                    {
                        if (Close_ProcessTreeByProcessId(foundProcesses.Identifier))
                        {
                            processClosed = true;
                        }
                    }
                    catch { }
                }

                AVDebug.WriteLine("Closed processes by executable path: " + targetExecutablePath + "/" + processClosed);
                return processClosed;
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed closing processes by executable path: " + targetExecutablePath + "/" + ex.Message);
                return false;
            }
        }

        //Close processes by AppUserModelId
        public static bool Close_ProcessesByAppUserModelId(string targetAppUserModelId)
        {
            try
            {
                bool processClosed = false;
                foreach (ProcessMulti foundProcesses in Get_ProcessesMultiByAppUserModelId(targetAppUserModelId))
                {
                    try
                    {
                        if (Close_ProcessTreeByProcessId(foundProcesses.Identifier))
                        {
                            processClosed = true;
                        }
                    }
                    catch { }
                }

                AVDebug.WriteLine("Closed processes by AppUserModelId: " + targetAppUserModelId + "/" + processClosed);
                return processClosed;
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed closing processes by AppUserModelId: " + targetAppUserModelId + "/" + ex.Message);
                return false;
            }
        }

        //Close process by window message
        public static bool Close_ProcessByWindowMessage(IntPtr targetWindowHandle)
        {
            try
            {
                PostMessageAsync(targetWindowHandle, WindowMessages.WM_CLOSE, 0, 0);
                PostMessageAsync(targetWindowHandle, WindowMessages.WM_QUIT, 0, 0);

                AVDebug.WriteLine("Closed process by window message: " + targetWindowHandle);
                return true;
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed closing process by window message: " + targetWindowHandle + "/" + ex.Message);
                return false;
            }
        }
    }
}