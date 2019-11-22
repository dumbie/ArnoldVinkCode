using System;
using System.Diagnostics;

namespace ArnoldVinkCode
{
    public partial class ProcessClasses
    {
        public enum ProcessType
        {
            Win32,
            Win32Store,
            UWP
        }

        public class ProcessMulti
        {
            public ProcessType Type = ProcessType.Win32;
            public string Status = string.Empty;
            public string AppUserModelId = string.Empty;
            public IntPtr WindowHandle = IntPtr.Zero;
            public int ProcessId = -1;
            public ProcessThreadCollection ProcessThreads = null;
        }

        public class ProcessFocus
        {
            public string Title = "Unknown";
            public string ClassName = "Unknown";
            public IntPtr WindowHandle = IntPtr.Zero;
            public Process Process = null;
        }
    }
}