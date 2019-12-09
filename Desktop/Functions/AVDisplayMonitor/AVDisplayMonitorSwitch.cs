using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVDisplayMonitor
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
                int error = SetDisplayConfig(0, null, 0, null, (uint)SetDisplayConfig_Flags.SDC_APPLY | (uint)SetDisplayConfig_Flags.SDC_TOPOLOGY_INTERNAL);
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
                int error = SetDisplayConfig(0, null, 0, null, (uint)SetDisplayConfig_Flags.SDC_APPLY | (uint)SetDisplayConfig_Flags.SDC_TOPOLOGY_EXTERNAL);
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
                int error = SetDisplayConfig(0, null, 0, null, (uint)SetDisplayConfig_Flags.SDC_APPLY | (uint)SetDisplayConfig_Flags.SDC_TOPOLOGY_CLONE);
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
                int error = SetDisplayConfig(0, null, 0, null, (uint)SetDisplayConfig_Flags.SDC_APPLY | (uint)SetDisplayConfig_Flags.SDC_TOPOLOGY_EXTEND);
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
                    return "Unknown";
                }

                string friendlyName = deviceName.monitorFriendlyDeviceName;
                if (!string.IsNullOrWhiteSpace(friendlyName))
                {
                    return deviceName.monitorFriendlyDeviceName;
                }
                else
                {
                    return "Unknown";
                }
            }
            catch
            {
                Debug.WriteLine("Failed reading the friendly monitor name.");
                return "Unknown";
            }
        }

        //List all the connected display monitors
        public static List<DisplayMonitorSummary> ListDisplayMonitors()
        {
            try
            {
                uint displayPathCount = 0;
                uint displayModeCount = 0;
                int error = GetDisplayConfigBufferSizes(QUERY_DEVICE_CONFIG_FLAGS.QDC_ALL_PATHS, out displayPathCount, out displayModeCount);
                if (error != 0)
                {
                    Debug.WriteLine("Failed GetDisplayConfigBufferSizes: " + error);
                    return null;
                }

                DISPLAYCONFIG_PATH_INFO[] displayPaths = new DISPLAYCONFIG_PATH_INFO[displayPathCount];
                DISPLAYCONFIG_MODE_INFO[] displayModes = new DISPLAYCONFIG_MODE_INFO[displayModeCount];
                error = QueryDisplayConfig(QUERY_DEVICE_CONFIG_FLAGS.QDC_ALL_PATHS, ref displayPathCount, displayPaths, ref displayModeCount, displayModes, DISPLAYCONFIG_TOPOLOGY_ID.DISPLAYCONFIG_TOPOLOGY_NONE);
                if (error != 0)
                {
                    Debug.WriteLine("Failed QueryDisplayConfig: " + error);
                    return null;
                }

                List<DisplayMonitorSummary> monitorListSummary = new List<DisplayMonitorSummary>();

                uint prevMonitorId = 0;
                int pathInfoIndex = 0;
                int validationId = 100000;
                foreach (DISPLAYCONFIG_PATH_INFO pathInfo in displayPaths)
                {
                    try
                    {
                        if (pathInfo.targetInfo.targetAvailable)
                        {
                            uint monitorId = pathInfo.targetInfo.id;
                            if (!monitorListSummary.Any(x => x.Id == monitorId))
                            {
                                //Check if the monitor id is valid
                                if (monitorId > validationId)
                                {
                                    monitorId = prevMonitorId + 1;
                                }

                                //Update the previous monitor id
                                prevMonitorId = monitorId;

                                //Get the monitor friendly name
                                string monitorName = MonitorFriendlyName(pathInfo.targetInfo.adapterId, monitorId);

                                //Check the monitor friendly name
                                if (monitorName != "Unknown")
                                {
                                    monitorName = monitorName + " (" + monitorId + ")";

                                    //Add monitor to summary list
                                    monitorListSummary.Add(new DisplayMonitorSummary() { Id = monitorId, Name = monitorName });
                                }
                            }
                        }
                    }
                    catch { }
                    pathInfoIndex++;
                }

                return monitorListSummary;
            }
            catch
            {
                Debug.WriteLine("Failed loading the displays list.");
                return null;
            }
        }

        //Switch the primary monitor and disable the others
        public static bool SwitchPrimaryMonitor(uint switchMonitorId)
        {
            try
            {
                Debug.WriteLine("Switching to display monitor: " + switchMonitorId);
                EnableMonitorFirst();

                uint displayPathCount = 0;
                uint displayModeCount = 0;
                int error = GetDisplayConfigBufferSizes(QUERY_DEVICE_CONFIG_FLAGS.QDC_ALL_PATHS, out displayPathCount, out displayModeCount);
                if (error != 0)
                {
                    Debug.WriteLine("Failed GetDisplayConfigBufferSizes: " + error);
                    return false;
                }

                DISPLAYCONFIG_PATH_INFO[] displayPaths = new DISPLAYCONFIG_PATH_INFO[displayPathCount];
                DISPLAYCONFIG_MODE_INFO[] displayModes = new DISPLAYCONFIG_MODE_INFO[displayModeCount];
                error = QueryDisplayConfig(QUERY_DEVICE_CONFIG_FLAGS.QDC_ALL_PATHS, ref displayPathCount, displayPaths, ref displayModeCount, displayModes, DISPLAYCONFIG_TOPOLOGY_ID.DISPLAYCONFIG_TOPOLOGY_NONE);
                if (error != 0)
                {
                    Debug.WriteLine("Failed QueryDisplayConfig: " + error);
                    return false;
                }

                int pathInfoIndex = 0;
                int validationId = 100000;
                foreach (DISPLAYCONFIG_PATH_INFO pathInfo in displayPaths)
                {
                    try
                    {
                        if (pathInfo.targetInfo.targetAvailable)
                        {
                            if (pathInfo.sourceInfo.modeInfoIdx < validationId || pathInfo.sourceInfo.modeInfoIdx == 0)
                            {
                                if (pathInfo.targetInfo.modeInfoIdx > validationId || pathInfo.targetInfo.modeInfoIdx == 0)
                                {
                                    if (pathInfo.targetInfo.id == switchMonitorId)
                                    {
                                        displayPaths[pathInfoIndex].flags = (uint)DISPLAYCONFIG_PATH_FLAGS.DISPLAYCONFIG_PATH_ACTIVE;
                                    }
                                    else
                                    {
                                        displayPaths[pathInfoIndex].flags = (uint)DISPLAYCONFIG_PATH_FLAGS.DISPLAYCONFIG_PATH_DISABLE;
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                    pathInfoIndex++;
                }

                //Set display information
                error = SetDisplayConfig(displayPathCount, displayPaths, displayModeCount, displayModes, (uint)SetDisplayConfig_Flags.SDC_APPLY | (uint)SetDisplayConfig_Flags.SDC_USE_SUPPLIED_DISPLAY_CONFIG | (uint)SetDisplayConfig_Flags.SDC_SAVE_TO_DATABASE | (uint)SetDisplayConfig_Flags.SDC_ALLOW_CHANGES);
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
            catch
            {
                Debug.WriteLine("Failed switching the primary monitor.");
                return false;
            }
        }
    }
}