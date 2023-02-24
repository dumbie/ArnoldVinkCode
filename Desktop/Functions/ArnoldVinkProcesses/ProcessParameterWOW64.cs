using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Imports
        [DllImport("ntdll.dll", EntryPoint = "NtQueryInformationProcess")]
        private static extern int NtQueryInformationProcessWow64(IntPtr ProcessHandle, __PROCESS_INFO_CLASS ProcessInformationClass, ref ulong ProcessInformation, uint ProcessInformationLength, out uint ReturnLength);

        [DllImport("ntdll.dll", EntryPoint = "NtReadVirtualMemory")]
        private static extern int NtReadVirtualMemoryWow64(IntPtr ProcessHandle, ulong BaseAddress, ref __PEBWOW64 Buffer, ulong NumberOfBytesToRead, out ulong NumberOfBytesRead);

        [DllImport("ntdll.dll", EntryPoint = "NtReadVirtualMemory")]
        private static extern int NtReadVirtualMemoryWow64(IntPtr ProcessHandle, ulong BaseAddress, ref __RTL_USER_PROCESS_PARAMETERSWOW64 Buffer, ulong NumberOfBytesToRead, out ulong NumberOfBytesRead);

        [DllImport("ntdll.dll", EntryPoint = "NtReadVirtualMemory")]
        private static extern int NtReadVirtualMemoryWow64(IntPtr ProcessHandle, ulong BaseAddress, [MarshalAs(UnmanagedType.LPWStr)] string Buffer, ulong NumberOfBytesToRead, out ulong NumberOfBytesRead);

        //Structures
        [StructLayout(LayoutKind.Sequential)]
        public struct __PEBWOW64
        {
            public uint Reserved0;
            public uint Reserved1;
            public uint Reserved2;
            public uint Reserved3;
            public uint RtlUserProcessParameters;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct __UNICODE_STRINGWOW64
        {
            public ushort Length;
            public ushort MaximumLength;
            public uint Buffer;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct __RTL_DRIVE_LETTER_CURDIRWOW64
        {
            public ushort Flags;
            public ushort Length;
            public uint TimeStamp;
            public __UNICODE_STRINGWOW64 DosPath;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct __RTL_USER_PROCESS_PARAMETERSWOW64
        {
            public uint MaximumLength;
            public uint Length;
            public uint Flags;
            public uint DebugFlags;
            public uint ConsoleHandle;
            public uint ConsoleFlags;
            public uint StandardInput;
            public uint StandardOutput;
            public uint StandardError;
            public __UNICODE_STRINGWOW64 CurrentDirectory;
            public uint CurrentDirectoryHandle;
            public __UNICODE_STRINGWOW64 DllPath;
            public __UNICODE_STRINGWOW64 ImagePathName;
            public __UNICODE_STRINGWOW64 CommandLine;
            public uint Environment;
            public uint StartingX;
            public uint StartingY;
            public uint CountX;
            public uint CountY;
            public uint CountCharsX;
            public uint CountCharsY;
            public uint FillAttribute;
            public uint WindowFlags;
            public uint ShowWindowFlags;
            public __UNICODE_STRINGWOW64 WindowTitle;
            public __UNICODE_STRINGWOW64 DesktopInfo;
            public __UNICODE_STRINGWOW64 ShellInfo;
            public __UNICODE_STRINGWOW64 RuntimeData;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public __RTL_DRIVE_LETTER_CURDIRWOW64[] CurrentDirectores;
            public uint EnvironmentSize;
        }

        //Methods
        private static string GetApplicationParameterWOW64(IntPtr processHandle, __PROCESS_PARAMETER_OPTIONS pOption)
        {
            string parameterString = string.Empty;
            try
            {
                Debug.WriteLine("GetApplicationParameter architecture WOW64");

                ulong pebBaseAddress = 0;
                int readResult = NtQueryInformationProcessWow64(processHandle, __PROCESS_INFO_CLASS.ProcessWow64Information, ref pebBaseAddress, (uint)Marshal.SizeOf(pebBaseAddress), out _);
                if (readResult != 0)
                {
                    Debug.WriteLine("Failed to get ProcessBasicInformation for: " + processHandle);
                    return parameterString;
                }

                __PEBWOW64 pebCopy = new __PEBWOW64();
                readResult = NtReadVirtualMemoryWow64(processHandle, pebBaseAddress, ref pebCopy, (uint)Marshal.SizeOf(pebCopy), out _);
                if (readResult != 0)
                {
                    Debug.WriteLine("Failed to get PebBaseAddress for: " + processHandle);
                    return parameterString;
                }

                __RTL_USER_PROCESS_PARAMETERSWOW64 paramsCopy = new __RTL_USER_PROCESS_PARAMETERSWOW64();
                readResult = NtReadVirtualMemoryWow64(processHandle, pebCopy.RtlUserProcessParameters, ref paramsCopy, (uint)Marshal.SizeOf(paramsCopy), out _);
                if (readResult != 0)
                {
                    Debug.WriteLine("Failed to get ProcessParameters for: " + processHandle);
                    return parameterString;
                }

                ushort stringLength = 0;
                uint stringBuffer = 0;
                if (pOption == __PROCESS_PARAMETER_OPTIONS.CurrentDirectoryPath)
                {
                    stringLength = paramsCopy.CurrentDirectory.Length;
                    stringBuffer = paramsCopy.CurrentDirectory.Buffer;
                }
                else if (pOption == __PROCESS_PARAMETER_OPTIONS.ImagePathName)
                {
                    stringLength = paramsCopy.ImagePathName.Length;
                    stringBuffer = paramsCopy.ImagePathName.Buffer;
                }
                else if (pOption == __PROCESS_PARAMETER_OPTIONS.DesktopInfo)
                {
                    stringLength = paramsCopy.DesktopInfo.Length;
                    stringBuffer = paramsCopy.DesktopInfo.Buffer;
                }
                else if (pOption == __PROCESS_PARAMETER_OPTIONS.Environment)
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
                    Debug.WriteLine("Failed to get ParameterString length for: " + processHandle);
                    return parameterString;
                }

                string getString = new string(' ', stringLength);
                readResult = NtReadVirtualMemoryWow64(processHandle, stringBuffer, getString, stringLength, out _);
                if (readResult != 0)
                {
                    Debug.WriteLine("Failed to get ParameterString for: " + processHandle);
                    return parameterString;
                }

                return getString;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get GetApplicationParameter: " + ex.Message);
                return parameterString;
            }
        }
    }
}