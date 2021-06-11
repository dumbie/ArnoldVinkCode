using System.Diagnostics;

namespace ArnoldVinkCode
{
    public partial class AVDisplayMonitor
    {
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

        //Set primary monitor and disable the others
        public static bool SetMonitorPrimary(uint switchMonitorId)
        {
            try
            {
                Debug.WriteLine("Switching to display monitor: " + switchMonitorId);

                //Query all monitors
                QueryMonitorsDisplayConfig(out uint displayPathCount, out uint displayModeCount, out DISPLAYCONFIG_PATH_INFO[] displayPaths, out DISPLAYCONFIG_MODE_INFO[] displayModes);

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
                                        displayPaths[pathInfoIndex].flags = DISPLAYCONFIG_PATH_FLAGS.DISPLAYCONFIG_PATH_ACTIVE;
                                    }
                                    else
                                    {
                                        displayPaths[pathInfoIndex].flags = DISPLAYCONFIG_PATH_FLAGS.DISPLAYCONFIG_PATH_DISABLE;
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                    pathInfoIndex++;
                }

                //Set display information
                int error = SetDisplayConfig(displayPathCount, displayPaths, displayModeCount, displayModes, (uint)SetDisplayConfig_Flags.SDC_APPLY | (uint)SetDisplayConfig_Flags.SDC_USE_SUPPLIED_DISPLAY_CONFIG | (uint)SetDisplayConfig_Flags.SDC_SAVE_TO_DATABASE | (uint)SetDisplayConfig_Flags.SDC_ALLOW_CHANGES);
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
                Debug.WriteLine("Failed setting the primary monitor.");
                return false;
            }
        }
    }
}