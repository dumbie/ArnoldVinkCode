using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcessLaunch
    {
        private static bool Token_Adjust_UIAccess(bool setUiAccess, ref IntPtr token)
        {
            //Enabling uiaccess requires SeTcbPrivilege.
            try
            {
                //Set token information
                uint tokenInformation = setUiAccess ? (uint)1 : (uint)0;
                if (SetTokenInformation(token, TOKEN_INFORMATION_CLASS.TokenUIAccess, ref tokenInformation, sizeof(uint)))
                {
                    Debug.WriteLine("Adjusted token ui access to: " + (setUiAccess ? "Enabled" : "Disabled"));
                    return true;
                }
                else
                {
                    Debug.WriteLine("Failed adjusting token ui access: " + Marshal.GetLastWin32Error());
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adjusting token ui access: " + ex.Message);
                return false;
            }
        }

        private static bool Token_Adjust_Integrity(WELL_KNOWN_SID_TYPE sidType, ref IntPtr token)
        {
            IntPtr sidPointer = IntPtr.Zero;
            try
            {
                //Create integrity sid
                Sid_Create_WellKnownType(sidType, out sidPointer, out int sidSize);

                //Set integrity sid
                SID_AND_ATTRIBUTES sidAttributes = new SID_AND_ATTRIBUTES();
                sidAttributes.Attributes = SID_ATTRIBUTES.SE_GROUP_INTEGRITY | SID_ATTRIBUTES.SE_GROUP_INTEGRITY_ENABLED;
                sidAttributes.Sid = sidPointer;

                //Set token information
                int cbTokenSize = Marshal.SizeOf(sidAttributes);
                int tokenInfoSize = cbTokenSize + sidSize;
                if (SetTokenInformation(token, TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, sidAttributes, tokenInfoSize))
                {
                    Debug.WriteLine("Adjusted token integrity level: " + sidType);
                    return true;
                }
                else
                {
                    Debug.WriteLine("Failed adjusting token integrity level: " + Marshal.GetLastWin32Error());
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adjusting token integrity level: " + ex.Message);
                return false;
            }
            finally
            {
                CloseMarshalAuto(sidPointer);
            }
        }

        private static bool Token_Disable_Elevation(ref IntPtr token)
        {
            try
            {
                //Create integrity sid
                SID_AND_ATTRIBUTES[] sidDisable = new SID_AND_ATTRIBUTES[1];

                SID_AND_ATTRIBUTES restrictAllAdminSid = new SID_AND_ATTRIBUTES();
                restrictAllAdminSid.Attributes = SID_ATTRIBUTES.SE_GROUP_NONE;
                restrictAllAdminSid.Sid = Sid_Create_AllAdminGroup();
                sidDisable[0] = restrictAllAdminSid;

                SID_AND_ATTRIBUTES[] sidRestrict = new SID_AND_ATTRIBUTES[0];
                LUID_AND_ATTRIBUTES[] luidDelete = new LUID_AND_ATTRIBUTES[0];

                //Create restricted token
                if (CreateRestrictedToken(token, CreateRestrictedTokenFlags.LUA_TOKEN, sidDisable.Count(), sidDisable, luidDelete.Count(), luidDelete, sidRestrict.Count(), sidRestrict, out token))
                {
                    Debug.WriteLine("Removed admin elevation from token: " + token);
                    return true;
                }
                else
                {
                    Debug.WriteLine("Failed removing admin elevation token: " + token + "/" + Marshal.GetLastWin32Error());
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed removing admin elevation token: " + ex.Message);
                return false;
            }
        }
    }
}