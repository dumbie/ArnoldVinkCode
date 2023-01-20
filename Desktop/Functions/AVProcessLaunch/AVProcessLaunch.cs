using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcessLaunch
    {
        public static bool ProcessLaunch_User(string exePath, string workingPath, string launchArgs, bool disableAdminAccess, bool disableUiAccess)
        {
            try
            {
                Debug.WriteLine("Launching process: " + exePath);

                //Get current process token (Elevation: Yes/Full / Depends on process)
                if (!Token_CreateFromCurrentProcess(out IntPtr hToken, out bool tokenAdminAccess))
                {
                    return false;
                }

                ////Get unelevated process token (Elevation: No/Limited)
                //if (!Token_CreateFromUnelevatedProcess(out IntPtr hToken, out bool tokenAdminAccess))
                //{
                //    return false;
                //}

                ////Get safer process token (Elevation: Yes/Full / Depends on process)
                //if (!Token_CreateFromSaferApi(out IntPtr hToken, out bool tokenAdminAccess))
                //{
                //    return false;
                //}

                //Adjust token privilege
                Token_Set_Privilege(ref hToken, PrivilegeConstants.SeTcbPrivilege, true);

                //Adjust token access
                if (disableAdminAccess && tokenAdminAccess)
                {
                    Token_Disable_Elevation(ref hToken);
                    Token_Adjust_Integrity(WELL_KNOWN_SID_TYPE.WinMediumLabelSid, ref hToken);
                }

                if (disableUiAccess)
                {
                    Token_Adjust_UIAccess(false, ref hToken);
                }

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
                Debug.Write("CreateProcess failed: " + ex.Message);
                return false;
            }
        }

        public static bool ProcessLaunch_Admin(string exePath, string workingPath, string launchArgs)
        {
            //Does not reset integrity level from process.
            //Does not remove elevation from process.
            //Does not disable UIAccess from process.
            try
            {
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
                Debug.Write("ShellExecuteEx failed: " + ex.Message);
                return false;
            }
        }
    }
}