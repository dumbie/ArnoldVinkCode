using System;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Close process by identifier
        public static bool Close_ProcessByProcessId(int targetProcessId)
        {
            try
            {
                if (GetCurrentProcessId() == targetProcessId)
                {
                    AVDebug.WriteLine("Prevented closing process by id: " + targetProcessId + "/Process is current application.");
                    return false;
                }

                using AVFin closeProcess = new AVFin(AVFinMethod.CloseHandle, OpenProcess(PROCESS_DESIRED_ACCESS.PROCESS_TERMINATE, false, targetProcessId));
                if (closeProcess.Get() == IntPtr.Zero)
                {
                    AVDebug.WriteLine("Failed closing process by id: " + targetProcessId + "/Process not found.");
                    return false;
                }
                else
                {
                    bool processClosed = TerminateProcess(closeProcess.Get(), 0);
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
        public static bool Close_ProcessTreeByProcessId(int targetProcessId)
        {
            try
            {
                //Close child processes
                foreach (AVProcess childProcess in Get_ProcessAll())
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

        //Close process by name
        public static bool Close_ProcessByName(string targetProcessName, bool exactName)
        {
            try
            {
                bool processClosed = false;
                foreach (AVProcess foundProcesses in Get_ProcessByName(targetProcessName, exactName))
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

                AVDebug.WriteLine("Closed process by name: " + targetProcessName + "/" + processClosed);
                return processClosed;
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed closing process by name: " + targetProcessName + "/" + ex.Message);
                return false;
            }
        }

        //Close process by executable path
        public static bool Close_ProcessByExecutablePath(string targetExecutablePath)
        {
            try
            {
                bool processClosed = false;
                foreach (AVProcess foundProcesses in Get_ProcessByExecutablePath(targetExecutablePath))
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

                AVDebug.WriteLine("Closed process by executable path: " + targetExecutablePath + "/" + processClosed);
                return processClosed;
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed closing process by executable path: " + targetExecutablePath + "/" + ex.Message);
                return false;
            }
        }

        //Close process by AppUserModelId
        public static bool Close_ProcessByAppUserModelId(string targetAppUserModelId)
        {
            try
            {
                bool processClosed = false;
                foreach (AVProcess foundProcesses in Get_ProcessByAppUserModelId(targetAppUserModelId))
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

                AVDebug.WriteLine("Closed process by AppUserModelId: " + targetAppUserModelId + "/" + processClosed);
                return processClosed;
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed closing process by AppUserModelId: " + targetAppUserModelId + "/" + ex.Message);
                return false;
            }
        }

        //Close process by window message
        public static bool Close_ProcessByWindowMessage(IntPtr targetWindowHandle)
        {
            try
            {
                PostMessage(targetWindowHandle, WindowMessages.WM_CLOSE, 0, 0);
                PostMessage(targetWindowHandle, WindowMessages.WM_QUIT, 0, 0);

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