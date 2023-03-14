using System;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Hide window by process id
        public static async Task<bool> Hide_ProcessByProcessId(int processId)
        {
            try
            {
                AVDebug.WriteLine("Hiding process by id: " + processId);

                //Get multi process
                ProcessMulti processMulti = Get_ProcessMultiByProcessId(processId, 0);

                //Check multi process
                if (processMulti == null)
                {
                    AVDebug.WriteLine("Failed hiding process by id: " + processId);
                    return false;
                }

                //Check window handle main
                if (processMulti.WindowHandleMain == IntPtr.Zero)
                {
                    AVDebug.WriteLine("Failed hiding process by id: " + processId);
                    return false;
                }

                //Hide process window
                return await Hide_ProcessByWindowHandle(processMulti.WindowHandleMain);
            }
            catch { }
            return false;
        }

        //Hide window by window handle
        public static async Task<bool> Hide_ProcessByWindowHandle(IntPtr windowHandle)
        {
            try
            {
                //Check the window handle
                if (windowHandle == IntPtr.Zero)
                {
                    AVDebug.WriteLine("Failed hiding process, window handle is empty.");
                    return false;
                }

                AVDebug.WriteLine("Hiding process by window handle: " + windowHandle);
                int showCommandDelay = 25;

                //Post message window
                PostMessageAsync(windowHandle, WindowMessages.WM_SYSCOMMAND, (int)SystemCommand.SC_MINIMIZE, 0);
                await Task.Delay(showCommandDelay);

                //Hide window async
                ShowWindowAsync(windowHandle, WindowShowCommand.Minimize);
                await Task.Delay(showCommandDelay);

                AVDebug.WriteLine("Hidden process window handle: " + windowHandle);
                return true;
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed hiding process window handle: " + windowHandle + "/" + ex.Message);
                return false;
            }
        }
    }
}