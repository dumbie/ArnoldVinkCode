using System;
using System.Diagnostics;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        public static ProcessAccess Get_ProcessAccessStatus(int processId, bool currentProcess)
        {
            IntPtr processTokenHandle = IntPtr.Zero;
            try
            {
                //Open process token
                if (currentProcess)
                {
                    processTokenHandle = Token_Create_Current();
                }
                else
                {
                    processTokenHandle = Token_Create_Process(processId, PROCESS_DESIRED_ACCESS.PROCESS_QUERY_LIMITED_INFORMATION, TOKEN_DESIRED_ACCESS.TOKEN_QUERY);
                }

                //Check process uiaccess access
                uint tokenUiAccess = 0;
                GetTokenInformation(processTokenHandle, TOKEN_INFORMATION_CLASS.TokenUIAccess, ref tokenUiAccess, sizeof(uint), out _);

                //Check process elevation access
                uint tokenElevation = 0;
                GetTokenInformation(processTokenHandle, TOKEN_INFORMATION_CLASS.TokenElevation, ref tokenElevation, sizeof(uint), out _);

                //Check process elevation type
                TOKEN_ELEVATION_TYPE tokenElevationType = TOKEN_ELEVATION_TYPE.TokenElevationTypeDefault;
                GetTokenInformation(processTokenHandle, TOKEN_INFORMATION_CLASS.TokenElevationType, ref tokenElevationType, sizeof(TOKEN_ELEVATION_TYPE), out _);

                //Create process access
                ProcessAccess processAccess = new ProcessAccess();
                processAccess.UiAccess = Convert.ToBoolean(tokenUiAccess);
                processAccess.Elevation = Convert.ToBoolean(tokenElevation);
                processAccess.ElevationType = tokenElevationType;

                //Check process admin access
                processAccess.AdminAccess = processAccess.Elevation || processAccess.ElevationType == TOKEN_ELEVATION_TYPE.TokenElevationTypeFull;

                //Debug.WriteLine("Process token uiaccess access: " + processAccess.UiAccess);
                //Debug.WriteLine("Process token administrator access: " + processAccess.AdminAccess);
                //Debug.WriteLine("Process token elevation access: " + processAccess.Elevation);
                //Debug.WriteLine("Process token elevation type: " + processAccess.ElevationType);
                return processAccess;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get process access status: " + ex.Message);
                return null;
            }
            finally
            {
                CloseHandleAuto(processTokenHandle);
            }
        }
    }
}