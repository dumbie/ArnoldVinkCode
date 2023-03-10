using System;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        /// <summary>
        /// Launch application by ShellExecute inherit access
        /// </summary>
        /// <summary>Disables UIAccess from launching process.</summary>
        public static int Launch_ExecuteInherit(string exePath, string workPath, string arguments, bool asAdmin)
        {
            try
            {
                //Check execute path
                if (string.IsNullOrWhiteSpace(exePath))
                {
                    AVDebug.WriteLine("Launching shell process failed: execute path is empty.");
                    return 0;
                }

                AVDebug.WriteLine("Launching shell process with inherited access: " + exePath);

                //Get current process token
                IntPtr launchTokenHandle = Token_Create_Current();

                //Disable token ui access
                Token_Adjust_UIAccess(ref launchTokenHandle, false);

                //Set shell execute info
                ShellExecuteInfo shellExecuteInfo = new ShellExecuteInfo();
                shellExecuteInfo.nShow = WindowShowCommand.Show;
                shellExecuteInfo.fMask = SHELL_EXECUTE_SEE_MASK.SEE_MASK_NOCLOSEPROCESS | SHELL_EXECUTE_SEE_MASK.SEE_MASK_FLAG_NO_UI;
                shellExecuteInfo.lpVerb = asAdmin ? "runas" : "open";
                shellExecuteInfo.lpFile = exePath;

                //Check for url protocol
                if (!Check_PathUrlProtocol(exePath))
                {
                    if (!string.IsNullOrWhiteSpace(arguments))
                    {
                        shellExecuteInfo.lpParameters = arguments;
                    }
                    if (!string.IsNullOrWhiteSpace(workPath))
                    {
                        shellExecuteInfo.lpDirectory = workPath;
                    }
                }

                //Shell execute process
                if (!ShellExecuteExW(shellExecuteInfo))
                {
                    AVDebug.WriteLine("Launching shell process failed: " + Marshal.GetLastWin32Error());
                    return 0;
                }
                else
                {
                    int processId = GetProcessId(shellExecuteInfo.hProcess);
                    AVDebug.WriteLine("Launched shell process successfully: " + processId);
                    return processId;
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Launching shell process failed: " + exePath + "/" + ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// Launch application by ShellExecute user access
        /// </summary>
        public static bool Launch_ExecuteUser(string exePath, string workPath, string arguments, bool asAdmin)
        {
            try
            {
                //Check execute path
                if (string.IsNullOrWhiteSpace(exePath))
                {
                    AVDebug.WriteLine("Launching shell process failed: execute path is empty.");
                    return false;
                }

                AVDebug.WriteLine("Launching shell process with user access: " + exePath);

                //Set shell execute info
                WindowShowCommand nShow = WindowShowCommand.Show;
                string lpVerb = asAdmin ? "runas" : "open";
                string lpFile = exePath;

                //Check for url protocol
                string lpParameters = string.Empty;
                string lpDirectory = string.Empty;
                if (!Check_PathUrlProtocol(exePath))
                {
                    if (!string.IsNullOrWhiteSpace(arguments))
                    {
                        lpParameters = arguments;
                    }
                    if (!string.IsNullOrWhiteSpace(workPath))
                    {
                        lpDirectory = workPath;
                    }
                }

                //Shell execute process
                if (!ShellExecuteUser(lpFile, lpDirectory, lpParameters, lpVerb, nShow))
                {
                    AVDebug.WriteLine("Launching shell process failed.");
                    return false;
                }
                else
                {
                    AVDebug.WriteLine("Launched shell process successfully.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Launching shell process failed: " + exePath + "/" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Launch uwp application
        /// </summary>
        public static int Launch_UwpApplication(string appUserModelId, string arguments)
        {
            try
            {
                //Show launching message
                AVDebug.WriteLine("Launching UWP or Win32Store application: " + appUserModelId + "/" + arguments);

                //Start the process
                UWPActivationManager UWPActivationManager = new UWPActivationManager();
                UWPActivationManager.ActivateApplication(appUserModelId, arguments, UWP_ACTIVATEOPTIONS.AO_NONE, out int processId);

                //Return process id
                AVDebug.WriteLine("Launched UWP or Win32Store process identifier: " + processId);
                return processId;
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed launching UWP or Win32Store: " + appUserModelId + "/" + ex.Message);
                return 0;
            }
        }
    }
}