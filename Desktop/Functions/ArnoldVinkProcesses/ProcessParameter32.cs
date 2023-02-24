using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Imports
        [DllImport("ntdll.dll", EntryPoint = "NtQueryInformationProcess")]
        private static extern int NtQueryInformationProcess32(IntPtr ProcessHandle, PROCESS_INFO_CLASS ProcessInformationClass, ref PROCESS_BASIC_INFORMATION32 ProcessInformation, uint ProcessInformationLength, out uint ReturnLength);

        [DllImport("ntdll.dll", EntryPoint = "NtReadVirtualMemory")]
        private static extern int NtReadVirtualMemory32(IntPtr ProcessHandle, IntPtr BaseAddress, ref PEB32 Buffer, uint NumberOfBytesToRead, out uint NumberOfBytesRead);

        [DllImport("ntdll.dll", EntryPoint = "NtReadVirtualMemory")]
        private static extern int NtReadVirtualMemory32(IntPtr ProcessHandle, IntPtr BaseAddress, ref RTL_USER_PROCESS_PARAMETERS32 Buffer, uint NumberOfBytesToRead, out uint NumberOfBytesRead);

        [DllImport("ntdll.dll", EntryPoint = "NtReadVirtualMemory")]
        private static extern int NtReadVirtualMemory32(IntPtr ProcessHandle, IntPtr BaseAddress, [MarshalAs(UnmanagedType.LPWStr)] string Buffer, uint NumberOfBytesToRead, out uint NumberOfBytesRead);

        //Structures
        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_BASIC_INFORMATION32
        {
            public uint ExitStatus;
            public IntPtr PebBaseAddress;
            public UIntPtr AffinityMask;
            public int BasePriority;
            public UIntPtr UniqueProcessId;
            public UIntPtr InheritedFromUniqueProcessId;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PEB32
        {
            public IntPtr Reserved0;
            public IntPtr Reserved1;
            public IntPtr Reserved2;
            public IntPtr Reserved3;
            public IntPtr RtlUserProcessParameters;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct UNICODE_STRING32
        {
            public ushort Length;
            public ushort MaximumLength;
            public IntPtr Buffer;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct __RTL_DRIVE_LETTER_CURDIR32
        {
            public ushort Flags;
            public ushort Length;
            public uint TimeStamp;
            public UNICODE_STRING32 DosPath;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RTL_USER_PROCESS_PARAMETERS32
        {
            public uint MaximumLength;
            public uint Length;
            public uint Flags;
            public uint DebugFlags;
            public IntPtr ConsoleHandle;
            public uint ConsoleFlags;
            public IntPtr StandardInput;
            public IntPtr StandardOutput;
            public IntPtr StandardError;
            public UNICODE_STRING32 CurrentDirectory;
            public IntPtr CurrentDirectoryHandle;
            public UNICODE_STRING32 DllPath;
            public UNICODE_STRING32 ImagePathName;
            public UNICODE_STRING32 CommandLine;
            public IntPtr Environment;
            public uint StartingX;
            public uint StartingY;
            public uint CountX;
            public uint CountY;
            public uint CountCharsX;
            public uint CountCharsY;
            public uint FillAttribute;
            public uint WindowFlags;
            public uint ShowWindowFlags;
            public UNICODE_STRING32 WindowTitle;
            public UNICODE_STRING32 DesktopInfo;
            public UNICODE_STRING32 ShellInfo;
            public UNICODE_STRING32 RuntimeData;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public __RTL_DRIVE_LETTER_CURDIR32[] CurrentDirectores;
            public uint EnvironmentSize;
        }

        //Methods
        private static string GetApplicationParameter32(IntPtr processHandle, PROCESS_PARAMETER_OPTIONS pOption)
        {
            string parameterString = string.Empty;
            try
            {
                //Debug.WriteLine("GetApplicationParameter architecture 32");

                PROCESS_BASIC_INFORMATION32 basicInformation = new PROCESS_BASIC_INFORMATION32();
                int readResult = NtQueryInformationProcess32(processHandle, PROCESS_INFO_CLASS.ProcessBasicInformation, ref basicInformation, (uint)Marshal.SizeOf(basicInformation), out _);
                if (readResult != 0)
                {
                    Debug.WriteLine("Failed to get ProcessBasicInformation for: " + processHandle);
                    return parameterString;
                }

                PEB32 pebCopy = new PEB32();
                readResult = NtReadVirtualMemory32(processHandle, basicInformation.PebBaseAddress, ref pebCopy, (uint)Marshal.SizeOf(pebCopy), out _);
                if (readResult != 0)
                {
                    Debug.WriteLine("Failed to get PebBaseAddress for: " + processHandle);
                    return parameterString;
                }

                RTL_USER_PROCESS_PARAMETERS32 paramsCopy = new RTL_USER_PROCESS_PARAMETERS32();
                readResult = NtReadVirtualMemory32(processHandle, pebCopy.RtlUserProcessParameters, ref paramsCopy, (uint)Marshal.SizeOf(paramsCopy), out _);
                if (readResult != 0)
                {
                    Debug.WriteLine("Failed to get ProcessParameters for: " + processHandle);
                    return parameterString;
                }

                ushort stringLength = 0;
                IntPtr stringBuffer = IntPtr.Zero;
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
                    Debug.WriteLine("Failed to get ParameterString length for: " + processHandle);
                    return parameterString;
                }

                string getString = new string(' ', stringLength);
                readResult = NtReadVirtualMemory32(processHandle, stringBuffer, getString, stringLength, out _);
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