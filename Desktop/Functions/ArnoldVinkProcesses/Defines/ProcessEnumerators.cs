namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        public enum PROCESS_PARAMETER_OPTIONS
        {
            CurrentDirectoryPath,
            ImagePathName,
            CommandLine,
            DesktopInfo,
            Environment
        }

        public enum PROCESS_INFO_CLASS : int
        {
            ProcessBasicInformation = 0,
            ProcessWow64Information = 26
        }

        public enum ProcessThreadState : int
        {
            Initialized,
            Ready,
            Running,
            Standby,
            Terminated,
            Waiting,
            Transition,
            Unknown
        }

        public enum ProcessThreadWaitReason : int
        {
            Executive,
            FreePage,
            PageIn,
            SystemAllocation,
            ExecutionDelay,
            Suspended,
            UserRequest,
            EventPairHigh,
            EventPairLow,
            LpcReceive,
            LpcReply,
            VirtualMemory,
            PageOut,
            Unknown
        }

        public enum ProcessType : int
        {
            Unknown = -1,
            Win32 = 0,
            Win32Store = 1,
            UWP = 2
        }
    }
}