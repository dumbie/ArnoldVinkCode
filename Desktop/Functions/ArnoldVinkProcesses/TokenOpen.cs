using System;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Get current process token
        private static IntPtr Token_Open_Current()
        {
            try
            {
                //Open current process token
                if (!OpenProcessToken(GetCurrentProcess(), TOKEN_DESIRED_ACCESS.TOKEN_ALL_ACCESS, out IntPtr hToken))
                {
                    AVDebug.WriteLine("Failed getting current process token: " + Marshal.GetLastWin32Error());
                    return IntPtr.Zero;
                }

                //AVDebug.WriteLine("Got current process token: " + hToken);
                return hToken;
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed getting current process token: " + ex.Message);
                return IntPtr.Zero;
            }
        }

        //Get other process token
        private static IntPtr Token_Open_Process(int processId, PROCESS_DESIRED_ACCESS processAccess, TOKEN_DESIRED_ACCESS tokenAccess)
        {
            try
            {
                //Open other process
                using AVFin hProcess = new AVFin(AVFinMethod.CloseHandle, OpenProcess(processAccess, false, processId));
                if (hProcess.Get() == IntPtr.Zero)
                {
                    AVDebug.WriteLine("Failed getting other process: " + processId + "/" + Marshal.GetLastWin32Error());
                    return IntPtr.Zero;
                }

                //Open other process token
                if (!OpenProcessToken(hProcess.Get(), tokenAccess, out IntPtr hToken))
                {
                    AVDebug.WriteLine("Failed getting other process token: " + processId + "/" + Marshal.GetLastWin32Error());
                    return IntPtr.Zero;
                }

                //AVDebug.WriteLine("Got other process token: " + processId + "/" + hToken);
                return hToken;
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed getting other process token: " + processId + "/" + ex.Message);
                return IntPtr.Zero;
            }
        }

        //Get unelevated process token
        private static IntPtr Token_Open_Unelevated()
        {
            try
            {
                //Get unelevated process
                IntPtr shellWindow = GetShellWindow();
                int unelevatedProcessId = Detail_ProcessIdByWindowHandle(shellWindow);
                if (unelevatedProcessId <= 0)
                {
                    AVDebug.WriteLine("Invalid unelevated process: " + unelevatedProcessId);
                    return IntPtr.Zero;
                }

                //Open unelevated process
                using AVFin hProcess = new AVFin(AVFinMethod.CloseHandle, OpenProcess(PROCESS_DESIRED_ACCESS.PROCESS_QUERY_INFORMATION, false, unelevatedProcessId));
                if (hProcess.Get() == IntPtr.Zero)
                {
                    AVDebug.WriteLine("Failed getting unelevated process: " + Marshal.GetLastWin32Error());
                    return IntPtr.Zero;
                }

                //Open unelevated process token
                if (!OpenProcessToken(hProcess.Get(), TOKEN_DESIRED_ACCESS.TOKEN_ALL_ACCESS, out IntPtr hToken))
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
        }
    }
}