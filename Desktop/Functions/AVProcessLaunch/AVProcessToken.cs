using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcessLaunch
    {
        private static bool Token_Adjust_UIAccess(bool enabled, ref IntPtr token)
        {
            try
            {
                //Set token information
                int tokenInformation = enabled ? 1 : 0;
                bool tokenAdjusted = SetTokenInformation(token, TOKEN_INFORMATION_CLASS.TokenUIAccess, ref tokenInformation, sizeof(int));

                Debug.WriteLine(enabled ? "Enabled" : "Disabled" + " token ui access: " + tokenAdjusted);
                return tokenAdjusted;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adjusting token ui access: " + ex.Message);
                return false;
            }
        }

        private static bool Token_Adjust_Integrity(WELL_KNOWN_SID_TYPE sidType, ref IntPtr token)
        {
            IntPtr pIntegritySid = IntPtr.Zero;
            try
            {
                //Create integrity sid
                int cbSidSize = GetSidLengthRequired(1);
                pIntegritySid = Marshal.AllocHGlobal(cbSidSize);
                CreateWellKnownSid(sidType, IntPtr.Zero, pIntegritySid, ref cbSidSize);

                //Set integrity level
                SID_AND_ATTRIBUTES test = new SID_AND_ATTRIBUTES();
                test.Attributes = SID_ATTRIBUTES.SE_GROUP_INTEGRITY;
                test.Sid = pIntegritySid;

                //Set token information
                int cbTokenSize = Marshal.SizeOf(test);
                int tokenInfoSize = cbTokenSize + cbSidSize;
                bool tokenAdjusted = SetTokenInformation(token, TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, test, tokenInfoSize);

                Debug.WriteLine("Adjusted token integrity level: " + tokenAdjusted + "/" + sidType);
                return tokenAdjusted;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adjusting token integrity level: " + ex.Message);
                return false;
            }
            finally
            {
                CloseMarshalAuto(pIntegritySid);
            }
        }
    }
}