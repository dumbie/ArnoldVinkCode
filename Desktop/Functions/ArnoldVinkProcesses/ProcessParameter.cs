using System;
using System.Diagnostics;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Enumerators
        public enum __PROCESS_PARAMETER_OPTIONS
        {
            CurrentDirectoryPath,
            ImagePathName,
            CommandLine,
            DesktopInfo,
            Environment
        };

        public enum __PROCESS_INFO_CLASS : int
        {
            ProcessBasicInformation = 0,
            ProcessWow64Information = 26
        }

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

        public static string Process_GetApplicationParameter(IntPtr processHandle, __PROCESS_PARAMETER_OPTIONS pOption)
        {
            string parameterString = string.Empty;
            try
            {
                IsWow64Process(processHandle, out bool target32bit);
                IsWow64Process(GetCurrentProcess(), out bool current32bit);
                if (current32bit && target32bit)
                {
                    parameterString = GetApplicationParameter32(processHandle, pOption);
                }
                else if (current32bit && !target32bit)
                {
                    parameterString = GetApplicationParameter64(processHandle, pOption);
                }
                else if (!current32bit && target32bit)
                {
                    parameterString = GetApplicationParameterWOW64(processHandle, pOption);
                }
                else if (!current32bit && !target32bit)
                {
                    parameterString = GetApplicationParameter32(processHandle, pOption);
                }
                else
                {
                    Debug.WriteLine("GetApplicationParameter unknown architecture.");
                }

                //Remove executable path from commandline
                if (pOption == __PROCESS_PARAMETER_OPTIONS.CommandLine)
                {
                    parameterString = CommandLine_RemoveExePath(parameterString);
                }

                return parameterString;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get GetApplicationParameter: " + ex.Message);
                return parameterString;
            }
        }
    }
}