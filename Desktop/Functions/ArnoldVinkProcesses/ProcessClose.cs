using System;
using System.Diagnostics;
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
                Process.GetProcessById(targetProcessId).Kill();

                AVDebug.WriteLine("Closed process by id: " + targetProcessId);
                return true;
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
                foreach (Process childProcess in Process.GetProcesses())
                {
                    try
                    {
                        int parentProcessId = Detail_ProcessParentIdByProcess(childProcess);
                        if (parentProcessId == targetProcessId)
                        {
                            childProcess.Kill();
                        }
                    }
                    catch { }
                }

                //Close parent process
                Process.GetProcessById(targetProcessId).Kill();

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
                foreach (Process allProcesses in Get_ProcessesByName(targetProcessName, exactName))
                {
                    if (Close_ProcessTreeById(allProcesses.Id))
                    {
                        processClosed = true;
                    }
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