using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Imports
        [DllImport("ntdll.dll", EntryPoint = "NtQueryInformationProcess")]
        private static extern uint NtQueryInformationProcess64(IntPtr ProcessHandle, ProcessInfoClass ProcessInformationClass, ref ulong ProcessInformation, uint ProcessInformationLength, out uint ReturnLength);

        [DllImport("ntdll.dll", EntryPoint = "NtReadVirtualMemory")]
        private static extern uint NtReadVirtualMemory64(IntPtr ProcessHandle, ulong BaseAddress, ref __PEB64 Buffer, ulong NumberOfBytesToRead, out ulong NumberOfBytesRead);

        [DllImport("ntdll.dll", EntryPoint = "NtReadVirtualMemory")]
        private static extern uint NtReadVirtualMemory64(IntPtr ProcessHandle, ulong BaseAddress, ref __RTL_USER_PROCESS_PARAMETERS64 Buffer, ulong NumberOfBytesToRead, out ulong NumberOfBytesRead);

        [DllImport("ntdll.dll", EntryPoint = "NtReadVirtualMemory")]
        private static extern uint NtReadVirtualMemory64(IntPtr ProcessHandle, ulong BaseAddress, ref __PEB_LDR_DATA64 Buffer, ulong NumberOfBytesToRead, out ulong NumberOfBytesRead);

        [DllImport("ntdll.dll", EntryPoint = "NtReadVirtualMemory")]
        private static extern uint NtReadVirtualMemory64(IntPtr ProcessHandle, ulong BaseAddress, ref __LDR_DATA_TABLE_ENTRY64 Buffer, ulong NumberOfBytesToRead, out ulong NumberOfBytesRead);

        [DllImport("ntdll.dll", EntryPoint = "NtReadVirtualMemory")]
        private static extern uint NtReadVirtualMemory64(IntPtr ProcessHandle, ulong BaseAddress, [MarshalAs(UnmanagedType.LPWStr)] string Buffer, ulong NumberOfBytesToRead, out ulong NumberOfBytesRead);

        //Structures
        [StructLayout(LayoutKind.Sequential)]
        public struct __PROCESS_BASIC_INFORMATION64
        {
            public int ExitStatus;
            public uint PebBaseAddress;
            public uint AffinityMask;
            public int BasePriority;
            public uint UniqueProcessId;
            public uint InheritedFromUniqueProcessId;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct __PEB64
        {
            public uint Reserved0;
            public uint Reserved1;
            public uint Reserved2;
            public uint LdrData;
            public uint RtlUserProcessParameters;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct __UNICODE_STRING64
        {
            public ushort Length;
            public ushort MaximumLength;
            public uint Buffer;
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
            public uint ConsoleHandle;
            public uint ConsoleFlags;
            public uint StandardInput;
            public uint StandardOutput;
            public uint StandardError;
            public __UNICODE_STRING64 CurrentDirectory;
            public uint CurrentDirectoryHandle;
            public __UNICODE_STRING64 DllPath;
            public __UNICODE_STRING64 ImagePathName;
            public __UNICODE_STRING64 CommandLine;
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
            public __UNICODE_STRING64 WindowTitle;
            public __UNICODE_STRING64 DesktopInfo;
            public __UNICODE_STRING64 ShellInfo;
            public __UNICODE_STRING64 RuntimeData;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public __RTL_DRIVE_LETTER_CURDIR64[] CurrentDirectores;
            public uint EnvironmentSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct __LIST_ENTRY64
        {
            public uint Flink;
            public uint Blink;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct __PEB_LDR_DATA64
        {
            public uint Length;
            public byte Initialized;
            public uint SsHandle;
            public __LIST_ENTRY64 InLoadOrderModuleList;
            public __LIST_ENTRY64 InMemoryOrderModuleList;
            public __LIST_ENTRY64 InInitializationOrderModuleList;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct __LDR_DATA_TABLE_ENTRY64
        {
            public __LIST_ENTRY64 InLoadOrderLinks;
            public __LIST_ENTRY64 InMemoryOrderLinks;
            public __LIST_ENTRY64 InInitializationOrderLinks;
            public uint DllBase;
            public uint EntryPoint;
            public uint SizeOfImage;
            public __UNICODE_STRING64 FullDllName;
            public __UNICODE_STRING64 BaseDllName;
        }

        //Methods
        private static string GetApplicationParameter64(IntPtr processHandle, ProcessParameterOptions pOption)
        {
            try
            {
                //AVDebug.WriteLine("GetApplicationParameter architecture 64");

                ulong pebBaseAddress = 0;
                uint readResult = NtQueryInformationProcess64(processHandle, ProcessInfoClass.ProcessWow64Information, ref pebBaseAddress, (uint)Marshal.SizeOf(pebBaseAddress), out _);
                if (readResult != 0)
                {
                    //AVDebug.WriteLine("Failed to get ProcessWow64Information for: " + processHandle + "/Query failed.");
                    return string.Empty;
                }

                __PEB64 pebCopy = new __PEB64();
                readResult = NtReadVirtualMemory64(processHandle, pebBaseAddress, ref pebCopy, (uint)Marshal.SizeOf(pebCopy), out _);
                if (readResult != 0)
                {
                    //AVDebug.WriteLine("Failed to get PebBaseAddress for: " + processHandle);
                    return string.Empty;
                }

                __RTL_USER_PROCESS_PARAMETERS64 paramsCopy = new __RTL_USER_PROCESS_PARAMETERS64();
                readResult = NtReadVirtualMemory64(processHandle, pebCopy.RtlUserProcessParameters, ref paramsCopy, (uint)Marshal.SizeOf(paramsCopy), out _);
                if (readResult != 0)
                {
                    //AVDebug.WriteLine("Failed to get ProcessParameters for: " + processHandle);
                    return string.Empty;
                }

                ushort stringLength = 0;
                uint stringBuffer = 0;
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
                readResult = NtReadVirtualMemory64(processHandle, stringBuffer, getString, stringLength, out _);
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

        private static List<string> GetApplicationModules64(IntPtr processHandle)
        {
            List<string> processModules = new List<string>();
            try
            {
                //AVDebug.WriteLine("GetApplicationModules architecture 64");

                ulong pebBaseAddress = 0;
                uint readResult = NtQueryInformationProcess64(processHandle, ProcessInfoClass.ProcessWow64Information, ref pebBaseAddress, (uint)Marshal.SizeOf(pebBaseAddress), out _);
                if (readResult != 0)
                {
                    //AVDebug.WriteLine("Failed to get ProcessWow64Information for: " + processHandle + "/Query failed.");
                    return processModules;
                }

                __PEB64 pebCopy = new __PEB64();
                readResult = NtReadVirtualMemory64(processHandle, pebBaseAddress, ref pebCopy, (uint)Marshal.SizeOf(pebCopy), out _);
                if (readResult != 0)
                {
                    //AVDebug.WriteLine("Failed to get PebBaseAddress for: " + processHandle);
                    return processModules;
                }

                __PEB_LDR_DATA64 ldrData = new __PEB_LDR_DATA64();
                readResult = NtReadVirtualMemory64(processHandle, pebCopy.LdrData, ref ldrData, (uint)Marshal.SizeOf(ldrData), out _);
                if (readResult != 0)
                {
                    //AVDebug.WriteLine("Failed to get LdrData for: " + processHandle);
                    return processModules;
                }

                //Loop to get the module names
                uint moduleFlinkStart = ldrData.InLoadOrderModuleList.Flink;
                uint moduleFlinkNext = moduleFlinkStart;
                while (true)
                {
                    try
                    {
                        //Get module info
                        __LDR_DATA_TABLE_ENTRY64 moduleInfo = new __LDR_DATA_TABLE_ENTRY64();
                        readResult = NtReadVirtualMemory64(processHandle, moduleFlinkNext, ref moduleInfo, (uint)Marshal.SizeOf(moduleInfo), out _);
                        if (readResult != 0)
                        {
                            continue;
                        }

                        //Get module name
                        string getString = new string(' ', moduleInfo.BaseDllName.Length);
                        readResult = NtReadVirtualMemory64(processHandle, moduleInfo.BaseDllName.Buffer, getString, moduleInfo.BaseDllName.Length, out _);
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