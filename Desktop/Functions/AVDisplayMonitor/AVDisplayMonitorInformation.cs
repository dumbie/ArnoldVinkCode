using System.Windows.Forms;

namespace ArnoldVinkCode
{
    public partial class AVDisplayMonitor
    {
        public static int GetScreenRefreshRate(Screen targetScreen)
        {
            try
            {
                DEVMODE devMode = new DEVMODE();
                EnumDisplaySettings(targetScreen.DeviceName, IMODENUM.ENUM_CURRENT_SETTINGS, ref devMode);
                //Debug.WriteLine("Monitor refresh rate: " + devMode.dmDisplayFrequency + "Hz");
                return devMode.dmDisplayFrequency;
            }
            catch { }
            return 0;
        }
    }
}