using System;

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
            public string Title { get; set; } = "Unknown";
            public string Name { get; set; } = string.Empty;
            public string ExecutableName { get; set; } = string.Empty;
            public string Path { get; set; } = string.Empty;
            public int Identifier { get; set; } = -1;
            public string Argument { get; set; } = string.Empty;
            public ProcessType Type { get; set; } = ProcessType.Unknown;
            public IntPtr WindowHandle { get; set; } = IntPtr.Zero;
            public string ClassName { get; set; } = string.Empty;
            public string Action { get; set; } = string.Empty;
        }
    }
}