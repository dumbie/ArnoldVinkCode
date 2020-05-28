using System;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVInputOutputKeyboard
    {
        //Send single key press
        public static async Task KeySendSingle(byte virtualKey, IntPtr windowHandle)
        {
            try
            {
                PostMessage(windowHandle, (int)WindowMessages.WM_KEYDOWN, virtualKey, 0); //Key Press
                await Task.Delay(10);
                PostMessage(windowHandle, (int)WindowMessages.WM_KEYUP, virtualKey, 0); //Key Release
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
        public static async Task KeyPressSingle(byte virtualKey, bool extendedKey)
        {
            try
            {
                byte scanByte = Convert.ToByte(MapVirtualKey(virtualKey, MAPVK_VK_TO_VSC));
                uint KeyFlagsDown = KEYEVENTF_NONE;
                uint KeyFlagsUp = KEYEVENTF_KEYUP;

                if (extendedKey)
                {
                    scanByte = Convert.ToByte(MapVirtualKey(virtualKey, MAPVK_VK_TO_VSC_EX));
                    KeyFlagsDown = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_NONE;
                    KeyFlagsUp = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
                }

                keybd_event(virtualKey, scanByte, KeyFlagsDown, 0); //Key Press
                await Task.Delay(10);
                keybd_event(virtualKey, scanByte, KeyFlagsUp, 0); //Key Release
            }
            catch { }
        }

        //Simulate single key up or down
        public static void KeyToggleSingle(byte virtualKey, bool extendedKey, bool toggleDown)
        {
            try
            {
                byte scanByte = Convert.ToByte(MapVirtualKey(virtualKey, MAPVK_VK_TO_VSC));
                uint KeyFlagsDown = KEYEVENTF_NONE;
                uint KeyFlagsUp = KEYEVENTF_KEYUP;

                if (extendedKey)
                {
                    scanByte = Convert.ToByte(MapVirtualKey(virtualKey, MAPVK_VK_TO_VSC_EX));
                    KeyFlagsDown = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_NONE;
                    KeyFlagsUp = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
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
        public static async Task KeyPressCombo(byte modifierKey, byte virtualKey, bool extendedKey)
        {
            try
            {
                byte scanByteVk = Convert.ToByte(MapVirtualKey(virtualKey, MAPVK_VK_TO_VSC));
                byte scanByteMod = Convert.ToByte(MapVirtualKey(modifierKey, MAPVK_VK_TO_VSC));
                uint KeyFlagsDown = KEYEVENTF_NONE;
                uint KeyFlagsUp = KEYEVENTF_KEYUP;

                if (extendedKey)
                {
                    scanByteVk = Convert.ToByte(MapVirtualKey(virtualKey, MAPVK_VK_TO_VSC_EX));
                    scanByteMod = Convert.ToByte(MapVirtualKey(modifierKey, MAPVK_VK_TO_VSC_EX));
                    KeyFlagsDown = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_NONE;
                    KeyFlagsUp = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
                }

                keybd_event(modifierKey, scanByteMod, KeyFlagsDown, 0); //Modifier Press
                keybd_event(virtualKey, scanByteVk, KeyFlagsDown, 0); //Key Press
                await Task.Delay(10);
                keybd_event(virtualKey, scanByteVk, KeyFlagsUp, 0); //Key Release
                keybd_event(modifierKey, scanByteMod, KeyFlagsUp, 0); //Modifier Release
            }
            catch { }
        }

        //Simulate combo key up or down
        public static void KeyToggleCombo(byte modifierKey, byte virtualKey, bool extendedKey, bool toggleDown)
        {
            try
            {
                byte scanByteVk = Convert.ToByte(MapVirtualKey(virtualKey, MAPVK_VK_TO_VSC));
                byte scanByteMod = Convert.ToByte(MapVirtualKey(modifierKey, MAPVK_VK_TO_VSC));
                uint KeyFlagsDown = KEYEVENTF_NONE;
                uint KeyFlagsUp = KEYEVENTF_KEYUP;

                if (extendedKey)
                {
                    scanByteVk = Convert.ToByte(MapVirtualKey(virtualKey, MAPVK_VK_TO_VSC_EX));
                    scanByteMod = Convert.ToByte(MapVirtualKey(modifierKey, MAPVK_VK_TO_VSC_EX));
                    KeyFlagsDown = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_NONE;
                    KeyFlagsUp = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
                }

                if (toggleDown)
                {
                    keybd_event(modifierKey, scanByteMod, KeyFlagsDown, 0); //Modifier Press
                    keybd_event(virtualKey, scanByteVk, KeyFlagsDown, 0); //Key Press
                }
                else
                {
                    keybd_event(virtualKey, scanByteVk, KeyFlagsUp, 0); //Key Release
                    keybd_event(modifierKey, scanByteMod, KeyFlagsUp, 0); //Modifier Release
                }
            }
            catch { }
        }
    }
}