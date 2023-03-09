using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInteropCom;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Launch application by ShellExecute
        public static int Launch_Execute(string exePath, string workPath, string arguments, bool asAdmin)
        {
            //Does not inherit token from thread started with custom token.
            //Does not reset integrity level from process.
            //Does not remove elevation from process.
            //Disables UIAccess from launch process.
            try
            {
                //Check execute path
                if (string.IsNullOrWhiteSpace(exePath))
                {
                    Debug.Write("Launching shell process failed: execute path is empty.");
                    return 0;
                }

                AVDebug.WriteLine("Launching shell process: " + exePath);

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
                    Debug.Write("Launching shell process failed: " + Marshal.GetLastWin32Error());
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
                Debug.Write("Launching shell process failed: " + exePath + "/" + ex.Message);
                return 0;
            }
        }

        //Launch uwp application
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