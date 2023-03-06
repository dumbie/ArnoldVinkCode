using System;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Imports
        [DllImport("ntdll.dll", EntryPoint = "NtQueryInformationThread")]
        public static extern int NtQueryInformationThread32(IntPtr ThreadHandle, THREAD_INFO_CLASS ThreadInformationClass, ref SYSTEM_THREAD_INFORMATION32 ThreadInformation, uint ThreadInformationLength, out uint ReturnLength);

        //Structures
        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_THREAD_INFORMATION32
        {
            public long KernelTime;
            public long UserTime;
            public long CreateTime;
            public uint WaitTime;
            public IntPtr StartAddress;
            public IntPtr ClientIdUniqueProcess;
            public IntPtr ClientIdUniqueThread;
            public int Priority;
            public int BasePriority;
            public uint ContextSwitches;
            public ProcessThreadState ThreadState;
            public ProcessThreadWaitReason ThreadWaitReason;
        }
    }
}