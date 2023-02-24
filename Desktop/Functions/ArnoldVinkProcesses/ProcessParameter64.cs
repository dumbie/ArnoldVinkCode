using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Imports
        [DllImport("ntdll.dll", EntryPoint = "NtWow64QueryInformationProcess64")]
        private static extern int NtQueryInformationProcess64(IntPtr ProcessHandle, __PROCESS_INFO_CLASS ProcessInformationClass, ref __PROCESS_BASIC_INFORMATION64 ProcessInformation, uint ProcessInformationLength, out uint ReturnLength);

        [DllImport("ntdll.dll", EntryPoint = "NtWow64ReadVirtualMemory64")]
        private static extern int NtReadVirtualMemory64(IntPtr ProcessHandle, long BaseAddress, ref __PEB64 Buffer, ulong NumberOfBytesToRead, out ulong NumberOfBytesRead);

        [DllImport("ntdll.dll", EntryPoint = "NtWow64ReadVirtualMemory64")]
        private static extern int NtReadVirtualMemory64(IntPtr ProcessHandle, long BaseAddress, ref __RTL_USER_PROCESS_PARAMETERS64 Buffer, ulong NumberOfBytesToRead, out ulong NumberOfBytesRead);

        [DllImport("ntdll.dll", EntryPoint = "NtWow64ReadVirtualMemory64")]
        private static extern int NtReadVirtualMemory64(IntPtr ProcessHandle, long BaseAddress, [MarshalAs(UnmanagedType.LPWStr)] string Buffer, ulong NumberOfBytesToRead, out ulong NumberOfBytesRead);

        //Structures
        [StructLayout(LayoutKind.Sequential)]
        private struct __PROCESS_BASIC_INFORMATION64
        {
            public int ExitStatus;
            public long PebBaseAddress;
            public long AffinityMask;
            public int BasePriority;
            public long UniqueProcessId;
            public long InheritedFromUniqueProcessId;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct __PEB64
        {
            public long Reserved0;
            public long Reserved1;
            public long Reserved2;
            public long Reserved3;
            public long RtlUserProcessParameters;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct __UNICODE_STRING64
        {
            public ushort Length;
            public ushort MaximumLength;
            public long Buffer;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct __RTL_DRIVE_LETTER_CURDIR64
        {
            public ushort Flags;
            public ushort Length;
            public uint TimeStamp;
            public __UNICODE_STRING64 DosPath;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct __RTL_USER_PROCESS_PARAMETERS64
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
            public __UNICODE_STRING64 CurrentDirectory;
            public long CurrentDirectoryHandle;
            public __UNICODE_STRING64 DllPath;
            public __UNICODE_STRING64 ImagePathName;
            public __UNICODE_STRING64 CommandLine;
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
            public __UNICODE_STRING64 WindowTitle;
            public __UNICODE_STRING64 DesktopInfo;
            public __UNICODE_STRING64 ShellInfo;
            public __UNICODE_STRING64 RuntimeData;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public __RTL_DRIVE_LETTER_CURDIR64[] CurrentDirectores;
            public uint EnvironmentSize;
        }

        //Methods
        private static string GetApplicationParameter64(IntPtr processHandle, __PROCESS_PARAMETER_OPTIONS pOption)
        {
            string parameterString = string.Empty;
            try
            {
                Debug.WriteLine("GetApplicationParameter architecture 64");

                __PROCESS_BASIC_INFORMATION64 basicInformation = new __PROCESS_BASIC_INFORMATION64();
                int readResult = NtQueryInformationProcess64(processHandle, __PROCESS_INFO_CLASS.ProcessBasicInformation, ref basicInformation, (uint)Marshal.SizeOf(basicInformation), out _);
                if (readResult != 0)
                {
                    Debug.WriteLine("Failed to get ProcessBasicInformation for: " + processHandle);
                    return parameterString;
                }

                __PEB64 pebCopy = new __PEB64();
                readResult = NtReadVirtualMemory64(processHandle, basicInformation.PebBaseAddress, ref pebCopy, (uint)Marshal.SizeOf(pebCopy), out _);
                if (readResult != 0)
                {
                    Debug.WriteLine("Failed to get PebBaseAddress for: " + processHandle);
                    return parameterString;
                }

                __RTL_USER_PROCESS_PARAMETERS64 paramsCopy = new __RTL_USER_PROCESS_PARAMETERS64();
                readResult = NtReadVirtualMemory64(processHandle, pebCopy.RtlUserProcessParameters, ref paramsCopy, (uint)Marshal.SizeOf(paramsCopy), out _);
                if (readResult != 0)
                {
                    Debug.WriteLine("Failed to get ProcessParameters for: " + processHandle);
                    return parameterString;
                }

                ushort stringLength = 0;
                long stringBuffer = 0;
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
                readResult = NtReadVirtualMemory64(processHandle, stringBuffer, getString, stringLength, out _);
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