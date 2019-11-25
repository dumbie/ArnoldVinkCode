using System;
using System.Diagnostics;

namespace ArnoldVinkCode
{
    public partial class ProcessClasses
    {
        public enum ProcessType : int
        {
            Unknown = -1,
            Win32 = 0,
            Win32Store = 1,
            UWP = 2,
        }

        public class ProcessMulti
        {
            public int Identifier { get; set; } = -1;
            public int Count { get; set; } = 0;
            public string Argument { get; set; } = string.Empty;
            public ProcessThreadCollection Threads { get; set; } = null;
            public ProcessType Type { get; set; } = ProcessType.Unknown;
            public IntPtr WindowHandle { get; set; } = IntPtr.Zero;
            public string Action { get; set; } = string.Empty;
        }

        public class ProcessFocus
        {
            public string Title { get; set; } = "Unknown";
            public string ClassName { get; set; } = "Unknown";
            public IntPtr WindowHandle { get; set; } = IntPtr.Zero;
            public Process Process { get; set; } = null;
        }
    }
}