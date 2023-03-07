using System;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Imports
        [DllImport("ntdll.dll", EntryPoint = "NtWow64QueryInformationProcess64")]
        private static extern uint NtQueryInformationProcessWOW64(IntPtr ProcessHandle, PROCESS_INFO_CLASS ProcessInformationClass, ref PROCESS_BASIC_INFORMATIONWOW64 ProcessInformation, uint ProcessInformationLength, out uint ReturnLength);

        [DllImport("ntdll.dll", EntryPoint = "NtWow64ReadVirtualMemory64")]
        private static extern uint NtReadVirtualMemoryWOW64(IntPtr ProcessHandle, long BaseAddress, ref PEBWOW64 Buffer, ulong NumberOfBytesToRead, out ulong NumberOfBytesRead);

        [DllImport("ntdll.dll", EntryPoint = "NtWow64ReadVirtualMemory64")]
        private static extern uint NtReadVirtualMemoryWOW64(IntPtr ProcessHandle, long BaseAddress, ref RTL_USER_PROCESS_PARAMETERSWOW64 Buffer, ulong NumberOfBytesToRead, out ulong NumberOfBytesRead);

        [DllImport("ntdll.dll", EntryPoint = "NtWow64ReadVirtualMemory64")]
        private static extern uint NtReadVirtualMemoryWOW64(IntPtr ProcessHandle, long BaseAddress, [MarshalAs(UnmanagedType.LPWStr)] string Buffer, ulong NumberOfBytesToRead, out ulong NumberOfBytesRead);

        //Structures
        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_BASIC_INFORMATIONWOW64
        {
            public int ExitStatus;
            public long PebBaseAddress;
            public long AffinityMask;
            public int BasePriority;
            public long UniqueProcessId;
            public long InheritedFromUniqueProcessId;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PEBWOW64
        {
            public long Reserved0;
            public long Reserved1;
            public long Reserved2;
            public long Reserved3;
            public long RtlUserProcessParameters;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct UNICODE_STRINGWOW64
        {
            public ushort Length;
            public ushort MaximumLength;
            public long Buffer;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RTL_DRIVE_LETTER_CURDIRWOW64
        {
            public ushort Flags;
            public ushort Length;
            public uint TimeStamp;
            public UNICODE_STRINGWOW64 DosPath;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RTL_USER_PROCESS_PARAMETERSWOW64
        {
            public uint MaximumLength;
            public uint Length;
            public uint Flags;
            public uint DebugFlags;
            public long ConsoleHandle;
            public uint ConsoleFlags;
            public long StandardInput;
            public long StandardOutput;
            public long StandardError;
            public UNICODE_STRINGWOW64 CurrentDirectory;
            public long CurrentDirectoryHandle;
            public UNICODE_STRINGWOW64 DllPath;
            public UNICODE_STRINGWOW64 ImagePathName;
            public UNICODE_STRINGWOW64 CommandLine;
            public long Environment;
            public uint StartingX;
            public uint StartingY;
            public uint CountX;
            public uint CountY;
            public uint CountCharsX;
            public uint CountCharsY;
            public uint FillAttribute;
            public uint WindowFlags;
            public uint ShowWindowFlags;
            public UNICODE_STRINGWOW64 WindowTitle;
            public UNICODE_STRINGWOW64 DesktopInfo;
            public UNICODE_STRINGWOW64 ShellInfo;
            public UNICODE_STRINGWOW64 RuntimeData;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public RTL_DRIVE_LETTER_CURDIRWOW64[] CurrentDirectores;
            public uint EnvironmentSize;
        }

        //Methods
        private static string GetApplicationParameterWOW64(IntPtr processHandle, PROCESS_PARAMETER_OPTIONS pOption)
        {
            string parameterString = string.Empty;
            try
            {
                //AVDebug.WriteLine("GetApplicationParameter architecture WOW64");

                PROCESS_BASIC_INFORMATIONWOW64 basicInformation = new PROCESS_BASIC_INFORMATIONWOW64();
                uint readResult = NtQueryInformationProcessWOW64(processHandle, PROCESS_INFO_CLASS.ProcessBasicInformation, ref basicInformation, (uint)Marshal.SizeOf(basicInformation), out _);
                if (readResult != 0)
                {
                    //AVDebug.WriteLine("Failed to get ProcessBasicInformation for: " + processHandle + "/Query failed.");
                    return parameterString;
                }

                PEBWOW64 pebCopy = new PEBWOW64();
                readResult = NtReadVirtualMemoryWOW64(processHandle, basicInformation.PebBaseAddress, ref pebCopy, (uint)Marshal.SizeOf(pebCopy), out _);
                if (readResult != 0)
                {
                    //AVDebug.WriteLine("Failed to get PebBaseAddress for: " + processHandle);
                    return parameterString;
                }

                RTL_USER_PROCESS_PARAMETERSWOW64 paramsCopy = new RTL_USER_PROCESS_PARAMETERSWOW64();
                readResult = NtReadVirtualMemoryWOW64(processHandle, pebCopy.RtlUserProcessParameters, ref paramsCopy, (uint)Marshal.SizeOf(paramsCopy), out _);
                if (readResult != 0)
                {
                    //AVDebug.WriteLine("Failed to get ProcessParameters for: " + processHandle);
                    return parameterString;
                }

                ushort stringLength = 0;
                long stringBuffer = 0;
                if (pOption == PROCESS_PARAMETER_OPTIONS.CurrentDirectoryPath)
                {
                    stringLength = paramsCopy.CurrentDirectory.Length;
                    stringBuffer = paramsCopy.CurrentDirectory.Buffer;
                }
                else if (pOption == PROCESS_PARAMETER_OPTIONS.ImagePathName)
                {
                    stringLength = paramsCopy.ImagePathName.Length;
                    stringBuffer = paramsCopy.ImagePathName.Buffer;
                }
                else if (pOption == PROCESS_PARAMETER_OPTIONS.DesktopInfo)
                {
                    stringLength = paramsCopy.DesktopInfo.Length;
                    stringBuffer = paramsCopy.DesktopInfo.Buffer;
                }
                else if (pOption == PROCESS_PARAMETER_OPTIONS.Environment)
                {
                    stringLength = (ushort)paramsCopy.EnvironmentSize;
                    stringBuffer = paramsCopy.Environment;
                }
                else
                {
                    stringLength = paramsCopy.CommandLine.Length;
                    stringBuffer = paramsCopy.CommandLine.Buffer;
                }

                if (stringLength <= 0)
                {
                    AVDebug.WriteLine("Failed to get ParameterString length for: " + processHandle);
                    return parameterString;
                }

                string getString = new string(' ', stringLength);
                readResult = NtReadVirtualMemoryWOW64(processHandle, stringBuffer, getString, stringLength, out _);
                if (readResult != 0)
                {
                    AVDebug.WriteLine("Failed to get ParameterString for: " + processHandle);
                    return parameterString;
                }

                return getString;
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get GetApplicationParameter: " + ex.Message);
                return parameterString;
            }
        }
    }
}