using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Get process thread identifiers
        public static List<int> Thread_GetProcessThreadIds(int processId)
        {
            //AVDebug.WriteLine("Getting thread identifiers: " + processId);
            IntPtr toolSnapShot = IntPtr.Zero;
            List<int> listIdentifiers = new List<int>();
            try
            {
                toolSnapShot = CreateToolhelp32Snapshot(SNAPSHOT_FLAGS.TH32CS_SNAPTHREAD, 0);
                if (toolSnapShot == IntPtr.Zero)
                {
                    AVDebug.WriteLine("GetProcessThreadIds failed: Zero snapshot.");
                    return listIdentifiers;
                }

                THREADENTRY32 threadEntry = new THREADENTRY32();
                threadEntry.dwSize = (uint)Marshal.SizeOf(threadEntry);

                while (Thread32Next(toolSnapShot, ref threadEntry))
                {
                    try
                    {
                        if (threadEntry.th32OwnerProcessID == processId)
                        {
                            listIdentifiers.Add(threadEntry.th32ThreadID);
                        }
                    }
                    catch { }
                }
            }
            catch { }
            finally
            {
                CloseHandleAuto(toolSnapShot);
            }
            return listIdentifiers;
        }

        //Enumerate all thread windows including fullscreen
        public static List<IntPtr> Thread_GetWindowHandles(int threadId)
        {
            //AVDebug.WriteLine("Getting thread window handles: " + threadId);
            List<IntPtr> listWindows = new List<IntPtr>();
            try
            {
                IntPtr childWindow = IntPtr.Zero;
                while ((childWindow = FindWindowEx(IntPtr.Zero, childWindow, null, null)) != IntPtr.Zero)
                {
                    try
                    {
                        int foundProcessId = 0;
                        int foundThreadId = GetWindowThreadProcessId(childWindow, out foundProcessId);
                        if (foundThreadId == threadId)
                        {
                            listWindows.Add(childWindow);
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return listWindows;
        }
    }
}