using System;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Duplicate process token
        private static IntPtr Token_Duplicate(IntPtr hToken)
        {
            try
            {
                IntPtr dToken = IntPtr.Zero;
                SECURITY_ATTRIBUTES securityAttributes = new SECURITY_ATTRIBUTES();
                if (!DuplicateTokenEx(hToken, TOKEN_DESIRED_ACCESS.TOKEN_ALL_ACCESS, ref securityAttributes, TOKEN_IMPERSONATION_LEVEL.SecurityImpersonation, TOKEN_TYPE.TokenImpersonation, out dToken))
                {
                    AVDebug.WriteLine("Failed to duplicate process token: " + Marshal.GetLastWin32Error());
                    return IntPtr.Zero;
                }
                else
                {
                    AVDebug.WriteLine("Succesfully duplicated process token: " + hToken + " > " + dToken);
                    CloseHandleAuto(hToken);
                    return dToken;
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to duplicate process token: " + ex.Message);
                return IntPtr.Zero;
            }
        }

        //Get current process token
        private static IntPtr Token_Create_Current()
        {
            IntPtr hProcess = IntPtr.Zero;
            try
            {
                //Get current process
                hProcess = GetCurrentProcess();

                //Open current process token
                if (!OpenProcessToken(hProcess, TOKEN_DESIRED_ACCESS.TOKEN_ALL_ACCESS, out IntPtr hToken))
                {
                    AVDebug.WriteLine("Failed getting current process token: " + Marshal.GetLastWin32Error());
                    return IntPtr.Zero;
                }

                AVDebug.WriteLine("Got current process token: " + hToken);
                return hToken;
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed getting current process token: " + ex.Message);
                return IntPtr.Zero;
            }
            finally
            {
                CloseHandleAuto(hProcess);
            }
        }

        //Get other process token
        private static IntPtr Token_Create_Process(int processId, PROCESS_DESIRED_ACCESS processAccess, TOKEN_DESIRED_ACCESS tokenAccess)
        {
            IntPtr hProcess = IntPtr.Zero;
            try
            {
                //Open other process
                hProcess = OpenProcess(processAccess, false, processId);
                if (hProcess == IntPtr.Zero)
                {
                    AVDebug.WriteLine("Failed getting other process: " + Marshal.GetLastWin32Error());
                    return IntPtr.Zero;
                }

                //Open other process token
                if (!OpenProcessToken(hProcess, tokenAccess, out IntPtr hToken))
                {
                    AVDebug.WriteLine("Failed getting other process token: " + Marshal.GetLastWin32Error());
                    return IntPtr.Zero;
                }

                AVDebug.WriteLine("Got other process token: " + hToken);
                return hToken;
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed getting other process token: " + ex.Message);
                return IntPtr.Zero;
            }
            finally
            {
                CloseHandleAuto(hProcess);
            }
        }

        //Get unelevated process token
        private static IntPtr Token_Create_Unelevated()
        {
            IntPtr hProcess = IntPtr.Zero;
            try
            {
                //Get unelevated process
                IntPtr shellWindow = GetShellWindow();
                int unelevatedProcessId = Detail_ProcessIdByWindowHandle(shellWindow);

                //Open unelevated process
                hProcess = OpenProcess(PROCESS_DESIRED_ACCESS.PROCESS_QUERY_INFORMATION, false, unelevatedProcessId);
                if (hProcess == IntPtr.Zero)
                {
                    AVDebug.WriteLine("Failed getting unelevated process: " + Marshal.GetLastWin32Error());
                    return IntPtr.Zero;
                }

                //Open unelevated process token
                if (!OpenProcessToken(hProcess, TOKEN_DESIRED_ACCESS.TOKEN_ALL_ACCESS, out IntPtr hToken))
                {
                    AVDebug.WriteLine("Failed getting unelevated process token: " + Marshal.GetLastWin32Error());
                    return IntPtr.Zero;
                }

                AVDebug.WriteLine("Got unelevated process token: " + hToken);
                return hToken;
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed getting unelevated process token: " + ex.Message);
                return IntPtr.Zero;
            }
            finally
            {
                CloseHandleAuto(hProcess);
            }
        }
    }
}