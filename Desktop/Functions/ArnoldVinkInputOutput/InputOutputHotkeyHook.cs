using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVInputOutputHotkeyHook
    {
        //Variables
        private static IntPtr vHookKeyboard = IntPtr.Zero;
        private static IntPtr vHookChange = IntPtr.Zero;

        //Settings
        public static bool BlockGlobalKeyboardPresses = false;

        //Lists
        private static bool[] vListKeysPressed = new bool[255];

        //Events
        public static Action<KeyboardMessage> EventHotkeyPressedMessage;
        public static Action<bool[]> EventHotkeyPressedList;

        //Start receiving key input
        public static bool Start()
        {
            bool hooked = false;
            try
            {
                //Reset pressed keys
                vListKeysPressed = new bool[255];

                //Set window keyboard hook
                hooked = WindowHookKeyboard();

                //Set window change hook
                if (hooked)
                {
                    WindowHookChange();
                }
            }
            catch { }
            return hooked;
        }

        //Stop receiving key input
        public static void Stop()
        {
            try
            {
                //Reset pressed keys
                vListKeysPressed = new bool[255];

                //Remove window keyboard hook
                WindowUnhookKeyboard();

                //Remove window change hook
                WindowUnhookChange();
            }
            catch { }
        }

        //Restart receiving key input
        public static bool Restart()
        {
            bool hooked = false;
            try
            {
                //Reset pressed keys
                vListKeysPressed = new bool[255];

                //Remove window keyboard hook
                WindowUnhookKeyboard();

                //Set window keyboard hook
                hooked = WindowHookKeyboard();
            }
            catch { }
            return hooked;
        }

        //Set window keyboard hook
        private static bool WindowHookKeyboard()
        {
            try
            {
                if (vHookKeyboard != IntPtr.Zero)
                {
                    Debug.WriteLine("Window keyboard hook already set, unhook first.");
                    return false;
                }

                vHookKeyboard = SetWindowsHookEx(WindowHookTypes.WH_KEYBOARD_LL, WindowHookKeyboardDelegate, IntPtr.Zero, 0);
                Debug.WriteLine("Hooked window keyboard: " + vHookKeyboard);
            }
            catch { }
            return vHookKeyboard != IntPtr.Zero;
        }

        //Remove window keyboard hook
        private static bool WindowUnhookKeyboard()
        {
            bool unhooked = false;
            try
            {
                if (vHookKeyboard != IntPtr.Zero)
                {
                    unhooked = UnhookWindowsHookEx(vHookKeyboard);
                    //Debug.WriteLine("Unhooked window keyboard: " + unhooked);
                    vHookKeyboard = IntPtr.Zero;
                }
            }
            catch { }
            return unhooked;
        }

        //Set window change hook
        private static bool WindowHookChange()
        {
            try
            {
                if (vHookChange != IntPtr.Zero)
                {
                    Debug.WriteLine("Window change hook already set, unhook first.");
                    return false;
                }

                vHookChange = SetWinEventHook(WinEventHooks.EVENT_SYSTEM_FOREGROUND, WinEventHooks.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, WinEventHookDelegate, 0, 0, WinEventFlags.WINEVENT_OUTOFCONTEXT);
                Debug.WriteLine("Hooked window change: " + vHookChange);
            }
            catch { }
            return vHookChange != IntPtr.Zero;
        }

        //Remove window change hook
        private static bool WindowUnhookChange()
        {
            bool unhooked = false;
            try
            {
                if (vHookChange != IntPtr.Zero)
                {
                    unhooked = UnhookWinEvent(vHookChange);
                    //Debug.WriteLine("Unhooked window change: " + unhooked);
                    vHookChange = IntPtr.Zero;
                }
            }
            catch { }
            return unhooked;
        }

        //Check if hotkey is pressed
        public static bool CheckHotkeyPressed(bool[] keysPressed, List<KeysVirtual> keysHotkey)
        {
            try
            {
                return keysHotkey.Where(x => x != KeysVirtual.None).All(x => keysPressed[(int)x]);
            }
            catch { }
            return false;
        }

        public static bool CheckHotkeyPressed(bool[] keysPressed, KeysVirtual[] keysHotkey)
        {
            try
            {
                return keysHotkey.Where(x => x != KeysVirtual.None).All(x => keysPressed[(int)x]);
            }
            catch { }
            return false;
        }

        //Restart hook on window change
        private static async void WinEventHookDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hWnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            try
            {
                //Wait for window events that may unhook
                await Task.Delay(1000);

                //Restart window keyboard hook
                Restart();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("WinEventHookDelegate error: " + ex.Message);
            }
        }

        //Check received keyboard input
        private static IntPtr WindowHookKeyboardDelegate(int nCode, IntPtr wParam, KBDLLHOOKSTRUCT lParam)
        {
            try
            {
                if (nCode >= 0)
                {
                    //Update keys pressed list
                    if (wParam == (IntPtr)WindowMessages.WM_KEYDOWN || wParam == (IntPtr)WindowMessages.WM_SYSKEYDOWN)
                    {
                        if (EventHotkeyPressedList != null)
                        {
                            //Update key press
                            vListKeysPressed[lParam.vkCode] = true;

                            //Trigger hotkey event
                            EventHotkeyPressedList(vListKeysPressed);
                        }

                        if (EventHotkeyPressedMessage != null)
                        {
                            //Set keyboard message
                            KeyboardMessage keyMessage = new KeyboardMessage();
                            keyMessage.windowMessage = (WindowMessages)wParam;
                            keyMessage.keyVirtual = (KeysVirtual)lParam.vkCode;

                            //Trigger hotkey event
                            EventHotkeyPressedMessage(keyMessage);
                        }

                        //Debug.WriteLine("Keyboard down: " + (KeysVirtual)lParam.vkCode);
                    }
                    else if (wParam == (IntPtr)WindowMessages.WM_KEYUP || wParam == (IntPtr)WindowMessages.WM_SYSKEYUP)
                    {
                        if (EventHotkeyPressedList != null)
                        {
                            //Update key press
                            vListKeysPressed[lParam.vkCode] = false;
                        }

                        //Debug.WriteLine("Keyboard up: " + (KeysVirtual)lParam.vkCode);
                    }
                }

                if (BlockGlobalKeyboardPresses)
                {
                    return new IntPtr(1);
                }
                else
                {
                    return CallNextHookEx(vHookKeyboard, nCode, wParam, lParam);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("WindowHookKeyboardDelegate error: " + ex.Message);
                return IntPtr.Zero;
            }
        }
    }
}