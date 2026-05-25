using System;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        public static bool Detail_ProcessRespondingByWindowHandle(IntPtr targetWindowHandle)
        {
            bool processResponding = true;
            try
            {
                if (targetWindowHandle == IntPtr.Zero) { return processResponding; }
                processResponding = !IsHungAppWindow(targetWindowHandle);
                //Debug.WriteLine("Process window handle is responding: " + processResponding  + "/" + targetWindowHandle);
            }
            catch { }
            return processResponding;
        }

        public static ProcessAccessStatus Detail_ProcessAccessStatusByProcessId(IntPtr tokenHandle, bool currentProcess)
        {
            ProcessAccessStatus processAccessStatus = new ProcessAccessStatus();
            try
            {
                //Check process uiaccess access
                uint tokenUiAccess = 0;
                GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenUIAccess, ref tokenUiAccess, sizeof(uint), out _);

                //Check process elevation access
                uint tokenElevation = 0;
                GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenElevation, ref tokenElevation, sizeof(uint), out _);

                //Check process elevation type
                TOKEN_ELEVATION_TYPE tokenElevationType = TOKEN_ELEVATION_TYPE.TokenElevationTypeDefault;
                GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenElevationType, ref tokenElevationType, sizeof(TOKEN_ELEVATION_TYPE), out _);

                //Create process access
                processAccessStatus.UiAccess = Convert.ToBoolean(tokenUiAccess);
                processAccessStatus.Elevation = Convert.ToBoolean(tokenElevation);
                processAccessStatus.ElevationType = tokenElevationType;

                //Check process admin access
                processAccessStatus.AdminAccess = processAccessStatus.Elevation || processAccessStatus.ElevationType == TOKEN_ELEVATION_TYPE.TokenElevationTypeFull;

                //AVDebug.WriteLine("Process token uiaccess access: " + processAccessStatus.UiAccess);
                //AVDebug.WriteLine("Process token administrator access: " + processAccessStatus.AdminAccess);
                //AVDebug.WriteLine("Process token elevation access: " + processAccessStatus.Elevation);
                //AVDebug.WriteLine("Process token elevation type: " + processAccessStatus.ElevationType);
                return processAccessStatus;
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get process access status: " + ex.Message);
                return processAccessStatus;
            }
        }
    }
}