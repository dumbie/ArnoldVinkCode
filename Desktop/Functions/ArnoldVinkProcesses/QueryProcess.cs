using System;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Remove executable path from commandline
        public static string CommandLine_RemoveExePath(string targetCommandLine)
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
                PROCESS_BASIC_INFORMATION32 basicInformation = new PROCESS_BASIC_INFORMATION32();
                int readResult = NtQueryInformationProcess32(targetProcessHandle, PROCESS_INFO_CLASS.ProcessBasicInformation, ref basicInformation, (uint)Marshal.SizeOf(basicInformation), out _);
                if (readResult != 0)
                {
                    AVDebug.WriteLine("Failed to get parent process id: " + targetProcessHandle + "/Query failed.");
                    return 0;
                }
                return (int)basicInformation.InheritedFromUniqueProcessId;
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
        public static string Detail_ParameterByProcessHandle(IntPtr targetProcessHandle, PROCESS_PARAMETER_OPTIONS pOption)
        {
            string parameterString = string.Empty;
            try
            {
                IsWow64Process(targetProcessHandle, out bool target32bit);
                IsWow64Process(GetCurrentProcess(), out bool current32bit);
                if (current32bit && target32bit)
                {
                    parameterString = GetApplicationParameter32(targetProcessHandle, pOption);
                }
                else if (current32bit && !target32bit)
                {
                    parameterString = GetApplicationParameterWOW64(targetProcessHandle, pOption);
                }
                else if (!current32bit && target32bit)
                {
                    parameterString = GetApplicationParameter64(targetProcessHandle, pOption);
                }
                else if (!current32bit && !target32bit)
                {
                    parameterString = GetApplicationParameter32(targetProcessHandle, pOption);
                }
                else
                {
                    AVDebug.WriteLine("GetApplicationParameter unknown architecture.");
                }

                //Remove executable path from commandline
                if (pOption == PROCESS_PARAMETER_OPTIONS.CommandLine)
                {
                    parameterString = CommandLine_RemoveExePath(parameterString);
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