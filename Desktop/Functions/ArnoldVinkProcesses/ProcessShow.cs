using System;
using System.Diagnostics;
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
                    Debug.WriteLine("Start menu is currently open, pressing escape to close it.");
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
                Debug.WriteLine("Closing system menu for window: " + foregroundProcess.WindowHandle);
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
                    Debug.WriteLine("Windows administrator consent prompt is open, killing the process.");
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
                    Debug.WriteLine("Windows feature installation prompt is open, killing the process.");
                    Close_ProcessesByName("fondue", true);
                }
            }
            catch { }
        }

        //Show window by process id and window handle
        public static async Task<bool> Show_ProcessIdAndWindowHandle(string processTitle, int processId, IntPtr processWindowHandle, WindowShowCommand windowShowCommand)
        {
            try
            {
                //Close open Windows prompts
                await Close_OpenWindowsPrompts();

                //Get current focused application
                ProcessMulti foregroundProcess = Get_ProcessMultiByWindowHandle(GetForegroundWindow());

                //Close open start menu, cortana or search
                await Close_OpenWindowsStartMenu(foregroundProcess);

                //Close open Windows system menu
                await Close_OpenWindowsSystemMenu(foregroundProcess);

                //Detect the previous window state
                WindowPlacement processWindowState = new WindowPlacement();
                GetWindowPlacement(processWindowHandle, ref processWindowState);
                Debug.WriteLine("Detected the previous window state: " + processWindowState.windowFlags);
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
                        ShowWindowAsync(processWindowHandle, windowShowCommand);
                        await Task.Delay(10);

                        //Show window normal
                        ShowWindow(processWindowHandle, windowShowCommand);
                        await Task.Delay(10);

                        //Bring window to top
                        BringWindowToTop(processWindowHandle);
                        await Task.Delay(10);

                        //Switch to the window
                        SwitchToThisWindow(processWindowHandle, true);
                        await Task.Delay(10);

                        //Focus on the window
                        UiaFocusWindowHandle(processWindowHandle);
                        await Task.Delay(10);
                    }
                    catch { }
                }

                Debug.WriteLine("Showed process window: " + processTitle + " WindowHandle: " + processWindowHandle + " ShowCmd: " + windowShowCommand);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed showing window: " + processTitle + "/" + ex.Message);
                return false;
            }
        }
    }
}