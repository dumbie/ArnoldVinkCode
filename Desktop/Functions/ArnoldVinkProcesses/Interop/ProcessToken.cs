﻿using System;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Constants
        public const int SID_MAX_SUB_AUTHORITIES = 15;
        public const int SECURITY_BUILTIN_DOMAIN_RID = 0x00000020;
        public const int DOMAIN_ALIAS_RID_ADMINS = 0x00000220;
        public const int DOMAIN_ALIAS_RID_USERS = 0x00000221;
        public static byte[] SECURITY_NT_AUTHORITY = { 0, 0, 0, 0, 0, 5 };

        //Structures
        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public uint nLength;
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

        public enum LUID_ATTRIBUTES : uint
        {
            SE_PRIVILEGE_ENABLED_BY_DEFAULT = 0x00000001,
            SE_PRIVILEGE_ENABLED = 0x00000002,
            SE_PRIVILEGE_REMOVED = 0X00000004,
            SE_PRIVILEGE_USED_FOR_ACCESS = 0x80000000,
            SE_PRIVILEGE_VALID_ATTRIBUTES = SE_PRIVILEGE_ENABLED_BY_DEFAULT | SE_PRIVILEGE_ENABLED | SE_PRIVILEGE_REMOVED | SE_PRIVILEGE_USED_FOR_ACCESS
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TOKEN_PRIVILEGES
        {
            public int PrivilegeCount;
            public LUID_AND_ATTRIBUTES Privileges;
        }

        //Enumerators
        //https://learn.microsoft.com/en-us/windows/win32/secauthz/privilege-constants
        public enum PrivilegeConstants //ToString()
        {
            SeTcbPrivilege,
            SeDebugPrivilege,
            SeCreateTokenPrivilege,
            SeIncreaseQuotaPrivilege,
            SeAssignPrimaryTokenPrivilege
        }

        //https://learn.microsoft.com/en-us/windows/win32/api/winnt/ne-winnt-well_known_sid_type
        public enum WELL_KNOWN_SID_TYPE : int
        {
            WinAuthenticatedUserSid = 17,
            BuiltinAdministratorsSid = 26,
            AccountAdministratorSid = 38,
            WinUntrustedLabelSid = 65,
            WinLowLabelSid = 66,
            WinMediumLabelSid = 67,
            WinHighLabelSid = 68,
            WinSystemLabelSid = 69,
            WinLocalLogonSid = 80,
            WinConsoleLogonSid = 81
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
            TokenProcessTrustLevel = 41,
            TokenPrivateNameSpace = 42,
            TokenSingletonAttributes = 43,
            TokenBnoIsolation = 44,
            TokenChildProcessFlags = 45,
            TokenIsLessPrivilegedAppContainer = 46,
            TokenIsSandboxed = 47,
            MaxTokenInfoClass = 48
        }

        //Imports
        [DllImport("advapi32.dll", SetLastError = true)]
        public extern static bool AllocateAndInitializeSid(byte[] pIdentifierAuthority, int nSubAuthorityCount, int dwSubAuthority0, int dwSubAuthority1, int dwSubAuthority2, int dwSubAuthority3, int dwSubAuthority4, int dwSubAuthority5, int dwSubAuthority6, int dwSubAuthority7, out IntPtr pSid);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool CreateWellKnownSid(WELL_KNOWN_SID_TYPE WellKnownSidType, IntPtr DomainSid, IntPtr pSid, ref int cbSidSize);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int GetSidLengthRequired(int nSubAuthorityCount);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool OpenProcessToken(IntPtr ProcessHandle, TOKEN_DESIRED_ACCESS DesiredAccess, out IntPtr TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool DuplicateTokenEx(IntPtr hExistingToken, TOKEN_DESIRED_ACCESS dwDesiredAccess, ref SECURITY_ATTRIBUTES lpTokenAttributes, TOKEN_IMPERSONATION_LEVEL ImpersonationLevel, TOKEN_TYPE TokenType, out IntPtr phNewToken);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool GetTokenInformation(IntPtr TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass, ref uint TokenInformation, uint TokenInformationLength, out uint ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool GetTokenInformation(IntPtr TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass, ref TOKEN_ELEVATION_TYPE TokenInformation, uint TokenInformationLength, out uint ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool SetTokenInformation(IntPtr TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass, ref uint TokenInformation, uint TokenInformationLength);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool SetTokenInformation(IntPtr TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass, SID_AND_ATTRIBUTES TokenInformation, int TokenInformationLength);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LookupPrivilegeValueW(string lpSystemName, string lpName, ref LUID lpLuid);

        [DllImport("kernelbase.dll", SetLastError = true)]
        public static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, bool DisableAllPrivileges, ref TOKEN_PRIVILEGES NewState, int BufferLength, IntPtr PreviousState, out int ReturnLength);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetShellWindow();
    }
}