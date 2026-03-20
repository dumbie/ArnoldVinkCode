using System;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Imports
        [DllImport("ntdll.dll", EntryPoint = "NtQueryInformationProcess")]
        private static extern uint NtQueryInformationProcess32(IntPtr ProcessHandle, ProcessInfoClass ProcessInformationClass, ref __PROCESS_BASIC_INFORMATION32 ProcessInformation, uint ProcessInformationLength, out uint ReturnLength);

        [DllImport("ntdll.dll", EntryPoint = "NtReadVirtualMemory")]
        private static extern uint NtReadVirtualMemory32(IntPtr ProcessHandle, IntPtr BaseAddress, ref __PEB32 Buffer, uint NumberOfBytesToRead, out uint NumberOfBytesRead);

        [DllImport("ntdll.dll", EntryPoint = "NtReadVirtualMemory")]
        private static extern uint NtReadVirtualMemory32(IntPtr ProcessHandle, IntPtr BaseAddress, ref __RTL_USER_PROCESS_PARAMETERS32 Buffer, uint NumberOfBytesToRead, out uint NumberOfBytesRead);

        [DllImport("ntdll.dll", EntryPoint = "NtReadVirtualMemory")]
        private static extern uint NtReadVirtualMemory32(IntPtr ProcessHandle, IntPtr BaseAddress, [MarshalAs(UnmanagedType.LPWStr)] string Buffer, uint NumberOfBytesToRead, out uint NumberOfBytesRead);

        //Structures
        [StructLayout(LayoutKind.Sequential)]
        private struct __PROCESS_BASIC_INFORMATION32
        {
            public int ExitStatus;
            public IntPtr PebBaseAddress;
            public IntPtr AffinityMask;
            public int BasePriority;
            public IntPtr UniqueProcessId;
            public IntPtr InheritedFromUniqueProcessId;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct __PEB32
        {
            public IntPtr Reserved0;
            public IntPtr Reserved1;
            public IntPtr Reserved2;
            public IntPtr Reserved3;
            public IntPtr RtlUserProcessParameters;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct __UNICODE_STRING32
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
            public __UNICODE_STRING32 DosPath;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct __RTL_USER_PROCESS_PARAMETERS32
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
            public __UNICODE_STRING32 CurrentDirectory;
            public IntPtr CurrentDirectoryHandle;
            public __UNICODE_STRING32 DllPath;
            public __UNICODE_STRING32 ImagePathName;
            public __UNICODE_STRING32 CommandLine;
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
            public __UNICODE_STRING32 WindowTitle;
            public __UNICODE_STRING32 DesktopInfo;
            public __UNICODE_STRING32 ShellInfo;
            public __UNICODE_STRING32 RuntimeData;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public __RTL_DRIVE_LETTER_CURDIR32[] CurrentDirectores;
            public uint EnvironmentSize;
        }

        //Methods
        private static string GetApplicationParameter32(IntPtr processHandle, ProcessParameterOptions pOption)
        {
            try
            {
                //AVDebug.WriteLine("GetApplicationParameter architecture 32");

                __PROCESS_BASIC_INFORMATION32 basicInformation = new __PROCESS_BASIC_INFORMATION32();
                uint readResult = NtQueryInformationProcess32(processHandle, ProcessInfoClass.ProcessBasicInformation, ref basicInformation, (uint)Marshal.SizeOf(basicInformation), out _);
                if (readResult != 0)
                {
                    //AVDebug.WriteLine("Failed to get ProcessBasicInformation for: " + processHandle + "/Query failed.");
                    return string.Empty;
                }

                __PEB32 pebCopy = new __PEB32();
                readResult = NtReadVirtualMemory32(processHandle, basicInformation.PebBaseAddress, ref pebCopy, (uint)Marshal.SizeOf(pebCopy), out _);
                if (readResult != 0)
                {
                    //AVDebug.WriteLine("Failed to get PebBaseAddress for: " + processHandle);
                    return string.Empty;
                }

                __RTL_USER_PROCESS_PARAMETERS32 paramsCopy = new __RTL_USER_PROCESS_PARAMETERS32();
                readResult = NtReadVirtualMemory32(processHandle, pebCopy.RtlUserProcessParameters, ref paramsCopy, (uint)Marshal.SizeOf(paramsCopy), out _);
                if (readResult != 0)
                {
                    //AVDebug.WriteLine("Failed to get ProcessParameters for: " + processHandle);
                    return string.Empty;
                }

                ushort stringLength = 0;
                IntPtr stringBuffer = IntPtr.Zero;
                if (pOption == ProcessParameterOptions.CurrentDirectoryPath)
                {
                    stringLength = paramsCopy.CurrentDirectory.Length;
                    stringBuffer = paramsCopy.CurrentDirectory.Buffer;
                }
                else if (pOption == ProcessParameterOptions.ImagePathName)
                {
                    stringLength = paramsCopy.ImagePathName.Length;
                    stringBuffer = paramsCopy.ImagePathName.Buffer;
                }
                else if (pOption == ProcessParameterOptions.DesktopInfo)
                {
                    stringLength = paramsCopy.DesktopInfo.Length;
                    stringBuffer = paramsCopy.DesktopInfo.Buffer;
                }
                else if (pOption == ProcessParameterOptions.Environment)
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
                    //AVDebug.WriteLine("Failed to get ParameterString length for: " + processHandle);
                    return string.Empty;
                }

                string getString = new string(' ', stringLength);
                readResult = NtReadVirtualMemory32(processHandle, stringBuffer, getString, stringLength, out _);
                if (readResult != 0)
                {
                    AVDebug.WriteLine("Failed to get ParameterString for: " + processHandle);
                    return string.Empty;
                }
                else
                {
                    //AVDebug.WriteLine("Got ParameterString: " + processHandle + "/" + getString);
                    return getString;
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get GetApplicationParameter: " + ex.Message);
                return string.Empty;
            }
        }
    }
}