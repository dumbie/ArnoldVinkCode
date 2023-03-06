using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Get process thread identifiers by process id
        public static List<int> Get_ThreadIdsByProcessId(int targetProcessId)
        {
            //AVDebug.WriteLine("Getting thread identifiers: " + targetProcessId);
            IntPtr toolSnapShot = IntPtr.Zero;
            List<int> listIdentifiers = new List<int>();
            try
            {
                toolSnapShot = CreateToolhelp32Snapshot(SNAPSHOT_FLAGS.TH32CS_SNAPTHREAD, 0);
                if (toolSnapShot == IntPtr.Zero)
                {
                    AVDebug.WriteLine("Failed to get process thread ids: " + targetProcessId + "/Zero snapshot.");
                    return listIdentifiers;
                }

                THREADENTRY32 threadEntry = new THREADENTRY32();
                threadEntry.dwSize = (uint)Marshal.SizeOf(threadEntry);

                while (Thread32Next(toolSnapShot, ref threadEntry))
                {
                    try
                    {
                        if (threadEntry.th32OwnerProcessID == targetProcessId)
                        {
                            listIdentifiers.Add(threadEntry.th32ThreadID);
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get process thread ids: " + targetProcessId + "/" + ex.Message);
            }
            finally
            {
                CloseHandleAuto(toolSnapShot);
            }
            return listIdentifiers;
        }
    }
}