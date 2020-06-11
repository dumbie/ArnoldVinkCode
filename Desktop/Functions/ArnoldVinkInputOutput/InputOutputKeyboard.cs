using System;
using System.Linq;
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

        ////Send combo key press
        //public static void KeySendCombo(byte Modifier, byte virtualKey, IntPtr WindowHandle)
        //{
        //    try
        //    {
        //        //PostMessage and SendMessage does not support key combinations
        //    }
        //    catch { }
        //}

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
                uint KeyFlagsDown = 0;
                uint KeyFlagsUp = 0;
                if (extendedKey)
                {
                    scanByte = MapVirtualKey(virtualKey, MAPVK_VK_TO_VSC_EX);
                    KeyFlagsDown = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_NONE;
                    KeyFlagsUp = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
                }
                else
                {
                    scanByte = MapVirtualKey(virtualKey, MAPVK_VK_TO_VSC);
                    KeyFlagsDown = KEYEVENTF_NONE;
                    KeyFlagsUp = KEYEVENTF_KEYUP;
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
                uint KeyFlagsDown = 0;
                uint KeyFlagsUp = 0;
                if (extendedKey)
                {
                    scanByte = MapVirtualKey(virtualKey, MAPVK_VK_TO_VSC_EX);
                    KeyFlagsDown = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_NONE;
                    KeyFlagsUp = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
                }
                else
                {
                    scanByte = MapVirtualKey(virtualKey, MAPVK_VK_TO_VSC);
                    KeyFlagsDown = KEYEVENTF_NONE;
                    KeyFlagsUp = KEYEVENTF_KEYUP;
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
                uint KeyFlagsDownMod = 0;
                uint KeyFlagsUpMod = 0;
                if (extendedModifier)
                {
                    scanByteMod = MapVirtualKey(modifierKey, MAPVK_VK_TO_VSC_EX);
                    KeyFlagsDownMod = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_NONE;
                    KeyFlagsUpMod = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
                }
                else
                {
                    scanByteMod = MapVirtualKey(modifierKey, MAPVK_VK_TO_VSC);
                    KeyFlagsDownMod = KEYEVENTF_NONE;
                    KeyFlagsUpMod = KEYEVENTF_KEYUP;
                }

                uint scanByteVk = 0;
                uint KeyFlagsDownVk = 0;
                uint KeyFlagsUpVk = 0;
                if (extendedVirtual)
                {
                    scanByteVk = MapVirtualKey(virtualKey, MAPVK_VK_TO_VSC_EX);
                    KeyFlagsDownVk = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_NONE;
                    KeyFlagsUpVk = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
                }
                else
                {
                    scanByteVk = MapVirtualKey(virtualKey, MAPVK_VK_TO_VSC);
                    KeyFlagsDownVk = KEYEVENTF_NONE;
                    KeyFlagsUpVk = KEYEVENTF_KEYUP;
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
                uint KeyFlagsDownMod = 0;
                uint KeyFlagsUpMod = 0;
                if (extendedModifier)
                {
                    scanByteMod = MapVirtualKey(modifierKey, MAPVK_VK_TO_VSC_EX);
                    KeyFlagsDownMod = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_NONE;
                    KeyFlagsUpMod = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
                }
                else
                {
                    scanByteMod = MapVirtualKey(modifierKey, MAPVK_VK_TO_VSC);
                    KeyFlagsDownMod = KEYEVENTF_NONE;
                    KeyFlagsUpMod = KEYEVENTF_KEYUP;
                }

                uint scanByteVk = 0;
                uint KeyFlagsDownVk = 0;
                uint KeyFlagsUpVk = 0;
                if (extendedVirtual)
                {
                    scanByteVk = MapVirtualKey(virtualKey, MAPVK_VK_TO_VSC_EX);
                    KeyFlagsDownVk = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_NONE;
                    KeyFlagsUpVk = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
                }
                else
                {
                    scanByteVk = MapVirtualKey(virtualKey, MAPVK_VK_TO_VSC);
                    KeyFlagsDownVk = KEYEVENTF_NONE;
                    KeyFlagsUpVk = KEYEVENTF_KEYUP;
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