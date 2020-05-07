using System;

namespace ArnoldVinkCode
{
    public partial class AVDisplayMonitor
    {
        public static void GetScreenRefreshRate(int screenNumber, out int refreshRate)
        {
            refreshRate = -1;
            try
            {
                IntPtr createDC = CreateDC(@"\\.\DISPLAY" + screenNumber, null, null, IntPtr.Zero);
                refreshRate = GetDeviceCaps(createDC, DEVICECAP.VREFRESH);
                ReleaseDC(IntPtr.Zero, createDC);
            }
            catch { }
        }

        public static void GetScreenResolution(int screenNumber, out int screenWidth, out int screenHeight, out float dpiScale)
        {
            screenWidth = -1;
            screenHeight = -1;
            dpiScale = -1;
            try
            {
                IntPtr createDC = CreateDC(@"\\.\DISPLAY" + screenNumber, null, null, IntPtr.Zero);

                int nativeScreenWidth = GetDeviceCaps(createDC, DEVICECAP.HORZRES);
                screenWidth = nativeScreenWidth;
                //int desktopScreenWidth = GetDeviceCaps(createDC, DeviceCap.DESKTOPHORZRES);
                //int dpiWidth = GetDeviceCaps(createDC, DeviceCap.LOGPIXELSX);
                //int dpiScreenWidth = (int)(desktopScreenWidth / (float)dpiWidth * (float)96);

                int nativeScreenHeight = GetDeviceCaps(createDC, DEVICECAP.VERTRES);
                screenHeight = nativeScreenHeight;
                //int desktopScreenHeight = GetDeviceCaps(createDC, DeviceCap.DESKTOPVERTRES);
                //int dpiHeight = GetDeviceCaps(createDC, DeviceCap.LOGPIXELSY);
                //int dpiScreenHeight = (int)(desktopScreenHeight / (float)dpiHeight * (float)96);

                //Highest used DPI on all monitors
                int dpiWidth = GetDeviceCaps(createDC, DEVICECAP.LOGPIXELSX);
                dpiScale = (float)dpiWidth / (float)96;

                ReleaseDC(IntPtr.Zero, createDC);
            }
            catch { }
        }

        public static void GetScreenBounds(int screenNumber, out int boundsLeft, out int boundsTop)
        {
            boundsLeft = -1;
            boundsTop = -1;
            try
            {
                DEVMODE devMode = new DEVMODE();
                EnumDisplaySettings(@"\\.\DISPLAY" + screenNumber, IMODENUM.ENUM_CURRENT_SETTINGS, ref devMode);
                boundsLeft = devMode.dmPositionX;
                boundsTop = devMode.dmPositionY;
            }
            catch { }
        }
    }
}