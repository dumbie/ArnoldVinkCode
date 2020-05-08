using System;

namespace ArnoldVinkCode
{
    public partial class AVDisplayMonitor
    {
        public class DisplayMonitorResolution
        {
            public IntPtr ScreenHandle { get; set; } = IntPtr.Zero;
            public string ScreenName { get; set; } = string.Empty;
            public int ScreenWidth { get; set; } = -1;
            public int ScreenHeight { get; set; } = -1;
            public int ScreenDpiWidth { get; set; } = -1;
            public float ScreenDpiScale { get; set; } = -1;
            public int BoundsLeft { get; set; } = -1;
            public int BoundsTop { get; set; } = -1;
            public int BoundsRight { get; set; } = -1;
            public int BoundsBottom { get; set; } = -1;
        }
    }
}