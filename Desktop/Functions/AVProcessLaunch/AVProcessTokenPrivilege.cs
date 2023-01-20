using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVProcessLaunch
    {
        public static bool Token_Set_Privilege(ref IntPtr hToken, PrivilegeConstants privilegeName, bool enablePrivilege)
        {
            //Does not add new privilege only change existing.
            try
            {
                //Lookup privilege value
                LUID luidPrivilege = new LUID();
                if (!LookupPrivilegeValueW(null, privilegeName.ToString(), ref luidPrivilege))
                {
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
                    Debug.WriteLine((enablePrivilege ? "Enabled" : "Removed") + " token privilege: " + privilegeName.ToString());
                    return true;
                }
                else
                {
                    Debug.WriteLine("Failed setting token privilege: " + privilegeName.ToString() + "/" + Marshal.GetLastWin32Error());
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed setting token privilege: " + ex.Message);
                return false;
            }
        }
    }
}