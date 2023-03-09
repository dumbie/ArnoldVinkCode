using System;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
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
                    AVDebug.WriteLine("Failed getting other process: " + processId + "/" + Marshal.GetLastWin32Error());
                    return IntPtr.Zero;
                }

                //Open other process token
                if (!OpenProcessToken(hProcess, tokenAccess, out IntPtr hToken))
                {
                    AVDebug.WriteLine("Failed getting other process token: " + processId + "/" + Marshal.GetLastWin32Error());
                    return IntPtr.Zero;
                }

                AVDebug.WriteLine("Got other process token: " + processId + "/" + hToken);
                return hToken;
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed getting other process token: " + processId + "/" + ex.Message);
                return IntPtr.Zero;
            }
            finally
            {
                CloseHandleAuto(hProcess);
            }
        }
    }
}