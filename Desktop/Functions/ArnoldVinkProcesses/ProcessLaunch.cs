using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using static ArnoldVinkCode.AVInteropCom;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        public static bool Launch_CreateProcess(string exePath, string workingPath, string launchArgs)
        {
            try
            {
                IntPtr hToken = IntPtr.Zero;
                Debug.WriteLine("Launching create process: " + exePath);

                //Set launch information
                string currentDirectory = string.Empty;
                if (!string.IsNullOrWhiteSpace(workingPath) && Directory.Exists(workingPath))
                {
                    currentDirectory = workingPath;
                }
                else
                {
                    currentDirectory = Path.GetDirectoryName(exePath);
                }
                string commandLine = exePath + " " + launchArgs;

                //Check administrator access
                WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent(TokenAccessLevels.AllAccess);
                bool processAdminAccess = new WindowsPrincipal(windowsIdentity).IsInRole(WindowsBuiltInRole.Administrator);

                //Create process
                STARTUPINFOW startupInfo = new STARTUPINFOW();
                PROCESS_INFORMATION processInfo = new PROCESS_INFORMATION();
                CreateProcessFlags creationFlags = CreateProcessFlags.CREATE_NEW_CONSOLE | CreateProcessFlags.CREATE_NEW_PROCESS_GROUP;
                if (processAdminAccess)
                {
                    if (!CreateProcessWithTokenW(hToken, CreateLogonFlags.LOGON_NONE, null, commandLine, creationFlags, IntPtr.Zero, currentDirectory, ref startupInfo, out processInfo))
                    {
                        Debug.Write("CreateProcessWithToken failed: " + Marshal.GetLastWin32Error());
                        return false;
                    }
                    else
                    {
                        Debug.Write("CreateProcessWithToken launched: " + processInfo.dwProcessId);
                        return true;
                    }
                }
                else
                {
                    SECURITY_ATTRIBUTES saProcess = new SECURITY_ATTRIBUTES();
                    SECURITY_ATTRIBUTES saThread = new SECURITY_ATTRIBUTES();
                    if (!CreateProcessAsUserW(hToken, null, commandLine, ref saProcess, ref saThread, false, creationFlags, IntPtr.Zero, currentDirectory, ref startupInfo, out processInfo))
                    {
                        Debug.Write("CreateProcessAsUser failed: " + Marshal.GetLastWin32Error());
                        return false;
                    }
                    else
                    {
                        Debug.Write("CreateProcessAsUser launched: " + processInfo.dwProcessId);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write("CreateProcess failed: " + exePath + " / " + ex.Message);
                return false;
            }
        }

        public static bool Launch_ShellExecute(string exePath, string workingPath, string launchArgs)
        {
            //Does not reset integrity level from process.
            //Does not remove elevation from process.
            //Does not disable UIAccess from process.
            try
            {
                Debug.WriteLine("Launching shell process: " + exePath);

                //Set shell execute info
                ShellExecuteInfo shellExecuteInfo = new ShellExecuteInfo();
                shellExecuteInfo.fMask = SHELL_EXECUTE_SEE_MASK.SEE_MASK_NOCLOSEPROCESS;
                shellExecuteInfo.nShow = WindowShowCommand.Normal;
                shellExecuteInfo.lpVerb = "runas";
                shellExecuteInfo.lpFile = exePath;
                shellExecuteInfo.lpParameters = launchArgs;
                if (!string.IsNullOrWhiteSpace(workingPath) && Directory.Exists(workingPath))
                {
                    shellExecuteInfo.lpDirectory = workingPath;
                }
                else
                {
                    shellExecuteInfo.lpDirectory = Path.GetDirectoryName(exePath);
                }

                //Shell execute process
                if (!ShellExecuteExW(shellExecuteInfo))
                {
                    Debug.Write("ShellExecuteEx failed: " + Marshal.GetLastWin32Error());
                    return false;
                }
                else
                {
                    int processId = GetProcessId(shellExecuteInfo.hProcess);
                    bool processLaunched = processId == 0 ? false : true;
                    Debug.WriteLine("ShellExecuteEx launched: " + processId + "/" + processLaunched);
                    return processLaunched;
                }
            }
            catch (Exception ex)
            {
                Debug.Write("ShellExecuteEx failed: " + exePath + " / " + ex.Message);
                return false;
            }
        }

        //Launch uwp application
        public static Process Launch_UwpApplication(string appUserModelId, string runArgument)
        {
            try
            {
                //Show launching message
                Debug.WriteLine("Launching UWP or Win32Store application: " + appUserModelId + " / " + runArgument);

                //Start the process
                UWPActivationManager UWPActivationManager = new UWPActivationManager();
                UWPActivationManager.ActivateApplication(appUserModelId, runArgument, UWPActivationManagerOptions.None, out int processId);

                //Return process
                Process returnProcess = Process.GetProcessById(processId);
                Debug.WriteLine("Launched UWP or Win32Store process identifier: " + processId);
                return returnProcess;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed launching UWP or Win32Store: " + appUserModelId + " / " + ex.Message);
                return null;
            }
        }
    }
}