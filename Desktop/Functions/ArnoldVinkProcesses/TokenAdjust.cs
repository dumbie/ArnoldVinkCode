using System;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        private static bool Token_Adjust_UIAccess(ref IntPtr hToken, bool enableUIAccess)
        {
            //Enabling uiaccess requires SeTcbPrivilege.
            try
            {
                //Set token information
                uint tokenInformation = enableUIAccess ? (uint)1 : (uint)0;
                if (SetTokenInformation(hToken, TOKEN_INFORMATION_CLASS.TokenUIAccess, ref tokenInformation, sizeof(uint)))
                {
                    AVDebug.WriteLine("Adjusted token ui access to: " + (enableUIAccess ? "Enabled" : "Disabled"));
                    return true;
                }
                else
                {
                    AVDebug.WriteLine("Failed adjusting token ui access: " + Marshal.GetLastWin32Error());
                    return false;
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed adjusting token ui access: " + ex.Message);
                return false;
            }
        }
    }
}