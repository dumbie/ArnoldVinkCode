using System;
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

                //Trim command line
                targetCommandLine = targetCommandLine.Trim();
            }
            catch { }
            return targetCommandLine;
        }

        public static string Detail_ApplicationParameterByProcessHandle(IntPtr targetProcessHandle, PROCESS_PARAMETER_OPTIONS pOption)
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
                    parameterString = GetApplicationParameter64(targetProcessHandle, pOption);
                }
                else if (!current32bit && target32bit)
                {
                    parameterString = GetApplicationParameterWOW64(targetProcessHandle, pOption);
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

                return parameterString;
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get GetApplicationParameter: " + ex.Message);
                return parameterString;
            }
        }
    }
}