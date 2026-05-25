using System;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Get process token handle
        private static IntPtr Token_Open_Process(IntPtr handleProcess, TOKEN_DESIRED_ACCESS tokenAccess)
        {
            try
            {
                //Open current process token
                if (!OpenProcessToken(handleProcess, tokenAccess, out IntPtr hToken))
                {
                    AVDebug.WriteLine("Failed getting process token: " + Marshal.GetLastWin32Error());
                    return IntPtr.Zero;
                }

                //AVDebug.WriteLine("Got process token: " + hToken);
                return hToken;
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed getting process token: " + ex.Message);
                return IntPtr.Zero;
            }
        }
    }
}