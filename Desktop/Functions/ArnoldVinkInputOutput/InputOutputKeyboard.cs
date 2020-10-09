using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVInputOutputKeyboard
    {
        //Send single key press
        public static async Task KeySendSingle(KeysVirtual virtualKey, IntPtr windowHandle)
        {
            try
            {
                PostMessage(windowHandle, (int)WindowMessages.WM_KEYDOWN, (byte)virtualKey, 0); //Key Press
                await Task.Delay(10);
                PostMessage(windowHandle, (int)WindowMessages.WM_KEYUP, (byte)virtualKey, 0); //Key Release
            }
            catch { }
        }

        //Keyboard type string
        public static void KeyTypeString(string typeString)
        {
            try
            {
                INPUT[] input = new INPUT[typeString.Length];
                for (int i = 0; i < input.Length; i++)
                {
                    input[i] = new INPUT();
                    input[i].type = InputTypes.INPUT_KEYBOARD;
                    input[i].keyboard.wVk = 0;
                    input[i].keyboard.wScan = (short)typeString[i];
                    input[i].keyboard.time = 0;
                    input[i].keyboard.dwFlags = KeyboardEventFlags.KEYEVENTF_UNICODE;
                    input[i].keyboard.dwExtraInfo = IntPtr.Zero;
                }
                SendInput(input.Length, input, Marshal.SizeOf(typeof(INPUT)));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to type string: " + ex.Message);
            }
        }

        //Simulate single key press
        public static async Task KeyPressSingleAuto(KeysVirtual virtualKey)
        {
            try
            {
                bool keyExtendedPressVk = KeysVirtualExtended.Contains(virtualKey);
                await KeyPressSingle(virtualKey, keyExtendedPressVk);
            }
            catch { }
        }
        public static async Task KeyPressSingle(KeysVirtual virtualKey, bool extendedKey)
        {
            try
            {
                uint scanByte = 0;
                KeyboardEventFlags KeyFlagsDown = 0;
                KeyboardEventFlags KeyFlagsUp = 0;
                if (extendedKey)
                {
                    scanByte = MapVirtualKey(virtualKey, MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC_EX);
                    KeyFlagsDown = KeyboardEventFlags.KEYEVENTF_EXTENDEDKEY | KeyboardEventFlags.KEYEVENTF_NONE;
                    KeyFlagsUp = KeyboardEventFlags.KEYEVENTF_EXTENDEDKEY | KeyboardEventFlags.KEYEVENTF_KEYUP;
                }
                else
                {
                    scanByte = MapVirtualKey(virtualKey, MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC);
                    KeyFlagsDown = KeyboardEventFlags.KEYEVENTF_NONE;
                    KeyFlagsUp = KeyboardEventFlags.KEYEVENTF_KEYUP;
                }

                keybd_event(virtualKey, scanByte, KeyFlagsDown, 0); //Key Press
                await Task.Delay(10);
                keybd_event(virtualKey, scanByte, KeyFlagsUp, 0); //Key Release
            }
            catch { }
        }

        //Simulate single key up or down
        public static void KeyToggleSingleAuto(KeysVirtual virtualKey, bool toggleDown)
        {
            try
            {
                bool keyExtendedPressVk = KeysVirtualExtended.Contains(virtualKey);
                KeyToggleSingle(virtualKey, keyExtendedPressVk, toggleDown);
            }
            catch { }
        }
        public static void KeyToggleSingle(KeysVirtual virtualKey, bool extendedKey, bool toggleDown)
        {
            try
            {
                uint scanByte = 0;
                KeyboardEventFlags KeyFlagsDown = 0;
                KeyboardEventFlags KeyFlagsUp = 0;
                if (extendedKey)
                {
                    scanByte = MapVirtualKey(virtualKey, MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC_EX);
                    KeyFlagsDown = KeyboardEventFlags.KEYEVENTF_EXTENDEDKEY | KeyboardEventFlags.KEYEVENTF_NONE;
                    KeyFlagsUp = KeyboardEventFlags.KEYEVENTF_EXTENDEDKEY | KeyboardEventFlags.KEYEVENTF_KEYUP;
                }
                else
                {
                    scanByte = MapVirtualKey(virtualKey, MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC);
                    KeyFlagsDown = KeyboardEventFlags.KEYEVENTF_NONE;
                    KeyFlagsUp = KeyboardEventFlags.KEYEVENTF_KEYUP;
                }

                if (toggleDown)
                {
                    keybd_event(virtualKey, scanByte, KeyFlagsDown, 0); //Key Press
                }
                else
                {
                    keybd_event(virtualKey, scanByte, KeyFlagsUp, 0); //Key Release
                }
            }
            catch { }
        }

        //Simulate combo key press
        public static async Task KeyPressComboAuto(KeysVirtual modifierKey, KeysVirtual virtualKey)
        {
            try
            {
                bool keyExtendedPressMod = KeysVirtualExtended.Contains(modifierKey);
                bool keyExtendedPressVk = KeysVirtualExtended.Contains(virtualKey);
                await KeyPressCombo(modifierKey, keyExtendedPressMod, virtualKey, keyExtendedPressVk);
            }
            catch { }
        }
        public static async Task KeyPressCombo(KeysVirtual modifierKey, bool extendedModifier, KeysVirtual virtualKey, bool extendedVirtual)
        {
            try
            {
                uint scanByteMod = 0;
                KeyboardEventFlags KeyFlagsDownMod = 0;
                KeyboardEventFlags KeyFlagsUpMod = 0;
                if (extendedModifier)
                {
                    scanByteMod = MapVirtualKey(modifierKey, MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC_EX);
                    KeyFlagsDownMod = KeyboardEventFlags.KEYEVENTF_EXTENDEDKEY | KeyboardEventFlags.KEYEVENTF_NONE;
                    KeyFlagsUpMod = KeyboardEventFlags.KEYEVENTF_EXTENDEDKEY | KeyboardEventFlags.KEYEVENTF_KEYUP;
                }
                else
                {
                    scanByteMod = MapVirtualKey(modifierKey, MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC);
                    KeyFlagsDownMod = KeyboardEventFlags.KEYEVENTF_NONE;
                    KeyFlagsUpMod = KeyboardEventFlags.KEYEVENTF_KEYUP;
                }

                uint scanByteVk = 0;
                KeyboardEventFlags KeyFlagsDownVk = 0;
                KeyboardEventFlags KeyFlagsUpVk = 0;
                if (extendedVirtual)
                {
                    scanByteVk = MapVirtualKey(virtualKey, MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC_EX);
                    KeyFlagsDownVk = KeyboardEventFlags.KEYEVENTF_EXTENDEDKEY | KeyboardEventFlags.KEYEVENTF_NONE;
                    KeyFlagsUpVk = KeyboardEventFlags.KEYEVENTF_EXTENDEDKEY | KeyboardEventFlags.KEYEVENTF_KEYUP;
                }
                else
                {
                    scanByteVk = MapVirtualKey(virtualKey, MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC);
                    KeyFlagsDownVk = KeyboardEventFlags.KEYEVENTF_NONE;
                    KeyFlagsUpVk = KeyboardEventFlags.KEYEVENTF_KEYUP;
                }

                keybd_event(modifierKey, scanByteMod, KeyFlagsDownMod, 0); //Modifier Press
                keybd_event(virtualKey, scanByteVk, KeyFlagsDownVk, 0); //Key Press
                await Task.Delay(10);
                keybd_event(virtualKey, scanByteVk, KeyFlagsUpVk, 0); //Key Release
                keybd_event(modifierKey, scanByteMod, KeyFlagsUpMod, 0); //Modifier Release
            }
            catch { }
        }

        //Simulate combo key up or down
        public static void KeyToggleComboAuto(KeysVirtual modifierKey, KeysVirtual virtualKey, bool toggleDown)
        {
            try
            {
                bool keyExtendedPressMod = KeysVirtualExtended.Contains(modifierKey);
                bool keyExtendedPressVk = KeysVirtualExtended.Contains(virtualKey);
                KeyToggleCombo(modifierKey, keyExtendedPressMod, virtualKey, keyExtendedPressVk, toggleDown);
            }
            catch { }
        }
        public static void KeyToggleCombo(KeysVirtual modifierKey, bool extendedModifier, KeysVirtual virtualKey, bool extendedVirtual, bool toggleDown)
        {
            try
            {
                uint scanByteMod = 0;
                KeyboardEventFlags KeyFlagsDownMod = 0;
                KeyboardEventFlags KeyFlagsUpMod = 0;
                if (extendedModifier)
                {
                    scanByteMod = MapVirtualKey(modifierKey, MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC_EX);
                    KeyFlagsDownMod = KeyboardEventFlags.KEYEVENTF_EXTENDEDKEY | KeyboardEventFlags.KEYEVENTF_NONE;
                    KeyFlagsUpMod = KeyboardEventFlags.KEYEVENTF_EXTENDEDKEY | KeyboardEventFlags.KEYEVENTF_KEYUP;
                }
                else
                {
                    scanByteMod = MapVirtualKey(modifierKey, MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC);
                    KeyFlagsDownMod = KeyboardEventFlags.KEYEVENTF_NONE;
                    KeyFlagsUpMod = KeyboardEventFlags.KEYEVENTF_KEYUP;
                }

                uint scanByteVk = 0;
                KeyboardEventFlags KeyFlagsDownVk = 0;
                KeyboardEventFlags KeyFlagsUpVk = 0;
                if (extendedVirtual)
                {
                    scanByteVk = MapVirtualKey(virtualKey, MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC_EX);
                    KeyFlagsDownVk = KeyboardEventFlags.KEYEVENTF_EXTENDEDKEY | KeyboardEventFlags.KEYEVENTF_NONE;
                    KeyFlagsUpVk = KeyboardEventFlags.KEYEVENTF_EXTENDEDKEY | KeyboardEventFlags.KEYEVENTF_KEYUP;
                }
                else
                {
                    scanByteVk = MapVirtualKey(virtualKey, MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC);
                    KeyFlagsDownVk = KeyboardEventFlags.KEYEVENTF_NONE;
                    KeyFlagsUpVk = KeyboardEventFlags.KEYEVENTF_KEYUP;
                }

                if (toggleDown)
                {
                    keybd_event(modifierKey, scanByteMod, KeyFlagsDownMod, 0); //Modifier Press
                    keybd_event(virtualKey, scanByteVk, KeyFlagsDownVk, 0); //Key Press
                }
                else
                {
                    keybd_event(virtualKey, scanByteVk, KeyFlagsUpVk, 0); //Key Release
                    keybd_event(modifierKey, scanByteMod, KeyFlagsUpMod, 0); //Modifier Release
                }
            }
            catch { }
        }
    }
}