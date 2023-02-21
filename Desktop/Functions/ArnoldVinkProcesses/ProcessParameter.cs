using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Remove executable path from commandline
        public static string CommandLine_RemoveExePath(string commandLine)
        {
            try
            {
                //Check command line
                if (string.IsNullOrWhiteSpace(commandLine))
                {
                    return commandLine;
                }

                //Remove executable path
                int endIndex = 0;
                bool inQuotes = false;
                foreach (char commandChar in commandLine)
                {
                    if (commandChar == '"')
                    {
                        inQuotes = !inQuotes;
                    }
                    else if (!inQuotes && commandChar == ' ')
                    {
                        break;
                    }
                    endIndex++;
                }
                commandLine = commandLine.Substring(endIndex);

                //Trim command line
                commandLine = commandLine.Trim();
            }
            catch { }
            return commandLine;
        }

        public static string GetProcessParameterString(int processId, USER_PROCESS_PARAMETERS requestedProcessParameter)
        {
            string parameterString = string.Empty;
            try
            {
                //Open the process for reading
                IntPtr openProcessHandle = OpenProcess(ProcessAccessFlags.QueryInformation | ProcessAccessFlags.VirtualMemoryRead, false, processId);
                if (openProcessHandle == IntPtr.Zero)
                {
                    //Debug.WriteLine("Failed to open the process: " + processId);
                    return parameterString;
                }

                //Check if Windows is 64 bit
                bool Windows64bits = IntPtr.Size > 4;

                //Set the parameter offset
                long userParameterOffset = 0;
                long processParametersOffset = Windows64bits ? 0x20 : 0x10;
                if (requestedProcessParameter == USER_PROCESS_PARAMETERS.CurrentDirectoryPath) { userParameterOffset = Windows64bits ? 0x38 : 0x24; }
                else if (requestedProcessParameter == USER_PROCESS_PARAMETERS.ImagePathName) { userParameterOffset = Windows64bits ? 0x60 : 0x38; }
                else if (requestedProcessParameter == USER_PROCESS_PARAMETERS.CommandLine) { userParameterOffset = Windows64bits ? 0x70 : 0x40; }

                //Read information from process
                PROCESS_BASIC_INFORMATION process_basic_information = new PROCESS_BASIC_INFORMATION();
                int ntQuery = NtQueryInformationProcess(openProcessHandle, PROCESSINFOCLASS.ProcessBasicInformation, ref process_basic_information, process_basic_information.Size, IntPtr.Zero);
                if (ntQuery != 0)
                {
                    Debug.WriteLine("Failed to query information, from process: " + processId);
                    return parameterString;
                }

                IntPtr process_parameter = new IntPtr();
                long pebBaseAddress = process_basic_information.PebBaseAddress.ToInt64();
                if (!ReadProcessMemory(openProcessHandle, new IntPtr(pebBaseAddress + processParametersOffset), ref process_parameter, new IntPtr(Marshal.SizeOf(process_parameter)), IntPtr.Zero))
                {
                    Debug.WriteLine("Failed to read parameter address, from process: " + processId);
                    return parameterString;
                }

                UNICODE_string unicode_string = new UNICODE_string();
                if (!ReadProcessMemory(openProcessHandle, new IntPtr(process_parameter.ToInt64() + userParameterOffset), ref unicode_string, new IntPtr(unicode_string.Size), IntPtr.Zero))
                {
                    Debug.WriteLine("Failed to read parameter unicode, from process: " + processId);
                    return parameterString;
                }

                string converted_string = new string(' ', unicode_string.Length);
                if (!ReadProcessMemory(openProcessHandle, unicode_string.Buffer, converted_string, new IntPtr(unicode_string.Length), IntPtr.Zero))
                {
                    Debug.WriteLine("Failed to read parameter string, from process: " + processId);
                    return parameterString;
                }
                parameterString = converted_string;

                //Remove executable path from commandline
                if (requestedProcessParameter == USER_PROCESS_PARAMETERS.CommandLine)
                {
                    parameterString = CommandLine_RemoveExePath(parameterString);
                }

                CloseHandleAuto(openProcessHandle);
            }
            catch { }
            return parameterString;
        }

        //Enumerators
        public enum USER_PROCESS_PARAMETERS
        {
            CurrentDirectoryPath,
            ImagePathName,
            CommandLine
        };
        private enum PROCESSINFOCLASS : int
        {
            ProcessBasicInformation = 0
        }

        //Structures
        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_BASIC_INFORMATION
        {
            public IntPtr ExitStatus;
            public IntPtr PebBaseAddress;
            public IntPtr AffinityMask;
            public IntPtr BasePriority;
            public UIntPtr UniqueProcessId;
            public IntPtr InheritedFromUniqueProcessId;
            public int Size
            {
                get { return Marshal.SizeOf(typeof(PROCESS_BASIC_INFORMATION)); }
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct UNICODE_string
        {
            public ushort Length;
            public ushort MaximumLength;
            public IntPtr Buffer;
            public int Size
            {
                get { return Marshal.SizeOf(typeof(UNICODE_string)); }
            }
        }

        //DllImports
        [DllImport("ntdll.dll")]
        private static extern int NtQueryInformationProcess(IntPtr hProcess, PROCESSINFOCLASS pic, ref PROCESS_BASIC_INFORMATION pbi, int cb, IntPtr pSize);
        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref IntPtr lpBuffer, IntPtr dwSize, IntPtr lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref UNICODE_string lpBuffer, IntPtr dwSize, IntPtr lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [MarshalAs(UnmanagedType.LPWStr)] string lpBuffer, IntPtr dwSize, IntPtr lpNumberOfBytesRead);
    }
}