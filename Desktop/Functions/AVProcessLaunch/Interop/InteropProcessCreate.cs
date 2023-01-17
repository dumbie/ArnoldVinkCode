using System;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVProcessLaunch
    {
        //Structures
        [StructLayout(LayoutKind.Sequential)]
        public struct STARTUPINFO
        {
            public int cbSize;
            [MarshalAs(UnmanagedType.LPWStr)] public string lpReserved;
            [MarshalAs(UnmanagedType.LPWStr)] public string lpDesktop;
            [MarshalAs(UnmanagedType.LPWStr)] public string lpTitle;
            public int dwX;
            public int dwY;
            public int dwXSize;
            public int dwYSize;
            public int dwXCountChars;
            public int dwYCountChars;
            public int dwFillAttribute;
            public int dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public bool bInheritHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SID_AND_ATTRIBUTES
        {
            public IntPtr Sid;
            public SID_ATTRIBUTES Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LUID_AND_ATTRIBUTES
        {
            public LUID Luid;
            public LUID_ATTRIBUTES Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LUID
        {
            public uint LowPart;
            public int HighPart;
        }

        //Enumerations
        public enum TOKEN_DESIRED_ACCESS : uint
        {
            STANDARD_RIGHTS_REQUIRED = 0x000F0000,
            TOKEN_ASSIGN_PRIMARY = 0x0001,
            TOKEN_DUPLICATE = 0x0002,
            TOKEN_IMPERSONATE = 0x0004,
            TOKEN_QUERY = 0x0008,
            TOKEN_QUERY_SOURCE = 0x0010,
            TOKEN_ADJUST_PRIVILEGES = 0x0020,
            TOKEN_ADJUST_GROUPS = 0x0040,
            TOKEN_ADJUST_DEFAULT = 0x0080,
            TOKEN_ADJUST_SESSIONID = 0x0100,
            TOKEN_ALL_ACCESS = STANDARD_RIGHTS_REQUIRED | TOKEN_ASSIGN_PRIMARY | TOKEN_DUPLICATE | TOKEN_IMPERSONATE | TOKEN_QUERY | TOKEN_QUERY_SOURCE | TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT | TOKEN_ADJUST_SESSIONID
        }

        public enum TOKEN_TYPE : int
        {
            TokenPrimary = 1,
            TokenImpersonation = 2
        }

        public enum TOKEN_IMPERSONATION_LEVEL : int
        {
            SecurityAnonymous = 0,
            SecurityIdentification = 1,
            SecurityImpersonation = 2,
            SecurityDelegation = 3
        }

        public enum TOKEN_ELEVATION_TYPE : int
        {
            TokenElevationTypeDefault = 1,
            TokenElevationTypeFull = 2,
            TokenElevationTypeLimited = 3
        }

        public enum TOKEN_INFORMATION_CLASS : int
        {
            TokenUser = 1,
            TokenGroups = 2,
            TokenPrivileges = 3,
            TokenOwner = 4,
            TokenPrimaryGroup = 5,
            TokenDefaultDacl = 6,
            TokenSource = 7,
            TokenType = 8,
            TokenImpersonationLevel = 9,
            TokenStatistics = 10,
            TokenRestrictedSids = 11,
            TokenSessionId = 12,
            TokenGroupsAndPrivileges = 13,
            TokenSessionReference = 14,
            TokenSandBoxInert = 15,
            TokenAuditPolicy = 16,
            TokenOrigin = 17,
            TokenElevationType = 18,
            TokenLinkedToken = 19,
            TokenElevation = 20,
            TokenHasRestrictions = 21,
            TokenAccessInformation = 22,
            TokenVirtualizationAllowed = 23,
            TokenVirtualizationEnabled = 24,
            TokenIntegrityLevel = 25,
            TokenUIAccess = 26,
            TokenMandatoryPolicy = 27,
            TokenLogonSid = 28,
            TokenIsAppContainer = 29,
            TokenCapabilities = 30,
            TokenAppContainerSid = 31,
            TokenAppContainerNumber = 32,
            TokenUserClaimAttributes = 33,
            TokenDeviceClaimAttributes = 34,
            TokenRestrictedUserClaimAttributes = 35,
            TokenRestrictedDeviceClaimAttributes = 36,
            TokenDeviceGroups = 37,
            TokenRestrictedDeviceGroups = 38,
            TokenSecurityAttributes = 39,
            TokenIsRestricted = 40,
            MaxTokenInfoClass = 41
        }

        public enum WELL_KNOWN_SID_TYPE : int
        {
            WinUntrustedLabelSid = 65,
            WinLowLabelSid = 66,
            WinMediumLabelSid = 67,
            WinHighLabelSid = 68,
            WinSystemLabelSid = 69
        }

        public enum SID_ATTRIBUTES : uint
        {
            SE_GROUP_NONE = 0x00000000,
            SE_GROUP_MANDATORY = 0x00000001,
            SE_GROUP_ENABLED_BY_DEFAULT = 0x00000002,
            SE_GROUP_ENABLED = 0x00000004,
            SE_GROUP_OWNER = 0x00000008,
            SE_GROUP_USE_FOR_DENY_ONLY = 0x00000010,
            SE_GROUP_INTEGRITY = 0x00000020,
            SE_GROUP_INTEGRITY_ENABLED = 0x00000040,
            SE_GROUP_LOGON_ID = 0xC0000000,
            SE_GROUP_RESOURCE = 0x20000000,
            SE_GROUP_VALID_ATTRIBUTES = SE_GROUP_MANDATORY | SE_GROUP_ENABLED_BY_DEFAULT | SE_GROUP_ENABLED | SE_GROUP_OWNER | SE_GROUP_USE_FOR_DENY_ONLY | SE_GROUP_LOGON_ID | SE_GROUP_RESOURCE | SE_GROUP_INTEGRITY | SE_GROUP_INTEGRITY_ENABLED
        }

        public enum LUID_ATTRIBUTES : uint
        {
            SE_PRIVILEGE_ENABLED_BY_DEFAULT = 0x00000001,
            SE_PRIVILEGE_ENABLED = 0x00000002,
            SE_PRIVILEGE_REMOVED = 0X00000004,
            SE_PRIVILEGE_USED_FOR_ACCESS = 0x80000000,
            SE_PRIVILEGE_VALID_ATTRIBUTES = SE_PRIVILEGE_ENABLED_BY_DEFAULT | SE_PRIVILEGE_ENABLED | SE_PRIVILEGE_REMOVED | SE_PRIVILEGE_USED_FOR_ACCESS
        }

        public enum CreateRestrictedTokenFlags : uint
        {
            DISABLE_MAX_PRIVILEGE = 0x1,
            SANDBOX_INERT = 0x2,
            LUA_TOKEN = 0x4,
            WRITE_RESTRICTED = 0x8
        }

        //Imports
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CreateProcessAsUserW(IntPtr hToken, string lpApplicationName, string lpCommandLine, ref SECURITY_ATTRIBUTES lpProcessAttributes, ref SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandle, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CreateProcessWithTokenW(IntPtr hToken, uint dwLogonFlags, string lpApplicationName, string lpCommandLine, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool GetTokenInformation(IntPtr TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass, int TokenInformation, int TokenInformationLength, out int ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool SetTokenInformation(IntPtr TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass, ref int TokenInformation, int TokenInformationLength);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool SetTokenInformation(IntPtr TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass, SID_AND_ATTRIBUTES TokenInformation, int TokenInformationLength);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool DuplicateTokenEx(IntPtr hExistingToken, TOKEN_DESIRED_ACCESS dwDesiredAccess, ref SECURITY_ATTRIBUTES lpTokenAttributes, TOKEN_IMPERSONATION_LEVEL ImpersonationLevel, TOKEN_TYPE TokenType, out IntPtr phNewToken);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool OpenProcessToken(IntPtr ProcessHandle, TOKEN_DESIRED_ACCESS DesiredAccess, out IntPtr TokenHandle);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetShellWindow();

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool CreateWellKnownSid(WELL_KNOWN_SID_TYPE WellKnownSidType, IntPtr DomainSid, IntPtr pSid, ref int cbSidSize);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int GetSidLengthRequired(int nSubAuthorityCount);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool CreateRestrictedToken(IntPtr ExistingTokenHandle, CreateRestrictedTokenFlags Flags, int DisableSidCount, SID_AND_ATTRIBUTES[] SidsToDisable, int DeletePrivilegeCount, LUID_AND_ATTRIBUTES[] PrivilegesToDelete, int RestrictedSidCount, SID_AND_ATTRIBUTES[] SidsToRestrict, out IntPtr NewTokenHandle);
    }
}