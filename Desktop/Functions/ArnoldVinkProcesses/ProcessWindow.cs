using System;
using System.Diagnostics;
using System.Linq;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Get uwp application window handle by AppUserModelId
        public static IntPtr Window_UwpWindowHandleByAppUserModelId(string targetAppUserModelId)
        {
            try
            {
                Process frameHostProcess = Get_ProcessesByName("ApplicationFrameHost", true).FirstOrDefault();
                if (frameHostProcess != null)
                {
                    foreach (ProcessThread threadProcess in frameHostProcess.Threads)
                    {
                        try
                        {
                            foreach (IntPtr threadWindowHandle in Thread_GetWindowHandles(threadProcess.Id))
                            {
                                try
                                {
                                    if (Check_WindowHandleIsUwpApp(threadWindowHandle))
                                    {
                                        string targetAppUserModelIdLower = targetAppUserModelId.ToLower();
                                        string foundAppUserModelIdLower = Detail_ApplicationUserModelIdByWindowHandle(threadWindowHandle).ToLower();
                                        if (targetAppUserModelIdLower == foundAppUserModelIdLower)
                                        {
                                            return threadWindowHandle;
                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }
            return IntPtr.Zero;
        }

        //Get window Z order by window handle
        public static int Window_ZOrderByWindowHandle(IntPtr windowHandle)
        {
            int zOrder = -1;
            try
            {
                IntPtr zHandle = windowHandle;
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