using System;
using System.Diagnostics;
using Windows.ApplicationModel;
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

        public enum ProcessType : int
        {
            Unknown = -1,
            Win32 = 0,
            Win32Store = 1,
            UWP = 2,
        }

        //Classes
        public class ProcessMulti
        {
            //Process details
            public int Identifier { get; set; } = -1;
            public ProcessType Type { get; set; } = ProcessType.Unknown;
            public string Name { get; set; } = string.Empty;
            public string ExecutableName { get; set; } = string.Empty;
            public string Path { get; set; } = string.Empty;
            public string Argument { get; set; } = string.Empty;
            public string ClassName { get; set; } = string.Empty;
            public string WindowTitle { get; set; } = "Unknown";
            public IntPtr WindowHandle { get; set; } = IntPtr.Zero;
            public DateTime StartTime { get; set; } = DateTime.MinValue;

            //UWP details
            public Package AppPackage { get; set; } = null;
            public AppxDetails AppxDetails { get; set; } = null;

            //Process action
            public string Action { get; set; } = string.Empty;

            public ProcessThreadCollection ProcessThreads()
            {
                try
                {
                    return Process.GetProcessById(Identifier).Threads;
                }
                catch { }
                return null;
            }

            public TimeSpan ProcessRuntime()
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