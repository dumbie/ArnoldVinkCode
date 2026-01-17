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
                ProcessMulti processMulti = Get_ProcessMultiByProcessId(processId);

                //Check multi process
                if (processMulti == null)
                {
                    AVDebug.WriteLine("Failed showing process by id: " + processId);
                    return false;
                }

                //Check window handle main
                if (processMulti.WindowHandleMain() == IntPtr.Zero)
                {
                    AVDebug.WriteLine("Failed showing process by id: " + processId);
                    return false;
                }

                //Show process window
                return await Show_ProcessByWindowHandle(processMulti.WindowHandleMain());
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
                GetWindowPlacement(windowHandle, out WindowPlacement windowPlacement);

                //Check current window placement
                SystemCommand windowSystemCommand = SystemCommand.SC_RESTORE;
                ShowWindowFlags windowShowCommand = ShowWindowFlags.SW_RESTORE;
                if (windowPlacement.windowFlags == WindowPlacementFlags.WPF_RESTORETOMAXIMIZED)
                {
                    windowSystemCommand = SystemCommand.SC_MAXIMIZE;
                    windowShowCommand = ShowWindowFlags.SW_SHOWMAXIMIZED;
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
                        //Post message window
                        PostMessage(windowHandle, WindowMessages.WM_SYSCOMMAND, (int)windowSystemCommand, 0);
                        await Task.Delay(showCommandDelay);

                        //Show window async
                        ShowWindowAsync(windowHandle, windowShowCommand);
                        await Task.Delay(showCommandDelay);

                        //Set foreground window
                        SetForegroundWindow(windowHandle);
                        await Task.Delay(showCommandDelay);

                        //Bring window to top
                        //Locks thread when target window is not responding
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