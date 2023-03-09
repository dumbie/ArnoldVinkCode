using System;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Close open Windows prompts
        public static async Task Close_OpenWindowsPrompts()
        {
            try
            {
                //Windows administrator consent prompt
                if (Check_RunningProcessByName("consent", true))
                {
                    AVDebug.WriteLine("Windows administrator consent prompt is open, killing the process.");
                    bool closedProcess = Close_ProcessesByName("consent", true);
                    await Task.Delay(250);
                    if (closedProcess)
                    {
                        KeyPressReleaseSingle(KeysVirtual.Escape);
                        await Task.Delay(50);
                    }
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

        //Show window by window handle
        public static async Task<bool> Show_ProcessByWindowHandle(IntPtr windowHandle)
        {
            try
            {
                AVDebug.WriteLine("Showing process by window handle: " + windowHandle);

                //Get process id from window handle
                int foundProcessId = Detail_ProcessIdByWindowHandle(windowHandle);

                //Show process window
                return await Show_ProcessByProcessIdAndWindowHandle(foundProcessId, windowHandle);
            }
            catch { }
            return false;
        }

        //Show window by process id
        public static async Task<bool> Show_ProcessByProcessId(int processId)
        {
            try
            {
                AVDebug.WriteLine("Showing process by id: " + processId);

                //Get multi process
                ProcessMulti processMulti = Get_ProcessMultiByProcessId(processId, 0);

                //Check window handle
                if (processMulti == null || processMulti.WindowHandleMain == IntPtr.Zero)
                {
                    AVDebug.WriteLine("Failed showing process by id: " + processId);
                    return false;
                }

                //Show process window
                return await Show_ProcessByProcessIdAndWindowHandle(processId, processMulti.WindowHandleMain);
            }
            catch { }
            return false;
        }

        //Show window by process id and window handle
        public static async Task<bool> Show_ProcessByProcessIdAndWindowHandle(int processId, IntPtr windowHandle)
        {
            try
            {
                AVDebug.WriteLine("Showing process by id: " + processId + " and window handle: " + windowHandle);

                //Close open Windows prompts
                await Close_OpenWindowsPrompts();

                //Get current window placement
                WindowPlacement processWindowState = new WindowPlacement();
                GetWindowPlacement(windowHandle, ref processWindowState);

                //Check current window placement
                WindowShowCommand windowShowCommand = WindowShowCommand.Restore;
                if (processWindowState.windowFlags == WindowFlags.RestoreToMaximized)
                {
                    windowShowCommand = WindowShowCommand.ShowMaximized;
                }
                AVDebug.WriteLine("Changing window placement to: " + windowShowCommand);

                //Retry to show the window
                for (int i = 0; i < 2; i++)
                {
                    try
                    {
                        //Allow changing window
                        if (processId > 0)
                        {
                            AllowSetForegroundWindow(processId);
                            await Task.Delay(10);
                        }

                        //Show window async
                        ShowWindowAsync(windowHandle, windowShowCommand);
                        await Task.Delay(10);

                        //Show window normal
                        ShowWindow(windowHandle, windowShowCommand);
                        await Task.Delay(10);

                        //Bring window to top
                        BringWindowToTop(windowHandle);
                        await Task.Delay(10);

                        //Switch to the window
                        SwitchToThisWindow(windowHandle, true);
                        await Task.Delay(10);
                    }
                    catch { }
                }

                AVDebug.WriteLine("Showed process Id: " + processId + " WindowHandle: " + windowHandle + " ShowCmd: " + windowShowCommand);
                return true;
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed showing process Id: " + processId + "/" + ex.Message);
                return false;
            }
        }
    }
}