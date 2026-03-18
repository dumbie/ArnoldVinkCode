using System;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Imports
        [DllImport("ntdll.dll")]
        public static extern uint NtQuerySystemInformation(SystemInfoClass SystemInformationClass, IntPtr SystemInformation, uint SystemInformationLength, out uint ReturnLength);

        //Constants
        public const uint STATUS_INFO_LENGTH_MISMATCH = 0xC0000004;
        public const uint STATUS_SUCCESS = 0x00000000;

        //Structures
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct UNICODE_STRING
        {
            public ushort Length;
            public ushort MaximumLength;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string Buffer;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_THREAD_INFORMATION
        {
            public long KernelTime;
            public long UserTime;
            public long CreateTime;
            public ulong WaitTime;
            public IntPtr StartAddress;
            public IntPtr ProcessId;
            public IntPtr ThreadId;
            public int Priority;
            public int BasePriority;
            public ulong ContextSwitches;
            public ProcessThreadState ThreadState;
            public ProcessThreadWaitReason WaitReason;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_PROCESS_INFORMATION
        {
            public int NextEntryOffset;
            public int NumberOfThreads;
            public ulong WorkingSetPrivateSize;
            public int HardFaultCount;
            public int NumberOfThreadsHighWatermark;
            public ulong CycleTime;
            public ulong CreateTime;
            public ulong UserTime;
            public ulong KernelTime;
            public UNICODE_STRING ImageName;
            public ProcessBasePriority BasePriority;
            public IntPtr UniqueProcessId;
            public IntPtr ParentProcessId;
            public int HandleCount;
            public int SessionId;
            public IntPtr PageDirectoryBase;
            public IntPtr PeakVirtualSize;
            public IntPtr VirtualSize;
            public int PageFaultCount;
            public IntPtr PeakWorkingSetSize;
            public IntPtr WorkingSetSize;
            public IntPtr QuotaPeakPagedPoolUsage;
            public IntPtr QuotaPagedPoolUsage;
            public IntPtr QuotaPeakNonPagedPoolUsage;
            public IntPtr QuotaNonPagedPoolUsage;
            public IntPtr PagefileUsage;
            public IntPtr PeakPagefileUsage;
            public IntPtr PrivatePageCount;
            public ulong ReadOperationCount;
            public ulong WriteOperationCount;
            public ulong OtherOperationCount;
            public ulong ReadTransferCount;
            public ulong WriteTransferCount;
            public ulong OtherTransferCount;
        }

        //Query system process information
        private static IntPtr Query_SystemProcessInformation()
        {
            uint systemOffset = 0;
            try
            {
                while (true)
                {
                    try
                    {
                        IntPtr systemInfo = Marshal.AllocHGlobal((int)systemOffset);
                        uint queryResult = NtQuerySystemInformation(SystemInfoClass.SystemProcessInformation, systemInfo, systemOffset, out uint systemLength);
                        if (queryResult == STATUS_INFO_LENGTH_MISMATCH)
                        {
                            systemOffset = Math.Max(systemOffset, systemLength);
                            if (systemInfo != IntPtr.Zero)
                            {
                                Marshal.FreeHGlobal(systemInfo);
                            }
                        }
                        else if (queryResult == STATUS_SUCCESS)
                        {
                            return systemInfo;
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return IntPtr.Zero;
        }
    }
}