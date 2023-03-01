using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInteropCom;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        public static int Launch_CreateProcess(string exePath, string workPath, string arguments, IntPtr hToken, bool toolAdminAccess)
        {
            try
            {
                AVDebug.WriteLine("Launching process using create process: " + exePath);

                //Set launch information
                string commandLine = exePath;
                if (!string.IsNullOrWhiteSpace(arguments))
                {
                    commandLine += " " + arguments;
                }

                //Create process
                STARTUPINFOW startupInfo = new STARTUPINFOW();
                PROCESS_INFORMATION processInfo = new PROCESS_INFORMATION();
                CreateProcessFlags creationFlags = CreateProcessFlags.CREATE_NEW_CONSOLE | CreateProcessFlags.CREATE_NEW_PROCESS_GROUP;
                if (toolAdminAccess)
                {
                    if (!CreateProcessWithTokenW(hToken, CreateLogonFlags.LOGON_NONE, null, commandLine, creationFlags, IntPtr.Zero, workPath, ref startupInfo, out processInfo))
                    {
                        Debug.Write("CreateProcessWithToken failed: " + Marshal.GetLastWin32Error());
                        return 0;
                    }
                    else
                    {
                        Debug.Write("CreateProcessWithToken launched: " + processInfo.dwProcessId);
                        return (int)processInfo.dwProcessId;
                    }
                }
                else
                {
                    SECURITY_ATTRIBUTES saProcess = new SECURITY_ATTRIBUTES();
                    SECURITY_ATTRIBUTES saThread = new SECURITY_ATTRIBUTES();
                    if (!CreateProcessAsUserW(hToken, null, commandLine, ref saProcess, ref saThread, false, creationFlags, IntPtr.Zero, workPath, ref startupInfo, out processInfo))
                    {
                        Debug.Write("CreateProcessAsUser failed: " + Marshal.GetLastWin32Error());
                        return 0;
                    }
                    else
                    {
                        Debug.Write("CreateProcessAsUser launched: " + processInfo.dwProcessId);
                        return (int)processInfo.dwProcessId;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write("CreateProcess failed: " + exePath + "/" + ex.Message);
                return 0;
            }
        }

        public static int Launch_ShellExecute(string exePath, string workPath, string arguments, bool asAdmin)
        {
            //Does not inherit token from thread started with custom token.
            //Does not reset integrity level from process.
            //Does not remove elevation from process.
            //Does not disable UIAccess from process.
            try
            {
                AVDebug.WriteLine("Launching shell process: " + exePath);

                //Set shell execute info
                ShellExecuteInfo shellExecuteInfo = new ShellExecuteInfo();
                shellExecuteInfo.nShow = WindowShowCommand.Show;
                shellExecuteInfo.fMask = SHELL_EXECUTE_SEE_MASK.SEE_MASK_NOCLOSEPROCESS;
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