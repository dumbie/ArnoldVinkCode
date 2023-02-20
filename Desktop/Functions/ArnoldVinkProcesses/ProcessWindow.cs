using System;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        public static int Window_GetZOrder(IntPtr hWnd)
        {
            int zOrder = -1;
            try
            {
                IntPtr zHandle = hWnd;
                while (zHandle != IntPtr.Zero)
                {
                    zHandle = GetWindow(zHandle, GetWindowFlags.GW_HWNDPREV);
                    zOrder++;
                }
                //Debug.WriteLine("Window " + hWnd + " ZOrder: " + zOrder);
            }
            catch { }
            return zOrder;
        }
    }
}