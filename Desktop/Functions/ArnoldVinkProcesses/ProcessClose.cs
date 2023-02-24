using System;
using System.Diagnostics;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        public static bool Close_ProcessById(int processId)
        {
            try
            {
                //Close process
                Process.GetProcessById(processId).Kill();

                Debug.WriteLine("Closed process by id: " + processId);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed closing process by id: " + processId + "/" + ex.Message);
                return false;
            }
        }

        public static bool Close_ProcessTreeById(int processId)
        {
            try
            {
                //Close child processes
                foreach (Process childProcess in Process.GetProcesses())
                {
                    try
                    {
                        int parentProcessId = Process_GetParentId(childProcess);
                        if (parentProcessId == processId)
                        {
                            childProcess.Kill();
                        }
                    }
                    catch { }
                }

                //Close parent process
                Process.GetProcessById(processId).Kill();

                Debug.WriteLine("Closed process tree by id: " + processId);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed closing process tree by id: " + processId + "/" + ex.Message);
                return false;
            }
        }

        public static bool Close_ProcessesByName(string processName, bool exactName)
        {
            try
            {
                bool processClosed = false;
                foreach (Process AllProcess in GetProcessesByName(processName, exactName))
                {
                    if (Close_ProcessTreeById(AllProcess.Id))
                    {
                        processClosed = true;
                    }
                }

                Debug.WriteLine("Closed processes by name: " + processName + "/" + processClosed);
                return processClosed;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed closing processes by name: " + processName + "/" + ex.Message);
                return false;
            }
        }

        public static bool Close_ProcessByWindowMessage(IntPtr windowHandle)
        {
            try
            {
                SendMessage(windowHandle, (int)WindowMessages.WM_CLOSE, 0, 0);
                SendMessage(windowHandle, (int)WindowMessages.WM_QUIT, 0, 0);

                Debug.WriteLine("Closed process by window message: " + windowHandle);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed closing process by window message: " + windowHandle + "/" + ex.Message);
                return false;
            }
        }
    }
}