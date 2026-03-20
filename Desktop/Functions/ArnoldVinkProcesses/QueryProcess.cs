using System;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Remove executable path from commandline
        public static string Remove_ExePathFromCommandLine(string targetCommandLine)
        {
            try
            {
                //Check command line
                if (string.IsNullOrWhiteSpace(targetCommandLine))
                {
                    return targetCommandLine;
                }

                //Remove executable path
                int endIndex = 0;
                bool inQuotes = false;
                foreach (char commandChar in targetCommandLine)
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
                targetCommandLine = targetCommandLine.Substring(endIndex);
            }
            catch { }
            return targetCommandLine;
        }

        //Get process parent id by process handle
        public static int Detail_ProcessParentIdByProcessHandle(IntPtr targetProcessHandle)
        {
            try
            {
                __PROCESS_BASIC_INFORMATION32 basicInformation = new __PROCESS_BASIC_INFORMATION32();
                uint queryResult = NtQueryInformationProcess32(targetProcessHandle, ProcessInfoClass.ProcessBasicInformation, ref basicInformation, (uint)Marshal.SizeOf(basicInformation), out _);
                if (queryResult != 0)
                {
                    AVDebug.WriteLine("Failed to get parent process id: " + targetProcessHandle + "/Query failed.");
                    return 0;
                }
                else
                {
                    return (int)basicInformation.InheritedFromUniqueProcessId;
                }
            }
            catch
            {
                //AVDebug.WriteLine("Failed to get parent processid: " + targetProcessHandle + "/" + ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// Get process parameter by process handle
        /// </summary>
        /// <summary>Process handle with VM_READ access is required.</summary>
        public static string Detail_ParameterByProcessHandle(IntPtr targetProcessHandle, ProcessParameterOptions pOption)
        {
            string parameterString = string.Empty;
            try
            {
                //Check target process handle
                if (targetProcessHandle == IntPtr.Zero)
                {
                    AVDebug.WriteLine("GetApplicationParameter invalid process handle.");
                    return parameterString;
                }

                //Check application architecture
                IsWow64Process(targetProcessHandle, out bool targetIsWow64);
                IsWow64Process(GetCurrentProcess(), out bool currentIsWow64);

                //Read application parameter
                if (currentIsWow64 && targetIsWow64)
                {
                    //AVDebug.WriteLine("GetApplicationParameter (32) target: " + targetIsWow64 + " current: " + currentIsWow64);
                    parameterString = GetApplicationParameter32(targetProcessHandle, pOption);
                }
                else if (currentIsWow64 && !targetIsWow64)
                {
                    //AVDebug.WriteLine("GetApplicationParameter (WOW64) target: " + targetIsWow64 + " current: " + currentIsWow64);
                    parameterString = GetApplicationParameterWOW64(targetProcessHandle, pOption);
                }
                else if (!currentIsWow64 && targetIsWow64)
                {
                    //AVDebug.WriteLine("GetApplicationParameter (64) target: " + targetIsWow64 + " current: " + currentIsWow64);
                    parameterString = GetApplicationParameter64(targetProcessHandle, pOption);
                }
                else if (!currentIsWow64 && !targetIsWow64)
                {
                    //AVDebug.WriteLine("GetApplicationParameter (32) target: " + targetIsWow64 + " current: " + currentIsWow64);
                    parameterString = GetApplicationParameter32(targetProcessHandle, pOption);
                }
                else
                {
                    AVDebug.WriteLine("GetApplicationParameter unknown architecture.");
                    return parameterString;
                }

                //Remove executable path from commandline
                if (pOption == ProcessParameterOptions.CommandLine)
                {
                    parameterString = Remove_ExePathFromCommandLine(parameterString);
                }

                //Trim and return string
                return parameterString.Trim();
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get GetApplicationParameter: " + ex.Message);
                return parameterString;
            }
        }
    }
}