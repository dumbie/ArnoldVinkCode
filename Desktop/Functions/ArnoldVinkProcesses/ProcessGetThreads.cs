using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Get process thread information by process id
        public static List<ProcessThreadInfo> Get_ProcessThreadsByProcessId(int targetProcessId, bool firstThreadOnly)
        {
            List<ProcessThreadInfo> listProcessThread = new List<ProcessThreadInfo>();
            try
            {
                //AVDebug.WriteLine("Getting process threads for process id: " + targetProcessId + "/" + firstThreadOnly);

                //Query process information
                using AVFin spiQueryBuffer = new AVFin(AVFinMethod.FreeMarshal, Query_SystemProcessInformation());
                if (spiQueryBuffer.Get() == IntPtr.Zero)
                {
                    AVDebug.WriteLine("Failed getting all process threads: query failed.");
                    return listProcessThread;
                }

                //Loop process information
                int systemProcessOffset = 0;
                while (true)
                {
                    try
                    {
                        //Get process information
                        SYSTEM_PROCESS_INFORMATION systemProcess = Marshal.PtrToStructure<SYSTEM_PROCESS_INFORMATION>(IntPtr.Add(spiQueryBuffer.Get(), systemProcessOffset));

                        //Check target process id
                        if (targetProcessId == systemProcess.UniqueProcessId.ToInt32())
                        {
                            //AVDebug.WriteLine("Found thread process: " + systemProcess.UniqueProcessId.ToInt32() + " / " + systemProcess.NumberOfThreads);

                            //Loop threads
                            int systemThreadOffset = Marshal.SizeOf(typeof(SYSTEM_PROCESS_INFORMATION));
                            for (int i = 0; i < systemProcess.NumberOfThreads; i++)
                            {
                                try
                                {
                                    //Get thread information
                                    SYSTEM_THREAD_INFORMATION systemThread = Marshal.PtrToStructure<SYSTEM_THREAD_INFORMATION>(IntPtr.Add(spiQueryBuffer.Get(), systemProcessOffset + systemThreadOffset));

                                    //Add process thread to list
                                    ProcessThreadInfo processThread = new ProcessThreadInfo();
                                    processThread.Identifier = systemThread.ThreadId.ToInt32();
                                    processThread.ThreadState = systemThread.ThreadState;
                                    processThread.ThreadWaitReason = systemThread.WaitReason;
                                    listProcessThread.Add(processThread);
                                    //AVDebug.WriteLine("Found thread: " + processThread.Identifier + "/" + processThread.ThreadState + "/" + processThread.ThreadWaitReason);

                                    //Return first process thread
                                    if (firstThreadOnly)
                                    {
                                        return listProcessThread;
                                    }

                                    //Move to next thread
                                    systemThreadOffset += Marshal.SizeOf(typeof(SYSTEM_THREAD_INFORMATION));
                                }
                                catch { }
                            }
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
                AVDebug.WriteLine("Failed getting all process threads: " + ex.Message);
            }
            return listProcessThread;
        }
    }
}