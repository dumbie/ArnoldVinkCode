using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Imports
        [DllImport("ntdll.dll")]
        public static extern uint NtQuerySystemInformation(SYSTEM_INFO_CLASS SystemInformationClass, IntPtr SystemInformation, uint SystemInformationLength, out uint ReturnLength);

        //Constants
        public const uint STATUS_INFO_LENGTH_MISMATCH = 0xC0000004;

        //Structures
        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_THREAD_INFORMATION
        {
            public long KernelTime;
            public long UserTime;
            public long CreateTime;
            public uint WaitTime;
            public IntPtr StartAddress;
            public IntPtr UniqueProcessId;
            public IntPtr UniqueThreadId;
            public int Priority;
            public int BasePriority;
            public uint ContextSwitches;
            public ProcessThreadState ThreadState;
            public ProcessThreadWaitReason ThreadWaitReason;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_PROCESS_INFORMATION
        {
            public uint NextEntryOffset;
            public uint NumberOfThreads;
            public long SpareLi1;
            public long SpareLi2;
            public long SpareLi3;
            public long CreateTime;
            public long UserTime;
            public long KernelTime;
            public ushort NameLength;
            public ushort MaximumNameLength;
            public IntPtr NamePtr;
            public int BasePriority;
            public IntPtr UniqueProcessId;
            public IntPtr ParentProcessId;
            public uint HandleCount;
            public uint SessionId;
            public UIntPtr PageDirectoryBase;
            public UIntPtr PeakVirtualSize;
            public UIntPtr VirtualSize;
            public uint PageFaultCount;
            public UIntPtr PeakWorkingSetSize;
            public UIntPtr WorkingSetSize;
            public UIntPtr QuotaPeakPagedPoolUsage;
            public UIntPtr QuotaPagedPoolUsage;
            public UIntPtr QuotaPeakNonPagedPoolUsage;
            public UIntPtr QuotaNonPagedPoolUsage;
            public UIntPtr PagefileUsage;
            public UIntPtr PeakPagefileUsage;
            public UIntPtr publicPageCount;
            public long ReadOperationCount;
            public long WriteOperationCount;
            public long OtherOperationCount;
            public long ReadTransferCount;
            public long WriteTransferCount;
            public long OtherTransferCount;
        }

        //Query system process information
        private static IntPtr Query_SystemProcessInformation()
        {
            uint systemOffset = 0;
            IntPtr systemInfoBufferBegin = IntPtr.Zero;
            try
            {
                while (true)
                {
                    try
                    {
                        systemInfoBufferBegin = Marshal.AllocHGlobal((int)systemOffset);
                        uint queryResult = NtQuerySystemInformation(SYSTEM_INFO_CLASS.SystemProcessInformation, systemInfoBufferBegin, systemOffset, out uint systemLength);
                        if (queryResult == STATUS_INFO_LENGTH_MISMATCH)
                        {
                            systemOffset = Math.Max(systemOffset, systemLength);
                            SafeCloseMarshal(systemInfoBufferBegin);
                        }
                        else if (queryResult == 0)
                        {
                            break;
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return systemInfoBufferBegin;
        }

        //Get process thread information by process id
        public static List<ProcessThreadInfo> Get_ProcessThreadsByProcessId(int targetProcessId, bool firstThreadOnly)
        {
            List<ProcessThreadInfo> listProcessThread = new List<ProcessThreadInfo>();
            IntPtr systemInfoBufferQuery = IntPtr.Zero;
            try
            {
                //AVDebug.WriteLine("Getting process threads for process id: " + targetProcessId + "/" + firstThreadOnly);

                //Query process information
                systemInfoBufferQuery = Query_SystemProcessInformation();
                if (systemInfoBufferQuery == IntPtr.Zero)
                {
                    AVDebug.WriteLine("Failed getting all process threads: query failed.");
                    return listProcessThread;
                }

                //Loop process information
                long systemInfoOffsetLoop = systemInfoBufferQuery.ToInt64();
                while (true)
                {
                    try
                    {
                        //Read process information
                        IntPtr systemInfoBufferLoop = new IntPtr(systemInfoOffsetLoop);
                        SYSTEM_PROCESS_INFORMATION systemProcess = (SYSTEM_PROCESS_INFORMATION)Marshal.PtrToStructure(systemInfoBufferLoop, typeof(SYSTEM_PROCESS_INFORMATION));

                        //Check target process id
                        if (targetProcessId == systemProcess.UniqueProcessId.ToInt32())
                        {
                            //AVDebug.WriteLine("Found thread process: " + systemProcess.UniqueProcessId.ToInt32());

                            //Move to thread information
                            systemInfoBufferLoop = new IntPtr(systemInfoBufferLoop.ToInt64() + Marshal.SizeOf(typeof(SYSTEM_PROCESS_INFORMATION)));

                            //Read thread info
                            for (int i = 0; i < systemProcess.NumberOfThreads; i++)
                            {
                                try
                                {
                                    //Read thread information
                                    SYSTEM_THREAD_INFORMATION systemThread = (SYSTEM_THREAD_INFORMATION)Marshal.PtrToStructure(systemInfoBufferLoop, typeof(SYSTEM_THREAD_INFORMATION));

                                    //Add process thread to list
                                    ProcessThreadInfo processThread = new ProcessThreadInfo();
                                    processThread.Identifier = systemThread.UniqueThreadId.ToInt32();
                                    processThread.ThreadState = systemThread.ThreadState;
                                    processThread.ThreadWaitReason = systemThread.ThreadWaitReason;
                                    listProcessThread.Add(processThread);

                                    //Return process threads
                                    if (firstThreadOnly)
                                    {
                                        return listProcessThread;
                                    }

                                    //Move to next thread
                                    systemInfoBufferLoop = new IntPtr(systemInfoBufferLoop.ToInt64() + Marshal.SizeOf(typeof(SYSTEM_THREAD_INFORMATION)));
                                }
                                catch { }
                            }
                        }

                        //Move to next process
                        if (systemProcess.NextEntryOffset != 0)
                        {
                            systemInfoOffsetLoop += systemProcess.NextEntryOffset;
                        }
                        else
                        {
                            break;
                        }
                    }
                    catch { }
                }

                //Return process threads
                return listProcessThread;
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed getting all process threads: " + ex.Message);
                return listProcessThread;
            }
            finally
            {
                SafeCloseMarshal(systemInfoBufferQuery);
            }
        }

        //Get all running processes multi
        public static List<ProcessMulti> Get_AllProcessesMulti()
        {
            List<ProcessMulti> listProcessMulti = new List<ProcessMulti>();
            IntPtr systemInfoBufferQuery = IntPtr.Zero;
            try
            {
                //AVDebug.WriteLine("Getting all multi processes.");

                //Query process information
                systemInfoBufferQuery = Query_SystemProcessInformation();
                if (systemInfoBufferQuery == IntPtr.Zero)
                {
                    AVDebug.WriteLine("Failed getting all multi processes: query failed.");
                    return listProcessMulti;
                }

                //Loop process information
                long systemInfoOffsetLoop = systemInfoBufferQuery.ToInt64();
                while (true)
                {
                    try
                    {
                        //Read process information
                        IntPtr systemInfoBufferLoop = new IntPtr(systemInfoOffsetLoop);
                        SYSTEM_PROCESS_INFORMATION systemProcess = (SYSTEM_PROCESS_INFORMATION)Marshal.PtrToStructure(systemInfoBufferLoop, typeof(SYSTEM_PROCESS_INFORMATION));

                        //Add multi process to list
                        ProcessMulti processMulti = new ProcessMulti(systemProcess.UniqueProcessId.ToInt32(), systemProcess.ParentProcessId.ToInt32());
                        listProcessMulti.Add(processMulti);

                        //Move to next process
                        if (systemProcess.NextEntryOffset != 0)
                        {
                            systemInfoOffsetLoop += systemProcess.NextEntryOffset;
                        }
                        else
                        {
                            break;
                        }
                    }
                    catch { }
                }

                //Return processes
                return listProcessMulti;
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed getting all multi processes: " + ex.Message);
                return listProcessMulti;
            }
            finally
            {
                SafeCloseMarshal(systemInfoBufferQuery);
            }
        }
    }
}