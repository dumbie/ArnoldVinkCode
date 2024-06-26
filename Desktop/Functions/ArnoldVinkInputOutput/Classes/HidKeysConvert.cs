﻿using System;

namespace ArnoldVinkCode
{
    public partial class AVInputOutputClass
    {
        public static KeysVirtual ConvertHidToVirtual(KeysHid keyboardKey)
        {
            KeysVirtual returnKey = KeysVirtual.None;
            try
            {
                Enum.TryParse(Enum.GetName(keyboardKey), out returnKey);
            }
            catch { }
            return returnKey;
        }

        public static KeysInput ConvertHidToInput(KeysHid keyboardKey)
        {
            KeysInput returnKey = KeysInput.None;
            try
            {
                Enum.TryParse(Enum.GetName(keyboardKey), out returnKey);
            }
            catch { }
            return returnKey;
        }
    }
}