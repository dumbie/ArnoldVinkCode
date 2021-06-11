using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ArnoldVinkCode
{
    public partial class AVDisplayMonitor
    {
        //Query all monitors
        private static List<IntPtr> QueryMonitorsEnumDisplay()
        {
            List<IntPtr> monitorHandles = new List<IntPtr>();
            try
            {
                EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
                delegate (IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
                {
                    monitorHandles.Add(hMonitor);
                    return true;
                },
                IntPtr.Zero);
            }
            catch { }
            return monitorHandles;
        }

        //Get monitor information
        public static DisplayMonitor MonitorEnumDisplay(int screenNumber)
        {

            try
            {
                //Check screen number
                if (screenNumber <= 0) { screenNumber = 1; }

                //Query all monitors
                List<IntPtr> screenHandles = QueryMonitorsEnumDisplay();
                IntPtr screenHandle = screenHandles[screenNumber - 1];

                //Create display monitor
                DisplayMonitor displayMonitorSettings = new DisplayMonitor();
                displayMonitorSettings.Identifier = screenNumber;

                //Get the screen dpi scale
                GetDpiForMonitor(screenHandle, DPITYPE.Effective, out int dpiHorizontal, out int dpiVertical);
                displayMonitorSettings.DpiScaleHorizontal = (float)dpiHorizontal / (float)96;
                displayMonitorSettings.DpiScaleVertical = (float)dpiVertical / (float)96;

                MONITORINFOEX monitorInfoEx = new MONITORINFOEX();
                GetMonitorInfo(screenHandle, monitorInfoEx);

                //Get the screen name
                displayMonitorSettings.Name = new string(monitorInfoEx.szDevice).TrimEnd((char)0);

                //Get the device path
                displayMonitorSettings.DevicePath = @"\\.\DISPLAY" + screenNumber;

                //Get the screen resolution
                displayMonitorSettings.WidthNative = monitorInfoEx.rcMonitor.Width;
                displayMonitorSettings.WidthDpi = (int)(displayMonitorSettings.WidthNative / displayMonitorSettings.DpiScaleHorizontal);
                displayMonitorSettings.HeightNative = monitorInfoEx.rcMonitor.Height;
                displayMonitorSettings.HeightDpi = (int)(displayMonitorSettings.HeightNative / displayMonitorSettings.DpiScaleVertical);

                //Get the screen bounds
                displayMonitorSettings.BoundsLeft = monitorInfoEx.rcMonitor.Left;
                displayMonitorSettings.BoundsTop = monitorInfoEx.rcMonitor.Top;
                displayMonitorSettings.BoundsRight = monitorInfoEx.rcMonitor.Right;
                displayMonitorSettings.BoundsBottom = monitorInfoEx.rcMonitor.Bottom;

                IntPtr createDC = CreateDC(displayMonitorSettings.Name, null, null, IntPtr.Zero);

                //Get the screen refresh rate
                displayMonitorSettings.RefreshRate = GetDeviceCaps(createDC, DEVICECAP.VREFRESH);

                //Get the screen bit depth
                displayMonitorSettings.BitDepth = GetDeviceCaps(createDC, DEVICECAP.BITSPIXEL);

                ReleaseDC(IntPtr.Zero, createDC);
                return displayMonitorSettings;
            }
            catch
            {
                Debug.WriteLine("Failed getting enumdisplay monitor information.");
                return null;
            }
        }
    }
}