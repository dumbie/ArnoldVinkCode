using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ArnoldVinkCode
{
    public partial class AVDisplayMonitor
    {
        private static List<DisplayMonitorSettings> GetScreenHandles()
        {
            List<DisplayMonitorSettings> screenSettingsList = new List<DisplayMonitorSettings>();
            try
            {
                EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
                delegate (IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
                {
                    DisplayMonitorSettings displayMonitorSettings = new DisplayMonitorSettings();
                    displayMonitorSettings.Handle = hMonitor;
                    screenSettingsList.Add(displayMonitorSettings);
                    return true;
                },
                IntPtr.Zero);
            }
            catch { }
            return screenSettingsList;
        }

        //Requires app manifest dpiawareness permonitor
        public static DisplayMonitorSettings GetScreenSettings(int screenNumber)
        {
            DisplayMonitorSettings displayMonitorSettings = null;
            try
            {
                //Get the screen handle
                if (screenNumber <= 0) { screenNumber = 1; }
                List<DisplayMonitorSettings> screenHandles = GetScreenHandles();
                try
                {
                    displayMonitorSettings = screenHandles[screenNumber - 1];
                }
                catch
                {
                    displayMonitorSettings = screenHandles.FirstOrDefault();
                }

                //Get the screen dpi scale
                GetDpiForMonitor(displayMonitorSettings.Handle, DPITYPE.Effective, out int dpiHorizontal, out int dpiVertical);
                displayMonitorSettings.DpiScaleHorizontal = (float)dpiHorizontal / (float)96;
                displayMonitorSettings.DpiScaleVertical = (float)dpiVertical / (float)96;

                MONITORINFOEX monitorInfoEx = new MONITORINFOEX();
                GetMonitorInfo(displayMonitorSettings.Handle, monitorInfoEx);

                //Get the screen name
                displayMonitorSettings.Name = new string(monitorInfoEx.szDevice).TrimEnd((char)0);

                //Get the screen resolution
                displayMonitorSettings.WidthNative = monitorInfoEx.rcMonitor.Width;
                displayMonitorSettings.WidthDpi = (int)(displayMonitorSettings.WidthNative / (float)dpiHorizontal * (float)96);
                displayMonitorSettings.HeightNative = monitorInfoEx.rcMonitor.Height;
                displayMonitorSettings.HeightDpi = (int)(displayMonitorSettings.HeightNative / (float)dpiVertical * (float)96);

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
            }
            catch
            {
                Debug.WriteLine("Failed getting monitor settings.");
            }
            return displayMonitorSettings;
        }
    }
}