﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVDisplayMonitor
    {
        //Query all monitors
        private static bool QueryMonitorsDisplayConfig(out uint displayPathCount, out uint displayModeCount, out DISPLAYCONFIG_PATH_INFO[] displayPaths, out DISPLAYCONFIG_MODE_INFO[] displayModes)
        {
            try
            {
                int error = GetDisplayConfigBufferSizes(QUERY_DEVICE_CONFIG_FLAGS.QDC_ALL_PATHS, out displayPathCount, out displayModeCount);
                if (error != 0)
                {
                    Debug.WriteLine("Failed GetDisplayConfigBufferSizes: " + error);
                    displayPathCount = 0;
                    displayModeCount = 0;
                    displayPaths = null;
                    displayModes = null;
                    return false;
                }

                displayPaths = new DISPLAYCONFIG_PATH_INFO[displayPathCount];
                displayModes = new DISPLAYCONFIG_MODE_INFO[displayModeCount];
                error = QueryDisplayConfig(QUERY_DEVICE_CONFIG_FLAGS.QDC_ALL_PATHS, ref displayPathCount, displayPaths, ref displayModeCount, displayModes, DISPLAYCONFIG_FLAGS.SDC_TOPOLOGY_NONE);
                if (error != 0)
                {
                    Debug.WriteLine("Failed QueryDisplayConfig: " + error);
                    displayPathCount = 0;
                    displayModeCount = 0;
                    displayPaths = null;
                    displayModes = null;
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to query monitors: " + ex.Message);
                displayPathCount = 0;
                displayModeCount = 0;
                displayPaths = null;
                displayModes = null;
                return false;
            }
        }

        //List all the connected display monitors
        public static List<DisplayMonitor> ListMonitorsDisplayConfig()
        {
            try
            {
                //Query all monitors
                QueryMonitorsDisplayConfig(out uint displayPathCount, out uint displayModeCount, out DISPLAYCONFIG_PATH_INFO[] displayPaths, out DISPLAYCONFIG_MODE_INFO[] displayModes);

                //Check all monitors
                List<DisplayMonitor> monitorListSummary = new List<DisplayMonitor>();
                foreach (DISPLAYCONFIG_PATH_INFO pathInfo in displayPaths)
                {
                    try
                    {
                        //Validate monitor
                        if (pathInfo.targetInfo.targetAvailable && pathInfo.sourceInfo.modeInfoIdx >= 0 && pathInfo.sourceInfo.modeInfoIdx < displayModeCount)
                        {
                            if (!monitorListSummary.Any(x => x.Identifier == pathInfo.targetInfo.id))
                            {
                                //Get the monitor friendly name
                                string monitorName = GetMonitorFriendlyName(pathInfo.targetInfo.adapterId, pathInfo.targetInfo.id);

                                //Check the monitor friendly name
                                if (monitorName != "Unknown")
                                {
                                    monitorName = monitorName + " (" + pathInfo.targetInfo.id + ")";

                                    //Add monitor to summary list
                                    monitorListSummary.Add(new DisplayMonitor() { Identifier = (int)pathInfo.targetInfo.id, Name = monitorName });
                                }
                            }
                        }
                    }
                    catch { }
                }

                return monitorListSummary;
            }
            catch
            {
                Debug.WriteLine("Failed loading the displays list.");
                return null;
            }
        }

        //Get monitor information
        public static DisplayMonitor MonitorDisplayConfig(int screenNumber)
        {
            try
            {
                //Check screen number
                if (screenNumber < 0) { screenNumber = 0; }

                //Query all monitors
                QueryMonitorsDisplayConfig(out uint displayPathCount, out uint displayModeCount, out DISPLAYCONFIG_PATH_INFO[] displayPaths, out DISPLAYCONFIG_MODE_INFO[] displayModes);

                //Check all monitors
                int monitorIndex = 0;
                int pathInfoIndex = 0;
                DISPLAYCONFIG_PATH_INFO pathInfoTarget = new DISPLAYCONFIG_PATH_INFO();
                DISPLAYCONFIG_MODE_INFO modeInfoTarget = new DISPLAYCONFIG_MODE_INFO();
                foreach (DISPLAYCONFIG_PATH_INFO pathInfo in displayPaths)
                {
                    try
                    {
                        //Validate monitor
                        if (pathInfo.targetInfo.targetAvailable && pathInfo.sourceInfo.modeInfoIdx >= 0 && pathInfo.sourceInfo.modeInfoIdx < displayModeCount)
                        {
                            //Check the monitor id
                            if (screenNumber == monitorIndex)
                            {
                                pathInfoTarget = displayPaths[pathInfoIndex];
                                modeInfoTarget = displayModes[pathInfoIndex];
                                break;
                            }
                            monitorIndex++;
                        }
                    }
                    catch { }
                    pathInfoIndex++;
                }

                //Create display monitor
                DisplayMonitor displayMonitorSettings = new DisplayMonitor();
                displayMonitorSettings.Identifier = screenNumber;

                //Get the screen name
                string monitorName = GetMonitorFriendlyName(pathInfoTarget.targetInfo.adapterId, pathInfoTarget.targetInfo.id);
                if (monitorName != "Unknown")
                {
                    monitorName = monitorName + " (" + pathInfoTarget.targetInfo.id + ")";
                }
                displayMonitorSettings.Name = monitorName;

                //Get the device path
                displayMonitorSettings.DevicePath = GetMonitorDevicePath(pathInfoTarget.targetInfo.adapterId, pathInfoTarget.targetInfo.id);

                //Get the screen resolution
                displayMonitorSettings.WidthNative = (int)modeInfoTarget.targetMode.targetVideoSignalInfo.activeSize.cx;
                displayMonitorSettings.HeightNative = (int)modeInfoTarget.targetMode.targetVideoSignalInfo.activeSize.cy;

                //Get the screen refresh rate
                if (pathInfoTarget.targetInfo.refreshRate.Numerator != 0)
                {
                    displayMonitorSettings.RefreshRate = (int)(pathInfoTarget.targetInfo.refreshRate.Numerator / pathInfoTarget.targetInfo.refreshRate.Denominator);
                }
                else
                {
                    displayMonitorSettings.RefreshRate = 0;
                }

                //Get the screen hdr status
                displayMonitorSettings.HdrEnabled = GetMonitorHdrStatus(pathInfoTarget.targetInfo.adapterId, pathInfoTarget.targetInfo.id);
                displayMonitorSettings.HdrWhiteLevel = GetMonitorHdrWhiteLevel(pathInfoTarget.targetInfo.adapterId, pathInfoTarget.targetInfo.id);

                return displayMonitorSettings;
            }
            catch
            {
                Debug.WriteLine("Failed getting displayconfig monitor information.");
                return null;
            }
        }

        private static string GetMonitorFriendlyName(LUID adapterId, uint targetId)
        {
            try
            {
                DISPLAYCONFIG_TARGET_DEVICE_NAME deviceInfo = new DISPLAYCONFIG_TARGET_DEVICE_NAME();
                deviceInfo.header.size = (uint)Marshal.SizeOf(typeof(DISPLAYCONFIG_TARGET_DEVICE_NAME));
                deviceInfo.header.adapterId = adapterId;
                deviceInfo.header.id = targetId;
                deviceInfo.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME;

                int error = DisplayConfigGetDeviceInfo(ref deviceInfo);
                if (error != 0)
                {
                    Debug.WriteLine("Failed GetMonitorFriendlyName: " + error);
                    return "Unknown";
                }

                string friendlyName = deviceInfo.monitorFriendlyDeviceName;
                if (!string.IsNullOrWhiteSpace(friendlyName))
                {
                    return deviceInfo.monitorFriendlyDeviceName;
                }
                else
                {
                    return "Unknown";
                }
            }
            catch
            {
                Debug.WriteLine("Failed GetMonitorFriendlyName.");
                return "Unknown";
            }
        }

        private static string GetMonitorDevicePath(LUID adapterId, uint targetId)
        {
            try
            {
                DISPLAYCONFIG_TARGET_DEVICE_NAME deviceInfo = new DISPLAYCONFIG_TARGET_DEVICE_NAME();
                deviceInfo.header.size = (uint)Marshal.SizeOf(typeof(DISPLAYCONFIG_TARGET_DEVICE_NAME));
                deviceInfo.header.adapterId = adapterId;
                deviceInfo.header.id = targetId;
                deviceInfo.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME;

                int error = DisplayConfigGetDeviceInfo(ref deviceInfo);
                if (error != 0)
                {
                    Debug.WriteLine("Failed GetMonitorDevicePath: " + error);
                    return "Unknown";
                }

                return deviceInfo.monitorDevicePath;
            }
            catch
            {
                Debug.WriteLine("Failed GetMonitorDevicePath.");
                return "Unknown";
            }
        }

        private static bool GetMonitorHdrStatus(LUID adapterId, uint targetId)
        {
            try
            {
                DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO deviceInfo = new DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO();
                deviceInfo.header.size = (uint)Marshal.SizeOf(typeof(DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO));
                deviceInfo.header.adapterId = adapterId;
                deviceInfo.header.id = targetId;
                deviceInfo.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO;

                int error = DisplayConfigGetDeviceInfo(ref deviceInfo);
                if (error != 0)
                {
                    Debug.WriteLine("Failed GetMonitorHdrStatus: " + error);
                    return false;
                }

                return deviceInfo.advancedColorEnabled;
            }
            catch
            {
                Debug.WriteLine("Failed GetMonitorHdrStatus.");
                return false;
            }
        }

        private static int GetMonitorHdrWhiteLevel(LUID adapterId, uint targetId)
        {
            try
            {
                DISPLAYCONFIG_SDR_WHITE_LEVEL deviceInfo = new DISPLAYCONFIG_SDR_WHITE_LEVEL();
                deviceInfo.header.size = (uint)Marshal.SizeOf(typeof(DISPLAYCONFIG_SDR_WHITE_LEVEL));
                deviceInfo.header.adapterId = adapterId;
                deviceInfo.header.id = targetId;
                deviceInfo.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_SDR_WHITE_LEVEL;

                int error = DisplayConfigGetDeviceInfo(ref deviceInfo);
                if (error != 0)
                {
                    Debug.WriteLine("Failed GetMonitorHdrWhiteLevel: " + error);
                    return 0;
                }

                return deviceInfo.SDRWhiteLevel;
            }
            catch
            {
                Debug.WriteLine("Failed GetMonitorHdrWhiteLevel.");
                return 0;
            }
        }
    }
}