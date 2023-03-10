using System;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        private static bool Sid_Create_WellKnownType(WELL_KNOWN_SID_TYPE sidType, out IntPtr sidPointer, out int sidSize)
        {
            //Close sid pointer marshal after usage.
            sidSize = 0;
            sidPointer = IntPtr.Zero;
            try
            {
                sidSize = GetSidLengthRequired(SID_MAX_SUB_AUTHORITIES);
                sidPointer = Marshal.AllocHGlobal(sidSize);
                if (CreateWellKnownSid(sidType, IntPtr.Zero, sidPointer, ref sidSize))
                {
                    AVDebug.WriteLine("Created well known sid: " + sidType.ToString() + "/" + sidPointer);
                    return true;
                }
                else
                {
                    AVDebug.WriteLine("Failed to create well known sid: " + sidType.ToString() + "/" + Marshal.GetLastWin32Error());
                    return false;
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to create well known sid: " + sidType.ToString() + "/" + ex.Message);
                return false;
            }
        }

        private static IntPtr Sid_Create_AllAdminGroup()
        {
            try
            {
                if (AllocateAndInitializeSid(SECURITY_NT_AUTHORITY, 8, SECURITY_BUILTIN_DOMAIN_RID, DOMAIN_ALIAS_RID_ADMINS, 0, 0, 0, 0, 0, 0, out IntPtr allAdminGroupSid))
                {
                    AVDebug.WriteLine("Created admin group sid: " + allAdminGroupSid);
                    return allAdminGroupSid;
                }
                else
                {
                    AVDebug.WriteLine("Failed to create admin group sid: " + Marshal.GetLastWin32Error());
                    return IntPtr.Zero;
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to create admin group sid: " + ex.Message);
                return IntPtr.Zero;
            }
        }

        private static IntPtr Sid_Create_AllUserGroup()
        {
            try
            {
                if (AllocateAndInitializeSid(SECURITY_NT_AUTHORITY, 8, SECURITY_BUILTIN_DOMAIN_RID, DOMAIN_ALIAS_RID_USERS, 0, 0, 0, 0, 0, 0, out IntPtr allAdminGroupSid))
                {
                    AVDebug.WriteLine("Created user group sid: " + allAdminGroupSid);
                    return allAdminGroupSid;
                }
                else
                {
                    AVDebug.WriteLine("Failed to create user group sid: " + Marshal.GetLastWin32Error());
                    return IntPtr.Zero;
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to create user group sid: " + ex.Message);
                return IntPtr.Zero;
            }
        }
    }
}