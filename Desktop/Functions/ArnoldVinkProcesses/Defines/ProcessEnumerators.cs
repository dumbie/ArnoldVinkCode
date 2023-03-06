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
            ProcessBasicInformation = 0x00,
            ProcessWow64Information = 0x26
        }

        public enum THREAD_INFO_CLASS : int
        {
            ThreadSystemThreadInformation = 0x28
        }

        public enum ProcessThreadState : int
        {
            Initialized = 0,
            Ready = 1,
            Running = 2,
            Standby = 3,
            Terminated = 4,
            Waiting = 5,
            Transition = 6,
            DeferredReady = 7,
            GateWait = 8,
            WaitingForProcessInSwap = 9,
            Unknown = 10
        }

        public enum ProcessThreadWaitReason : int
        {
            Executive = 0,
            FreePage = 1,
            PageIn = 2,
            PoolAllocation = 3,
            DelayExecution = 4,
            Suspended = 5,
            UserRequest = 6,
            Unknown = 7
        }

        public enum ProcessType : int
        {
            Unknown = 0,
            Win32 = 1,
            Win32Store = 2,
            UWP = 3
        }
    }
}