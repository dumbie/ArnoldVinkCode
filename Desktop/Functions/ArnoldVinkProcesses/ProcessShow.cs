using System;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Close open Windows prompts
        public static void Close_OpenWindowsPrompts()
        {
            try
            {
                //Windows administrator consent prompt
                if (Check_RunningProcessByName("consent", true))
                {
                    AVDebug.WriteLine("Windows administrator consent prompt is open, killing the process.");
                    Close_ProcessesByName("consent", true);
                }

                //Windows feature installation prompt
                if (Check_RunningProcessByName("fondue", true))
                {
                    AVDebug.WriteLine("Windows feature installation prompt is open, killing the process.");
                    Close_ProcessesByName("fondue", true);
                }
            }
            catch { }
        }

        //Show window by process id
        public static async Task<bool> Show_ProcessByProcessId(int processId)
        {
            try
            {
                AVDebug.WriteLine("Showing process by id: " + processId);

                //Get multi process
                ProcessMulti processMulti = Get_ProcessMultiByProcessId(processId, 0);

                //Check multi process
                if (processMulti == null)
                {
                    AVDebug.WriteLine("Failed showing process by id: " + processId);
                    return false;
                }

                //Show process window
                return await Show_ProcessByWindowHandle(processMulti.WindowHandleMain);
            }
            catch { }
            return false;
        }

        //Show window by window handle
        public static async Task<bool> Show_ProcessByWindowHandle(IntPtr windowHandle)
        {
            try
            {
                //Check the window handle
                if (windowHandle == IntPtr.Zero)
                {
                    AVDebug.WriteLine("Failed showing process, window handle is empty.");
                    return false;
                }

                AVDebug.WriteLine("Showing process by window handle: " + windowHandle);

                //Close open Windows prompts
                Close_OpenWindowsPrompts();

                //Get current window placement
                WindowPlacement processWindowState = new WindowPlacement();
                GetWindowPlacement(windowHandle, ref processWindowState);

                //Check current window placement
                WindowShowCommand windowShowCommand = WindowShowCommand.Restore;
                if (processWindowState.windowFlags == WindowFlags.RestoreToMaximized)
                {
                    windowShowCommand = WindowShowCommand.ShowMaximized;
                }

                //Allow set foreground window
                AllowSetForegroundWindow(ASFW_ANY);
                await Task.Delay(50);

                //Retry to show the window
                int showCommandDelay = 25;
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        //Show window async
                        ShowWindowAsync(windowHandle, windowShowCommand);
                        await Task.Delay(showCommandDelay);

                        //Show window normal
                        ShowWindow(windowHandle, windowShowCommand);
                        await Task.Delay(showCommandDelay);

                        //Set foreground window
                        SetForegroundWindow(windowHandle);
                        await Task.Delay(showCommandDelay);

                        //Bring window to top
                        //Locks thread when target process is unresponsive.
                        //BringWindowToTop(windowHandle);
                        //await Task.Delay(showCommandDelay);

                        //Switch to the window
                        SwitchToThisWindow(windowHandle, true);
                        await Task.Delay(showCommandDelay);
                    }
                    catch { }
                }

                AVDebug.WriteLine("Showed process window handle: " + windowHandle + "/Show command: " + windowShowCommand);
                return true;
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed showing process window handle: " + windowHandle + "/" + ex.Message);
                return false;
            }
        }
    }
}