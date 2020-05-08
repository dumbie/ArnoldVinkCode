using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ArnoldVinkCode
{
    public partial class AVDisplayMonitor
    {
        public static int GetScreenRefreshRate(int screenNumber)
        {
            int refreshRate = -1;
            try
            {
                IntPtr createDC = CreateDC(@"\\.\DISPLAY" + screenNumber, null, null, IntPtr.Zero);
                refreshRate = GetDeviceCaps(createDC, DEVICECAP.VREFRESH);
                ReleaseDC(IntPtr.Zero, createDC);
            }
            catch { }
            return refreshRate;
        }

        private static List<DisplayMonitorResolution> GetScreenHandles()
        {
            List<DisplayMonitorResolution> screenHandles = new List<DisplayMonitorResolution>();
            try
            {
                EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
                delegate (IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
                {
                    DisplayMonitorResolution displayMonitorResolution = new DisplayMonitorResolution();
                    displayMonitorResolution.ScreenHandle = hMonitor;
                    screenHandles.Add(displayMonitorResolution);
                    return true;
                },
                IntPtr.Zero);
            }
            catch { }
            return screenHandles;
        }

        //Requires app manifest dpiawareness permonitor
        public static DisplayMonitorResolution GetScreenResolutionBounds(int screenNumber)
        {
            DisplayMonitorResolution displayMonitorResolution = null;
            try
            {
                //Get the screen handle
                if (screenNumber <= 0) { screenNumber = 1; }
                List<DisplayMonitorResolution> screenHandles = GetScreenHandles();
                try
                {
                    displayMonitorResolution = screenHandles[screenNumber - 1];
                }
                catch
                {
                    displayMonitorResolution = screenHandles.FirstOrDefault();
                }

                //Get the screen dpi
                GetDpiForMonitor(displayMonitorResolution.ScreenHandle, DPITYPE.Effective, out int dpiWidth, out int dpiHeight);
                displayMonitorResolution.ScreenDpiScale = dpiWidth / (float)96;

                //Get the screen bounds
                //Improve get total dpi difference and substract
                MONITORINFOEX monitorInfoEx = new MONITORINFOEX();
                GetMonitorInfo(displayMonitorResolution.ScreenHandle, monitorInfoEx);
                displayMonitorResolution.BoundsLeft = monitorInfoEx.rcMonitor.Left;
                displayMonitorResolution.BoundsTop = monitorInfoEx.rcMonitor.Top;
                displayMonitorResolution.BoundsRight = monitorInfoEx.rcMonitor.Right;
                displayMonitorResolution.BoundsBottom = monitorInfoEx.rcMonitor.Bottom;

                //Get the screen resolution
                displayMonitorResolution.ScreenWidth = (int)(monitorInfoEx.rcMonitor.Width() / (float)dpiWidth * (float)96);
                displayMonitorResolution.ScreenHeight = (int)(monitorInfoEx.rcMonitor.Height() / (float)dpiWidth * (float)96);

                return displayMonitorResolution;
            }
            catch
            {
                Debug.WriteLine("Failed getting monitor resolution.");
            }
            return displayMonitorResolution;
        }
    }
}