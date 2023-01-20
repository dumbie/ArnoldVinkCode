using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVProcessLaunch
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
                    Debug.WriteLine("Created well known sid: " + sidType.ToString() + "/" + sidPointer);
                    return true;
                }
                else
                {
                    Debug.WriteLine("Failed to create well known sid: " + sidType.ToString() + "/" + Marshal.GetLastWin32Error());
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to create well known sid: " + sidType.ToString() + "/" + ex.Message);
                return false;
            }
        }

        private static IntPtr Sid_Create_AllAdminGroup()
        {
            try
            {
                byte[] SID_IDENTIFIER_AUTHORITY = new byte[6];
                SID_IDENTIFIER_AUTHORITY[5] = 5;

                if (AllocateAndInitializeSid(SID_IDENTIFIER_AUTHORITY, 8, SECURITY_BUILTIN_DOMAIN_RID, DOMAIN_ALIAS_RID_ADMINS, 0, 0, 0, 0, 0, 0, out IntPtr allAdminGroupSid))
                {
                    Debug.WriteLine("Created admin group sid: " + allAdminGroupSid);
                    return allAdminGroupSid;
                }
                else
                {
                    Debug.WriteLine("Failed to get admin group sid: " + Marshal.GetLastWin32Error());
                    return IntPtr.Zero;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get admin group sid: " + ex.Message);
                return IntPtr.Zero;
            }
        }
    }
}