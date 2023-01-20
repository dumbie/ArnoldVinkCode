using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcessLaunch
    {
        private static bool Token_CreateFromCurrentProcess(out IntPtr dToken, out bool tokenAdminAccess)
        {
            IntPtr hToken = IntPtr.Zero;
            dToken = IntPtr.Zero;
            tokenAdminAccess = false;
            try
            {
                //Get current process token
                WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent(TokenAccessLevels.AllAccess);
                hToken = windowsIdentity.Token;

                //Duplicate current process token
                SECURITY_ATTRIBUTES securityAttributes = new SECURITY_ATTRIBUTES();
                if (!DuplicateTokenEx(hToken, TOKEN_DESIRED_ACCESS.TOKEN_ALL_ACCESS, ref securityAttributes, TOKEN_IMPERSONATION_LEVEL.SecurityImpersonation, TOKEN_TYPE.TokenPrimary, out dToken))
                {
                    Debug.WriteLine("Failed to duplicate current process token: " + Marshal.GetLastWin32Error());
                    return false;
                }

                //Check administrator access
                tokenAdminAccess = new WindowsPrincipal(windowsIdentity).IsInRole(WindowsBuiltInRole.Administrator);
                //adminAccess = windowsIdentity.Owner.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid);
                Debug.WriteLine("Got current process token: " + dToken + "/admin: " + tokenAdminAccess);
                return dToken != IntPtr.Zero;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed getting current process token: " + ex.Message);
                return false;
            }
            finally
            {
                CloseHandleAuto(hToken);
            }
        }

        private static bool Token_CreateFromUnelevatedProcess(out IntPtr dToken, out bool tokenAdminAccess)
        {
            IntPtr hToken = IntPtr.Zero;
            dToken = IntPtr.Zero;
            tokenAdminAccess = false;
            IntPtr hProcess = IntPtr.Zero;
            PROCESS_INFORMATION processInfo = new PROCESS_INFORMATION();
            try
            {
                //Get unelevated process
                IntPtr shellWindow = GetShellWindow();
                GetWindowThreadProcessId(shellWindow, out int unelevatedProcessId);

                //Open unelevated process
                hProcess = OpenProcess(ProcessAccessFlags.QueryInformation, false, unelevatedProcessId);
                if (hProcess == IntPtr.Zero)
                {
                    Debug.WriteLine("Failed to get unelevated process: " + Marshal.GetLastWin32Error());
                    return false;
                }

                //Get unelevated process token
                if (!OpenProcessToken(hProcess, TOKEN_DESIRED_ACCESS.TOKEN_ALL_ACCESS, out hToken))
                {
                    Debug.WriteLine("Failed to get unelevated process token: " + Marshal.GetLastWin32Error());
                    return false;
                }

                //Duplicate unelevated process token
                SECURITY_ATTRIBUTES securityAttributes = new SECURITY_ATTRIBUTES();
                if (!DuplicateTokenEx(hToken, TOKEN_DESIRED_ACCESS.TOKEN_ALL_ACCESS, ref securityAttributes, TOKEN_IMPERSONATION_LEVEL.SecurityImpersonation, TOKEN_TYPE.TokenPrimary, out dToken))
                {
                    Debug.WriteLine("Failed to duplicate unelevated process token: " + Marshal.GetLastWin32Error());
                    return false;
                }

                //Check administrator access
                WindowsIdentity windowsIdentity = new WindowsIdentity(dToken);
                tokenAdminAccess = new WindowsPrincipal(windowsIdentity).IsInRole(WindowsBuiltInRole.Administrator);
                //adminAccess = windowsIdentity.Owner.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid);
                Debug.WriteLine("Got unelevated process token: " + dToken + "/admin: " + tokenAdminAccess);
                return dToken != IntPtr.Zero;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed getting unelevated process token: " + ex.Message);
                return false;
            }
            finally
            {
                CloseHandleAuto(hToken);
                CloseHandleAuto(hProcess);
                CloseHandleAuto(processInfo.hProcess);
                CloseHandleAuto(processInfo.hThread);
            }
        }

        public static bool Token_CreateFromSaferApi(out IntPtr hToken, out bool adminAccess)
        {
            hToken = IntPtr.Zero;
            adminAccess = false;
            IntPtr pLevelHandle = IntPtr.Zero;
            try
            {
                if (!SaferCreateLevel(SAFER_SCOPES.SAFER_SCOPEID_USER, SAFER_LEVELS.SAFER_LEVELID_FULLYTRUSTED, SAFER_OPEN_FLAGS.SAFER_LEVEL_OPEN, out pLevelHandle, IntPtr.Zero))
                {
                    Debug.Write("SaferCreateLevel failed: " + Marshal.GetLastWin32Error());
                    return false;
                }

                if (!SaferComputeTokenFromLevel(pLevelHandle, IntPtr.Zero, out hToken, SAFER_COMPUTE_TOKEN_FLAGS.SAFER_TOKEN_NONE, IntPtr.Zero))
                {
                    Debug.Write("SaferComputeTokenFromLevel failed: " + Marshal.GetLastWin32Error());
                    return false;
                }
                else
                {
                    //Check administrator access
                    WindowsIdentity windowsIdentity = new WindowsIdentity(hToken);
                    adminAccess = new WindowsPrincipal(windowsIdentity).IsInRole(WindowsBuiltInRole.Administrator);
                    //adminAccess = windowsIdentity.Owner.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid);
                    Debug.Write("SaferComputeTokenFromLevel created: " + hToken + "/admin: " + adminAccess);
                    return hToken != IntPtr.Zero;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed creating safer token: " + ex.Message);
                return false;
            }
            finally
            {
                SaferCloseLevel(pLevelHandle);
            }
        }
    }
}