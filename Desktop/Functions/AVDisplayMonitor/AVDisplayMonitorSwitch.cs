using System.Diagnostics;
using System.Linq;

namespace ArnoldVinkCode
{
    public partial class AVDisplayMonitor
    {
        public static bool EnableMonitorFirst()
        {
            try
            {
                int error = SetDisplayConfig(0, null, 0, null, (uint)DISPLAYCONFIG_FLAGS.SDC_APPLY | (uint)DISPLAYCONFIG_FLAGS.SDC_TOPOLOGY_INTERNAL);
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
                int error = SetDisplayConfig(0, null, 0, null, (uint)DISPLAYCONFIG_FLAGS.SDC_APPLY | (uint)DISPLAYCONFIG_FLAGS.SDC_TOPOLOGY_EXTERNAL);
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
                int error = SetDisplayConfig(0, null, 0, null, (uint)DISPLAYCONFIG_FLAGS.SDC_APPLY | (uint)DISPLAYCONFIG_FLAGS.SDC_TOPOLOGY_CLONE);
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
                int error = SetDisplayConfig(0, null, 0, null, (uint)DISPLAYCONFIG_FLAGS.SDC_APPLY | (uint)DISPLAYCONFIG_FLAGS.SDC_TOPOLOGY_EXTEND);
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

                //Set topology mode
                int error = SetDisplayConfig(0, null, 0, null, (uint)DISPLAYCONFIG_FLAGS.SDC_APPLY | (uint)DISPLAYCONFIG_FLAGS.SDC_TOPOLOGY_INTERNAL);
                if (error != 0)
                {
                    Debug.WriteLine("Failed SetDisplayConfig Topology: " + error);
                    return false;
                }

                //Query all monitors
                QueryMonitorsDisplayConfig(out uint displayPathCount, out uint displayModeCount, out DISPLAYCONFIG_PATH_INFO[] displayPaths, out DISPLAYCONFIG_MODE_INFO[] displayModes);

                //Check all monitors
                int pathInfoIndex = 0;
                foreach (DISPLAYCONFIG_PATH_INFO pathInfo in displayPaths)
                {
                    try
                    {
                        //Validate monitor
                        if (pathInfo.targetInfo.targetAvailable && pathInfo.sourceInfo.modeInfoIdx >= 0 && pathInfo.sourceInfo.modeInfoIdx < displayModeCount)
                        {
                            //Enable or disable monitor
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
                    catch { }
                    pathInfoIndex++;
                }

                //Set display information
                error = SetDisplayConfig(displayPathCount, displayPaths, displayModeCount, displayModes, (uint)DISPLAYCONFIG_FLAGS.SDC_APPLY | (uint)DISPLAYCONFIG_FLAGS.SDC_USE_SUPPLIED_DISPLAY_CONFIG | (uint)DISPLAYCONFIG_FLAGS.SDC_SAVE_TO_DATABASE | (uint)DISPLAYCONFIG_FLAGS.SDC_ALLOW_CHANGES);
                if (error != 0)
                {
                    Debug.WriteLine("Failed SetDisplayConfig: " + error);
                    return false;
                }
                else
                {
                    Debug.WriteLine("Adjusted SetDisplayConfig.");
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