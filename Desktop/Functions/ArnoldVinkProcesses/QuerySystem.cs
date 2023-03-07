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

        //Enumerators
        public enum THREAD_STATE
        {
            Initialized,
            Ready,
            Running,
            Standby,
            Terminated,
            Wait,
            Transition,
            Unknown
        }

        public enum THREAD_WAIT_REASON
        {
            Executive,
            FreePage,
            PageIn,
            PoolAllocation,
            DelayExecution,
            Suspended,
            UserRequest,
            Unknown
        }

        //Structures
        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_UNICODE_STRING
        {
            public ushort Length;
            public ushort MaximumLength;
            public IntPtr Buffer;
        }

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
            public uint Priority;
            public uint BasePriority;
            public uint ContextSwitchCount;
            public THREAD_STATE ThreadState;
            public THREAD_WAIT_REASON ThreadWaitReason;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_PROCESS_INFORMATION
        {
            public uint NextEntryOffset;
            public uint NumberOfThreads;
            public uint WorkingSetPrivateSize;
            public uint HardFaultCount;
            public uint NumberOfThreadsHighWatermark;
            public long CycleTime;
            public long CreateTime;
            public long UserTime;
            public long KernelTime;
            public SYSTEM_UNICODE_STRING ImageName;
            public uint BasePriority;
            public IntPtr UniqueProcessId;
            public IntPtr ParentProcessId;
            public uint HandleCount;
            public uint SessionId;
            public IntPtr UniqueProcessKey;
            public IntPtr PeakVirtualSize;
            public IntPtr VirtualSize;
            public uint PageFaultCount;
            public IntPtr PeakWorkingSetSize;
            public IntPtr WorkingSetSize;
            public IntPtr QuotaPeakPagedPoolUsage;
            public IntPtr QuotaPagedPoolUsage;
            public IntPtr QuotaPeakNonPagedPoolUsage;
            public IntPtr QuotaNonPagedPoolUsage;
            public IntPtr PagefileUsage;
            public IntPtr PeakPagefileUsage;
            public IntPtr PrivatePageCount;
            public long ReadOperationCount;
            public long WriteOperationCount;
            public long OtherOperationCount;
            public long ReadTransferCount;
            public long WriteTransferCount;
            public long OtherTransferCount;
        }

        //Get process thread information by process id
        public static List<ProcessThread> Get_ProcessThreadsByProcessId(int targetProcessId, bool firstThreadOnly)
        {
            //Fix write query system code
            return null;
        }

        public static List<ProcessMulti> Get_AllProcessesMulti()
        {
            List<ProcessMulti> listProcessMulti = new List<ProcessMulti>();
            IntPtr systemInfoBufferBegin = IntPtr.Zero;
            IntPtr systemInfoBufferSeek = IntPtr.Zero;
            try
            {
                AVDebug.WriteLine("Getting all multi processes.");

                //Query process information
                uint systemOffset = 0;
                uint systemLength = 0;
                while (true)
                {
                    systemInfoBufferBegin = Marshal.AllocHGlobal((int)systemOffset);
                    uint queryResult = NtQuerySystemInformation(SYSTEM_INFO_CLASS.SystemProcessInformation, systemInfoBufferBegin, systemOffset, out systemLength);
                    if (queryResult == 3221225476)
                    {
                        systemOffset = Math.Max(systemOffset, systemLength);
                        AVDebug.WriteLine("System information offset begin: " + systemOffset);
                    }
                    else if (queryResult == 0)
                    {
                        break;
                    }
                    else
                    {
                        AVDebug.WriteLine("Failed getting all multi processes: query failed.");
                        return listProcessMulti;
                    }
                }

                //Loop process information
                systemOffset = 0;
                while (true)
                {
                    try
                    {
                        //Read process information
                        systemInfoBufferSeek = new IntPtr(systemInfoBufferBegin.ToInt64() + systemOffset);
                        SYSTEM_PROCESS_INFORMATION systemProcess = (SYSTEM_PROCESS_INFORMATION)Marshal.PtrToStructure(systemInfoBufferSeek, typeof(SYSTEM_PROCESS_INFORMATION));

                        //Create multi process
                        ProcessMulti processMulti = new ProcessMulti();
                        processMulti.Identifier = systemProcess.UniqueProcessId.ToInt32();
                        processMulti.IdentifierParent = systemProcess.ParentProcessId.ToInt32();
                        processMulti.StartTime = DateTime.FromFileTime(systemProcess.CreateTime);

                        ////Move to thread information
                        //systemInfoBufferSeek = new IntPtr(systemInfoBufferSeek.ToInt64() + Marshal.SizeOf(typeof(SYSTEM_PROCESS_INFORMATION)));

                        ////Read thread info
                        //for (int i = 0; i < systemProcess.NumberOfThreads; i++)
                        //{
                        //    SYSTEM_THREAD_INFORMATION systemThread = (SYSTEM_THREAD_INFORMATION)Marshal.PtrToStructure(systemInfoBufferSeek, typeof(SYSTEM_THREAD_INFORMATION));
                        //    AVDebug.WriteLine("Thread ID: " + systemThread.UniqueThreadId);
                        //    AVDebug.WriteLine("Thread State: " + systemThread.ThreadState);
                        //    AVDebug.WriteLine("Thread WaitReason: " + systemThread.ThreadWaitReason);

                        //    //Move to next thread
                        //    systemInfoBufferSeek = new IntPtr(systemInfoBufferSeek.ToInt64() + Marshal.SizeOf(typeof(SYSTEM_THREAD_INFORMATION)));
                        //}

                        //Add multi process to list
                        listProcessMulti.Add(processMulti);

                        //Move to next process
                        if (systemProcess.NextEntryOffset != 0)
                        {
                            systemOffset += systemProcess.NextEntryOffset;
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
                AVDebug.WriteLine("Failed to get all multi processes: " + ex.Message);
                return listProcessMulti;
            }
            finally
            {
                CloseHandleAuto(systemInfoBufferBegin);
                CloseHandleAuto(systemInfoBufferSeek);
            }
        }
    }
}