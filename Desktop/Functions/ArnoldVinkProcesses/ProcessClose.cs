using System;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Close process by identifier
        public static bool Close_ProcessById(int targetProcessId)
        {
            try
            {
                IntPtr closeProcess = OpenProcess(PROCESS_DESIRED_ACCESS.PROCESS_TERMINATE, false, targetProcessId);
                if (closeProcess == IntPtr.Zero)
                {
                    AVDebug.WriteLine("Failed closing process by id: " + targetProcessId + "/Process not found.");
                    return false;
                }
                else
                {
                    bool processClosed = TerminateProcess(closeProcess, 1);
                    CloseHandleAuto(closeProcess);
                    AVDebug.WriteLine("Closed process by id: " + targetProcessId + "/" + processClosed);
                    return processClosed;
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed closing process by id: " + targetProcessId + "/" + ex.Message);
                return false;
            }
        }

        //Close process tree by identifier
        public static bool Close_ProcessTreeById(int targetProcessId)
        {
            try
            {
                //Close child processes
                foreach (ProcessHandle childProcess in Get_AllProcessesHandle(false))
                {
                    try
                    {
                        if (childProcess.ParentIdentifier == targetProcessId)
                        {
                            Close_ProcessById(childProcess.Identifier);
                        }
                    }
                    catch { }
                }

                //Close parent process
                Close_ProcessById(targetProcessId);

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
                foreach (ProcessMulti allProcesses in Get_ProcessesByName(targetProcessName, exactName))
                {
                    try
                    {
                        if (Close_ProcessTreeById(allProcesses.Identifier))
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

        //Close processes by AppUserModelId
        public static bool Close_ProcessesByAppUserModelId(string targetAppUserModelId)
        {
            try
            {
                bool processClosed = false;
                foreach (ProcessMulti allProcesses in Get_ProcessesByAppUserModelId(targetAppUserModelId))
                {
                    try
                    {
                        if (Close_ProcessTreeById(allProcesses.Identifier))
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
                SendMessage(targetWindowHandle, (int)WindowMessages.WM_CLOSE, 0, 0);
                SendMessage(targetWindowHandle, (int)WindowMessages.WM_QUIT, 0, 0);

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