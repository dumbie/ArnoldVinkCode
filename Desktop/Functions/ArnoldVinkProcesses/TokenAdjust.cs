using System;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        private static bool Token_Adjust_Privilege(IntPtr hToken, PrivilegeConstants privilegeName, bool enablePrivilege)
        {
            //Note: Does not add new privilege only changes existing.
            try
            {
                //Lookup privilege value
                LUID luidPrivilege = new LUID();
                if (!LookupPrivilegeValueW(null, privilegeName.ToString(), ref luidPrivilege))
                {
                    AVDebug.WriteLine("Failed lookup token privilege.");
                    return false;
                }

                //Create token privilege
                TOKEN_PRIVILEGES tokenPrivilege = new TOKEN_PRIVILEGES();
                tokenPrivilege.PrivilegeCount = 1;
                tokenPrivilege.Privileges = new LUID_AND_ATTRIBUTES();
                tokenPrivilege.Privileges.Luid = luidPrivilege;
                tokenPrivilege.Privileges.Attributes = enablePrivilege ? LUID_ATTRIBUTES.SE_PRIVILEGE_ENABLED : LUID_ATTRIBUTES.SE_PRIVILEGE_REMOVED;

                //Set token privilege
                if (AdjustTokenPrivileges(hToken, false, ref tokenPrivilege, 0, IntPtr.Zero, out int retLength))
                {
                    AVDebug.WriteLine((enablePrivilege ? "Enabled" : "Removed") + " token privilege: " + privilegeName.ToString());
                    return true;
                }
                else
                {
                    AVDebug.WriteLine("Failed setting token privilege: " + privilegeName.ToString() + "/" + Marshal.GetLastWin32Error());
                    return false;
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed setting token privilege: " + privilegeName.ToString() + "/" + ex.Message);
                return false;
            }
        }

        private static bool Token_Adjust_UIAccess(ref IntPtr hToken, bool enableUIAccess)
        {
            //Note: Enabling UIAccess requires SeTcbPrivilege.
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