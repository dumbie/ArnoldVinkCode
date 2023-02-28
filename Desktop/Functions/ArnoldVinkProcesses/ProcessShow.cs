using System;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Close open start menu, cortana or search
        public static async Task Close_OpenWindowsStartMenu(ProcessMulti foregroundProcess)
        {
            try
            {
                if (foregroundProcess.Name == "SearchUI")
                {
                    AVDebug.WriteLine("Start menu is currently open, pressing escape to close it.");
                    KeyPressReleaseSingle(KeysVirtual.Escape);
                    await Task.Delay(10);
                }
            }
            catch { }
        }

        //Close open Windows system menu
        public static async Task Close_OpenWindowsSystemMenu(ProcessMulti foregroundProcess)
        {
            try
            {
                AVDebug.WriteLine("Closing system menu for window: " + foregroundProcess.WindowHandle);
                SendMessage(foregroundProcess.WindowHandle, (int)WindowMessages.WM_CANCELMODE, 0, 0);
                await Task.Delay(10);
            }
            catch { }
        }

        //Close open Windows prompts
        public static async Task Close_OpenWindowsPrompts()
        {
            try
            {
                //Windows administrator consent prompt
                if (Get_ProcessesByName("consent", true).FirstOrDefault() != null)
                {
                    AVDebug.WriteLine("Windows administrator consent prompt is open, killing the process.");
                    bool closedProcess = Close_ProcessesByName("consent", true);
                    await Task.Delay(500);
                    if (closedProcess)
                    {
                        KeyPressReleaseSingle(KeysVirtual.Escape);
                        await Task.Delay(10);
                    }
                }

                //Windows feature installation prompt
                if (Get_ProcessesByName("fondue", true).FirstOrDefault() != null)
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
                return await Show_ProcessIdAndWindowHandle(foundProcessId, windowHandle);
            }
            catch { }
            return false;
        }

        //Show window by process id
        public static async Task<bool> Show_ProcessById(int processId)
        {
            try
            {
                AVDebug.WriteLine("Showing process by id: " + processId);

                //Get multi process
                ProcessMulti processMulti = Get_ProcessMultiByProcessId(processId);

                //Check window handle
                if (processMulti.WindowHandle == IntPtr.Zero)
                {
                    AVDebug.WriteLine("Failed showing process by id: " + processId);
                    return false;
                }

                //Show process window
                return await Show_ProcessIdAndWindowHandle(processId, processMulti.WindowHandle);
            }
            catch { }
            return false;
        }

        //Show window by process id and window handle
        public static async Task<bool> Show_ProcessIdAndWindowHandle(int processId, IntPtr windowHandle)
        {
            try
            {
                AVDebug.WriteLine("Showing process by id: " + processId + "and window handle: " + windowHandle);

                //Close open Windows prompts
                await Close_OpenWindowsPrompts();

                //Get current focused application
                ProcessMulti foregroundProcess = Get_ProcessMultiByWindowHandle(GetForegroundWindow());

                //Close open start menu, cortana or search
                await Close_OpenWindowsStartMenu(foregroundProcess);

                //Close open Windows system menu
                await Close_OpenWindowsSystemMenu(foregroundProcess);

                //Detect the previous window state
                WindowShowCommand windowShowCommand = WindowShowCommand.None;
                WindowPlacement processWindowState = new WindowPlacement();
                GetWindowPlacement(windowHandle, ref processWindowState);
                AVDebug.WriteLine("Detected the previous window state: " + processWindowState.windowFlags);
                if (processWindowState.windowFlags == WindowFlags.RestoreToMaximized)
                {
                    windowShowCommand = WindowShowCommand.ShowMaximized;
                }
                else
                {
                    windowShowCommand = WindowShowCommand.Restore;
                }

                //Retry to show the window
                for (int i = 0; i < 2; i++)
                {
                    try
                    {
                        //Allow changing window
                        AllowSetForegroundWindow(processId);
                        await Task.Delay(10);

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

                        //Focus on the window
                        UiaFocusWindowHandle(windowHandle);
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