using System;
using System.Diagnostics;

namespace ArnoldVinkCode
{
    public partial class ProcessClasses
    {
        public class ProcessMultipleCheck
        {
            public string Status = string.Empty;
            public Process Process = null;
            public ProcessUwp ProcessUwp = null;
        }

        public class ProcessUwp
        {
            public string AppUserModelId = string.Empty;
            public IntPtr WindowHandle = IntPtr.Zero;
            public int ProcessId = -1;
        }

        public class ProcessFocus
        {
            public string Title = "Unknown";
            public IntPtr WindowHandle = IntPtr.Zero;
            public Process Process = null;
        }
    }
}