using System;

namespace ArnoldVinkCode
{
    public partial class AVInputOutputClass
    {
        public static KeysVirtual ConvertInputToVirtual(KeysInput keyboardKey)
        {
            KeysVirtual returnKey = KeysVirtual.None;
            try
            {
                Enum.TryParse(Enum.GetName(keyboardKey), out returnKey);
            }
            catch { }
            return returnKey;
        }

        public static KeysHid ConvertInputToHid(KeysInput keyboardKey)
        {
            KeysHid returnKey = KeysHid.None;
            try
            {
                Enum.TryParse(Enum.GetName(keyboardKey), out returnKey);
            }
            catch { }
            return returnKey;
        }
    }
}