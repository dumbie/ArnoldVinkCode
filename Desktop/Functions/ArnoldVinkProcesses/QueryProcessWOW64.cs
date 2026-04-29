using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Imports
        [DllImport("ntdll.dll", EntryPoint = "NtWow64QueryInformationProcess64")]
        private static extern uint NtQueryInformationProcessWOW64(IntPtr ProcessHandle, ProcessInfoClass ProcessInformationClass, ref __PROCESS_BASIC_INFORMATIONWOW64 ProcessInformation, uint ProcessInformationLength, out uint ReturnLength);

        [DllImport("ntdll.dll", EntryPoint = "NtWow64ReadVirtualMemory64")]
        private static extern uint NtReadVirtualMemoryWOW64(IntPtr ProcessHandle, long BaseAddress, ref __PEBWOW64 Buffer, ulong NumberOfBytesToRead, out ulong NumberOfBytesRead);

        [DllImport("ntdll.dll", EntryPoint = "NtWow64ReadVirtualMemory64")]
        private static extern uint NtReadVirtualMemoryWOW64(IntPtr ProcessHandle, long BaseAddress, ref __RTL_USER_PROCESS_PARAMETERSWOW64 Buffer, ulong NumberOfBytesToRead, out ulong NumberOfBytesRead);

        [DllImport("ntdll.dll", EntryPoint = "NtWow64ReadVirtualMemory64")]
        private static extern uint NtReadVirtualMemoryWOW64(IntPtr ProcessHandle, long BaseAddress, ref __PEB_LDR_DATAWOW64 Buffer, ulong NumberOfBytesToRead, out ulong NumberOfBytesRead);

        [DllImport("ntdll.dll", EntryPoint = "NtWow64ReadVirtualMemory64")]
        private static extern uint NtReadVirtualMemoryWOW64(IntPtr ProcessHandle, long BaseAddress, ref __LDR_DATA_TABLE_ENTRYWOW64 Buffer, ulong NumberOfBytesToRead, out ulong NumberOfBytesRead);

        [DllImport("ntdll.dll", EntryPoint = "NtWow64ReadVirtualMemory64")]
        private static extern uint NtReadVirtualMemoryWOW64(IntPtr ProcessHandle, long BaseAddress, [MarshalAs(UnmanagedType.LPWStr)] string Buffer, ulong NumberOfBytesToRead, out ulong NumberOfBytesRead);

        //Structures
        [StructLayout(LayoutKind.Sequential)]
        private struct __PROCESS_BASIC_INFORMATIONWOW64
        {
            public int ExitStatus;
            public long PebBaseAddress;
            public long AffinityMask;
            public int BasePriority;
            public long UniqueProcessId;
            public long InheritedFromUniqueProcessId;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct __PEBWOW64
        {
            public long Reserved0;
            public long Reserved1;
            public long Reserved2;
            public long LdrData;
            public long RtlUserProcessParameters;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct __UNICODE_STRINGWOW64
        {
            public ushort Length;
            public ushort MaximumLength;
            public long Buffer;
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
            public long ConsoleHandle;
            public uint ConsoleFlags;
            public long StandardInput;
            public long StandardOutput;
            public long StandardError;
            public __UNICODE_STRINGWOW64 CurrentDirectory;
            public long CurrentDirectoryHandle;
            public __UNICODE_STRINGWOW64 DllPath;
            public __UNICODE_STRINGWOW64 ImagePathName;
            public __UNICODE_STRINGWOW64 CommandLine;
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
            public __UNICODE_STRINGWOW64 WindowTitle;
            public __UNICODE_STRINGWOW64 DesktopInfo;
            public __UNICODE_STRINGWOW64 ShellInfo;
            public __UNICODE_STRINGWOW64 RuntimeData;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public __RTL_DRIVE_LETTER_CURDIRWOW64[] CurrentDirectores;
            public uint EnvironmentSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct __LIST_ENTRYWOW64
        {
            public long Flink;
            public long Blink;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct __PEB_LDR_DATAWOW64
        {
            public uint Length;
            public byte Initialized;
            public long SsHandle;
            public __LIST_ENTRYWOW64 InLoadOrderModuleList;
            public __LIST_ENTRYWOW64 InMemoryOrderModuleList;
            public __LIST_ENTRYWOW64 InInitializationOrderModuleList;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct __LDR_DATA_TABLE_ENTRYWOW64
        {
            public __LIST_ENTRYWOW64 InLoadOrderLinks;
            public __LIST_ENTRYWOW64 InMemoryOrderLinks;
            public __LIST_ENTRYWOW64 InInitializationOrderLinks;
            public long DllBase;
            public long EntryPoint;
            public uint SizeOfImage;
            public __UNICODE_STRINGWOW64 FullDllName;
            public __UNICODE_STRINGWOW64 BaseDllName;
        }

        //Methods
        private static string GetApplicationParameterWOW64(IntPtr processHandle, ProcessParameterOptions pOption)
        {
            try
            {
                //AVDebug.WriteLine("GetApplicationParameter architecture WOW64");

                __PROCESS_BASIC_INFORMATIONWOW64 basicInformation = new __PROCESS_BASIC_INFORMATIONWOW64();
                uint readResult = NtQueryInformationProcessWOW64(processHandle, ProcessInfoClass.ProcessBasicInformation, ref basicInformation, (uint)Marshal.SizeOf(basicInformation), out _);
                if (readResult != 0)
                {
                    //AVDebug.WriteLine("Failed to get ProcessBasicInformation for: " + processHandle + "/Query failed.");
                    return string.Empty;
                }

                __PEBWOW64 pebCopy = new __PEBWOW64();
                readResult = NtReadVirtualMemoryWOW64(processHandle, basicInformation.PebBaseAddress, ref pebCopy, (uint)Marshal.SizeOf(pebCopy), out _);
                if (readResult != 0)
                {
                    //AVDebug.WriteLine("Failed to get PebBaseAddress for: " + processHandle);
                    return string.Empty;
                }

                __RTL_USER_PROCESS_PARAMETERSWOW64 paramsCopy = new __RTL_USER_PROCESS_PARAMETERSWOW64();
                readResult = NtReadVirtualMemoryWOW64(processHandle, pebCopy.RtlUserProcessParameters, ref paramsCopy, (uint)Marshal.SizeOf(paramsCopy), out _);
                if (readResult != 0)
                {
                    //AVDebug.WriteLine("Failed to get ProcessParameters for: " + processHandle);
                    return string.Empty;
                }

                ushort stringLength = 0;
                long stringBuffer = 0;
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
                readResult = NtReadVirtualMemoryWOW64(processHandle, stringBuffer, getString, stringLength, out _);
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

        private static List<string> GetApplicationModulesWOW64(IntPtr processHandle)
        {
            List<string> processModules = new List<string>();
            try
            {
                //AVDebug.WriteLine("GetApplicationModules architecture WOW64");

                __PROCESS_BASIC_INFORMATIONWOW64 basicInformation = new __PROCESS_BASIC_INFORMATIONWOW64();
                uint readResult = NtQueryInformationProcessWOW64(processHandle, ProcessInfoClass.ProcessBasicInformation, ref basicInformation, (uint)Marshal.SizeOf(basicInformation), out _);
                if (readResult != 0)
                {
                    //AVDebug.WriteLine("Failed to get ProcessBasicInformation for: " + processHandle + "/Query failed.");
                    return processModules;
                }

                __PEBWOW64 pebCopy = new __PEBWOW64();
                readResult = NtReadVirtualMemoryWOW64(processHandle, basicInformation.PebBaseAddress, ref pebCopy, (uint)Marshal.SizeOf(pebCopy), out _);
                if (readResult != 0)
                {
                    //AVDebug.WriteLine("Failed to get PebBaseAddress for: " + processHandle);
                    return processModules;
                }

                __PEB_LDR_DATAWOW64 ldrData = new __PEB_LDR_DATAWOW64();
                readResult = NtReadVirtualMemoryWOW64(processHandle, pebCopy.LdrData, ref ldrData, (uint)Marshal.SizeOf(ldrData), out _);
                if (readResult != 0)
                {
                    //AVDebug.WriteLine("Failed to get LdrData for: " + processHandle);
                    return processModules;
                }

                //Loop to get the module names
                long moduleFlinkStart = ldrData.InLoadOrderModuleList.Flink;
                long moduleFlinkNext = moduleFlinkStart;
                while (true)
                {
                    try
                    {
                        //Get module info
                        __LDR_DATA_TABLE_ENTRYWOW64 moduleInfo = new __LDR_DATA_TABLE_ENTRYWOW64();
                        readResult = NtReadVirtualMemoryWOW64(processHandle, moduleFlinkNext, ref moduleInfo, (uint)Marshal.SizeOf(moduleInfo), out _);
                        if (readResult != 0)
                        {
                            continue;
                        }

                        //Get module name
                        string getString = new string(' ', moduleInfo.BaseDllName.Length);
                        readResult = NtReadVirtualMemoryWOW64(processHandle, moduleInfo.BaseDllName.Buffer, getString, moduleInfo.BaseDllName.Length, out _);
                        if (readResult == 0)
                        {
                            if (!string.IsNullOrWhiteSpace(getString))
                            {
                                //AVDebug.WriteLine("Got module name: " + i + " / " + getString);
                                processModules.Add(getString.Trim());
                            }
                        }

                        //Move to next module
                        moduleFlinkNext = moduleInfo.InLoadOrderLinks.Flink;
                        if (moduleFlinkNext == moduleFlinkStart)
                        {
                            break;
                        }
                    }
                    catch { }
                }
                return processModules;
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get GetApplicationModules: " + ex.Message);
                return processModules;
            }
        }
    }
}