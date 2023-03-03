using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVUwpAppx;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Enumerators
        public enum PROCESS_PARAMETER_OPTIONS
        {
            CurrentDirectoryPath,
            ImagePathName,
            CommandLine,
            DesktopInfo,
            Environment
        };

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
            UWP = 2,
        }

        //Classes
        public class ProcessAccess
        {
            public bool UiAccess { get; set; } = false;
            public bool AdminAccess { get; set; } = false;
            public bool Elevation { get; set; } = false;
            public TOKEN_ELEVATION_TYPE ElevationType { get; set; } = TOKEN_ELEVATION_TYPE.TokenElevationTypeDefault;
        };

        public class ProcessHandle
        {
            public int Identifier { get; set; } = -1;
            public int ParentIdentifier { get; set; } = -1;
            public IntPtr Handle { get; set; } = IntPtr.Zero;
        };

        public class ProcessMulti
        {
            //Process handle
            public int Identifier { get; set; } = -1;
            public int ParentIdentifier { get; set; } = -1;
            public IntPtr Handle { get; set; } = IntPtr.Zero;

            //Process details
            public ProcessType Type { get; set; } = ProcessType.Unknown;
            public string AppUserModelId { get; set; } = string.Empty;
            public string ExeName { get; set; } = string.Empty;
            public string ExeNameNoExt { get; set; } = string.Empty;
            public string ExePath { get; set; } = string.Empty;
            public string WorkPath { get; set; } = string.Empty;
            public string Argument { get; set; } = string.Empty;
            public string WindowClassName { get; set; } = string.Empty;
            public string WindowTitle { get; set; } = "Unknown";
            public IntPtr WindowHandle { get; set; } = IntPtr.Zero;
            public DateTime StartTime { get; set; } = DateTime.MinValue;

            //UWP details
            public Package AppPackage { get; set; } = null;
            public AppxDetails AppxDetails { get; set; } = null;

            //Process action
            public string Action { get; set; } = string.Empty;

            //Multi functions
            public bool SetProcessPriority(PriorityClass priorityClass)
            {
                try
                {
                    return SetPriorityClass(Handle, priorityClass);
                }
                catch { }
                return false;
            }

            public List<int> GetProcessThreads()
            {
                try
                {
                    return Thread_GetProcessThreadIds(Identifier);
                }
                catch { }
                return null;
            }

            public TimeSpan GetProcessRuntime()
            {
                try
                {
                    return DateTime.Now.Subtract(StartTime);
                }
                catch { }
                return TimeSpan.Zero;
            }
        }
    }
}