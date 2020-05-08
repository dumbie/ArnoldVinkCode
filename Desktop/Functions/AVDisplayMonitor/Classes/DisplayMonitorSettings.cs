using System;

namespace ArnoldVinkCode
{
    public partial class AVDisplayMonitor
    {
        public class DisplayMonitorSettings
        {
            public IntPtr Handle { get; set; } = IntPtr.Zero;
            public string Name { get; set; } = string.Empty;
            public int WidthNative { get; set; } = -1;
            public int WidthDpi { get; set; } = -1;
            public int HeightNative { get; set; } = -1;
            public int HeightDpi { get; set; } = -1;
            public float DpiScaleHorizontal { get; set; } = -1;
            public float DpiScaleVertical { get; set; } = -1;
            public int RefreshRate { get; set; } = -1;
            public int BitDepth { get; set; } = -1;
            public int BoundsLeft { get; set; } = -1;
            public int BoundsTop { get; set; } = -1;
            public int BoundsRight { get; set; } = -1;
            public int BoundsBottom { get; set; } = -1;
        }
    }
}