using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVSwitchDisplayMonitor
    {
        public class DisplayMonitorSummary
        {
            public uint Id = 0;
            public string Name = string.Empty;
        }

        public static bool EnableMonitorFirst()
        {
            try
            {
                int error = SetDisplayConfig(0, null, 0, null, (uint)SDC.SDC_APPLY | (uint)SDC.SDC_TOPOLOGY_INTERNAL);
                if (error != 0)
                {
                    Debug.WriteLine("Failed SetDisplayConfig: " + error);
                    return false;
                }
                else
                {
                    Debug.WriteLine("Adjusted SetDisplayConfig: " + error);
                    return true;
                }
            }
            catch { }
            return false;
        }

        public static bool EnableMonitorSecond()
        {
            try
            {
                int error = SetDisplayConfig(0, null, 0, null, (uint)SDC.SDC_APPLY | (uint)SDC.SDC_TOPOLOGY_EXTERNAL);
                if (error != 0)
                {
                    Debug.WriteLine("Failed SetDisplayConfig: " + error);
                    return false;
                }
                else
                {
                    Debug.WriteLine("Adjusted SetDisplayConfig: " + error);
                    return true;
                }
            }
            catch { }
            return false;
        }

        public static bool EnableMonitorCloneMode()
        {
            try
            {
                int error = SetDisplayConfig(0, null, 0, null, (uint)SDC.SDC_APPLY | (uint)SDC.SDC_TOPOLOGY_CLONE);
                if (error != 0)
                {
                    Debug.WriteLine("Failed SetDisplayConfig: " + error);
                    return false;
                }
                else
                {
                    Debug.WriteLine("Adjusted SetDisplayConfig: " + error);
                    return true;
                }
            }
            catch { }
            return false;
        }

        public static bool EnableMonitorExtendMode()
        {
            try
            {
                int error = SetDisplayConfig(0, null, 0, null, (uint)SDC.SDC_APPLY | (uint)SDC.SDC_TOPOLOGY_EXTEND);
                if (error != 0)
                {
                    Debug.WriteLine("Failed SetDisplayConfig: " + error);
                    return false;
                }
                else
                {
                    Debug.WriteLine("Adjusted SetDisplayConfig: " + error);
                    return true;
                }
            }
            catch { }
            return false;
        }

        private static string MonitorFriendlyName(LUID adapterId, uint targetId)
        {
            try
            {
                DISPLAYCONFIG_TARGET_DEVICE_NAME deviceName = new DISPLAYCONFIG_TARGET_DEVICE_NAME();
                deviceName.header.size = (uint)Marshal.SizeOf(typeof(DISPLAYCONFIG_TARGET_DEVICE_NAME));
                deviceName.header.adapterId = adapterId;
                deviceName.header.id = targetId;
                deviceName.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME;

                int error = DisplayConfigGetDeviceInfo(ref deviceName);
                if (error != 0)
                {
                    Debug.WriteLine("Failed MonitorFriendlyName: " + error);
                    return string.Empty;
                }

                return deviceName.monitorFriendlyDeviceName;
            }
            catch
            {
                Debug.WriteLine("Failed reading the friendly monitor name.");
                return string.Empty;
            }
        }

        ////Fix: code does not work when monitor is in extend or clone mode.
        //public static List<DisplayMonitorSummary> ListDisplayMonitors()
        //{
        //    try
        //    {
        //        List<DisplayMonitorSummary> monitorListSummary = new List<DisplayMonitorSummary>();

        //        uint displayPathCount = 0;
        //        uint displayModeCount = 0;
        //        int error = GetDisplayConfigBufferSizes(QUERY_DEVICE_CONFIG_FLAGS.QDC_ALL_PATHS, out displayPathCount, out displayModeCount);
        //        if (error != 0)
        //        {
        //            Debug.WriteLine("Failed GetDisplayConfigBufferSizes: " + error);
        //            return null;
        //        }

        //        DISPLAYCONFIG_PATH_INFO[] DisplayPaths = new DISPLAYCONFIG_PATH_INFO[displayPathCount];
        //        DISPLAYCONFIG_MODE_INFO[] DisplayModes = new DISPLAYCONFIG_MODE_INFO[displayModeCount];
        //        error = QueryDisplayConfig(QUERY_DEVICE_CONFIG_FLAGS.QDC_ALL_PATHS, ref displayPathCount, DisplayPaths, ref displayModeCount, DisplayModes, IntPtr.Zero);
        //        if (error != 0)
        //        {
        //            Debug.WriteLine("Failed QueryDisplayConfig: " + error);
        //            return null;
        //        }

        //        int pathInfoIndex = 0;
        //        foreach (DISPLAYCONFIG_PATH_INFO pathInfo in DisplayPaths)
        //        {
        //            try
        //            {
        //                if (pathInfo.targetInfo.targetAvailable)
        //                {
        //                    if (pathInfo.sourceInfo.modeInfoIdx < displayModeCount || pathInfo.sourceInfo.modeInfoIdx == 0)
        //                    {
        //                        if (pathInfo.targetInfo.modeInfoIdx > displayModeCount || pathInfo.targetInfo.modeInfoIdx == 0)
        //                        {
        //                            uint monitorId = pathInfo.targetInfo.id;
        //                            string monitorName = MonitorFriendlyName(pathInfo.targetInfo.adapterId, monitorId);
        //                            monitorName = monitorName + " (" + monitorId + ")";

        //                            if (pathInfo.targetInfo.id == 4354)
        //                            {
        //                                DisplayPaths[pathInfoIndex].flags = (uint)DISPLAYCONFIG_PATH.DISPLAYCONFIG_PATH_ACTIVE;
        //                            }
        //                            else
        //                            {
        //                                DisplayPaths[pathInfoIndex].flags = (uint)DISPLAYCONFIG_PATH.DISPLAYCONFIG_PATH_DISABLE;
        //                            }

        //                            //Add monitor to summary list
        //                            monitorListSummary.Add(new DisplayMonitorSummary() { Id = monitorId, Name = monitorName });
        //                        }
        //                    }
        //                }
        //            }
        //            catch { }
        //            pathInfoIndex++;
        //        }

        //        //Set display information
        //        error = SetDisplayConfig(displayPathCount, DisplayPaths, displayModeCount, DisplayModes, (uint)SDC.SDC_APPLY | (uint)SDC.SDC_USE_SUPPLIED_DISPLAY_CONFIG | (uint)SDC.SDC_SAVE_TO_DATABASE | (uint)SDC.SDC_ALLOW_CHANGES);
        //        if (error != 0)
        //        {
        //            Debug.WriteLine("Failed SetDisplayConfig: " + error);
        //            return null;
        //        }
        //        else
        //        {
        //            Debug.WriteLine("Adjusted SetDisplayConfig: " + error);
        //            return null;
        //        }
        //    }
        //    catch
        //    {
        //        Debug.WriteLine("Failed loading the displays list.");
        //        return null;
        //    }
        //}
    }
}