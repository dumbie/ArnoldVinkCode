using System;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVProcessLaunch
    {
        //Enumerations
        public enum SAFER_SCOPES : uint
        {
            SAFER_SCOPEID_MACHINE = 1,
            SAFER_SCOPEID_USER = 2
        }

        public enum SAFER_LEVELS : uint
        {
            SAFER_LEVELID_DISALLOWED = 0,
            SAFER_LEVELID_UNTRUSTED = 0x1000,
            SAFER_LEVELID_CONSTRAINED = 0x10000,
            SAFER_LEVELID_NORMALUSER = 0x20000,
            SAFER_LEVELID_FULLYTRUSTED = 0x40000
        }

        public enum SAFER_OPEN_FLAGS : uint
        {
            SAFER_LEVEL_OPEN = 1
        }

        public enum SAFER_COMPUTE_TOKEN_FLAGS : uint
        {
            SAFER_TOKEN_NONE = 0x0,
            SAFER_TOKEN_NULL_IF_EQUAL = 0x1,
            SAFER_TOKEN_COMPARE_ONLY = 0x2,
            SAFER_TOKEN_MAKE_INERT = 0x4,
            SAFER_TOKEN_WANT_FLAGS = 0x8
        }

        //Imports
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool SaferCreateLevel(SAFER_SCOPES dwScopeId, SAFER_LEVELS dwLevelId, SAFER_OPEN_FLAGS OpenFlags, out IntPtr pLevelHandle, IntPtr lpReserved);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool SaferComputeTokenFromLevel(IntPtr levelHandle, IntPtr inAccessToken, out IntPtr outAccessToken, SAFER_COMPUTE_TOKEN_FLAGS dwFlags, IntPtr lpReserved);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool SaferCloseLevel(IntPtr pLevelHandle);
    }
}