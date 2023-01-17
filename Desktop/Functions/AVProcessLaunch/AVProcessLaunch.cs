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
        public static void ProcessLaunch_NormalUser(string exePath, string launchArgs)
        {
            IntPtr hProcess = IntPtr.Zero;
            IntPtr hToken = IntPtr.Zero;
            IntPtr dToken = IntPtr.Zero;
            PROCESS_INFORMATION processInfo = new PROCESS_INFORMATION();
            try
            {
                //Only works as administrator
                ////Check administrator permission
                //bool adminPermission = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
                //if (!adminPermission)
                //{
                //    ProcessLaunch_CurrentUser(exePath, launchArgs, true);
                //    return;
                //}

                //Get unelevated process
                IntPtr shellWindow = GetShellWindow();
                GetWindowThreadProcessId(shellWindow, out int shellProcessId);
                int tokenProcessId = Process.GetProcessById(shellProcessId).Id;

                //Open unelevated process
                hProcess = OpenProcess(ProcessAccessFlags.QueryInformation, false, tokenProcessId);
                if (hProcess == IntPtr.Zero)
                {
                    Debug.WriteLine("Failed to OpenProcess: " + Marshal.GetLastWin32Error());
                    return;
                }

                //Get unelevated process token
                if (!OpenProcessToken(hProcess, TOKEN_DESIRED_ACCESS.TOKEN_ALL_ACCESS, out hToken))
                {
                    Debug.WriteLine("Failed to OpenProcessToken: " + Marshal.GetLastWin32Error());
                    return;
                }

                //Duplicate unelevated process token
                SECURITY_ATTRIBUTES securityAttributes = new SECURITY_ATTRIBUTES();
                if (!DuplicateTokenEx(hToken, TOKEN_DESIRED_ACCESS.TOKEN_ALL_ACCESS, ref securityAttributes, TOKEN_IMPERSONATION_LEVEL.SecurityImpersonation, TOKEN_TYPE.TokenPrimary, out dToken))
                {
                    Debug.WriteLine("Failed to DuplicateTokenEx: " + Marshal.GetLastWin32Error());
                    return;
                }

                //Set startup information
                STARTUPINFO startupInfo = new STARTUPINFO();
                startupInfo.lpDesktop = "WinSta0\\Default";

                //Set launch information
                uint logonFlags = 0;
                uint creationFlags = 0;
                string commandLine = exePath;
                string currentDirectory = Path.GetDirectoryName(commandLine);
                commandLine += " " + launchArgs;

                //Create process
                if (!CreateProcessWithTokenW(dToken, logonFlags, null, commandLine, creationFlags, IntPtr.Zero, currentDirectory, ref startupInfo, out processInfo))
                {
                    Debug.WriteLine("CreateProcessWithToken failed: " + Marshal.GetLastWin32Error());
                }
                else
                {
                    Debug.WriteLine("CreateProcessWithToken PID: " + processInfo.dwProcessId);
                }
            }
            catch { }
            finally
            {
                CloseHandleAuto(hProcess);
                CloseHandleAuto(hToken);
                CloseHandleAuto(dToken);
                CloseHandleAuto(processInfo.hProcess);
                CloseHandleAuto(processInfo.hThread);
            }
        }

        public static void ProcessLaunch_CurrentUser(string exePath, string launchArgs, bool removeUIAccess, bool resetIntegrity)
        {
            PROCESS_INFORMATION processInfo = new PROCESS_INFORMATION();
            try
            {
                //Get current token
                IntPtr hToken = WindowsIdentity.GetCurrent().Token;
                if (hToken == IntPtr.Zero)
                {
                    Debug.WriteLine("Failed to GetCurrentToken.");
                    return;
                }

                //Adjust token access
                if (false)
                {
                    //Token_Disable_Elevation(ref hToken);
                }

                if (removeUIAccess)
                {
                    Token_Adjust_UIAccess(false, ref hToken);
                }

                if (resetIntegrity)
                {
                    Token_Adjust_Integrity(WELL_KNOWN_SID_TYPE.WinMediumLabelSid, ref hToken);
                }

                //Set startup information
                STARTUPINFO startupInfo = new STARTUPINFO();
                startupInfo.lpDesktop = "WinSta0\\Default";

                //Set launch information
                SECURITY_ATTRIBUTES saProcess = new SECURITY_ATTRIBUTES();
                SECURITY_ATTRIBUTES saThread = new SECURITY_ATTRIBUTES();
                uint creationFlags = 0;
                string commandLine = exePath;
                string currentDirectory = Path.GetDirectoryName(commandLine);
                commandLine += " " + launchArgs;

                //Create process
                if (!CreateProcessAsUserW(hToken, null, commandLine, ref saProcess, ref saThread, false, creationFlags, IntPtr.Zero, currentDirectory, ref startupInfo, out processInfo))
                {
                    Debug.Write("CreateProcessAsUser failed: " + Marshal.GetLastWin32Error());
                }
                else
                {
                    Debug.Write("CreateProcessAsUser PID: " + processInfo.dwProcessId);
                }
            }
            catch { }
            finally
            {
                CloseHandleAuto(processInfo.hProcess);
                CloseHandleAuto(processInfo.hThread);
            }
        }

        public static void ProcessLaunch_AdminUser(string exePath, string launchArgs)
        {
            ShellExecuteInfo shellExecuteInfo = new ShellExecuteInfo();
            try
            {
                shellExecuteInfo.nShow = (int)WindowShowCommand.ShowDefault;
                shellExecuteInfo.lpFile = Marshal.StringToHGlobalAuto(exePath);
                shellExecuteInfo.lpVerb = Marshal.StringToHGlobalAuto("runas");
                shellExecuteInfo.lpParameters = Marshal.StringToHGlobalAuto(launchArgs);
                shellExecuteInfo.lpDirectory = Marshal.StringToHGlobalAuto(Path.GetDirectoryName(exePath));

                if (!ShellExecuteExW(shellExecuteInfo))
                {
                    Debug.Write("ShellExecuteExW failed: " + Marshal.GetLastWin32Error());
                }
                else
                {
                    Debug.WriteLine("ShellExecuteExW launched process.");
                }
            }
            catch { }
            finally
            {
                CloseHandleAuto(shellExecuteInfo.lpFile);
                CloseHandleAuto(shellExecuteInfo.lpVerb);
                CloseHandleAuto(shellExecuteInfo.lpParameters);
                CloseHandleAuto(shellExecuteInfo.lpDirectory);
            }
        }
    }
}