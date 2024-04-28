using System;

namespace ArnoldVinkCode
{
    public partial class AVInputOutputClass
    {
        public static KeysVirtual ConvertInputToVirtual(KeysInput keyboardKey, KeysInput systemKey)
        {
            KeysVirtual returnKey = KeysVirtual.None;
            try
            {
                if (systemKey != KeysInput.None)
                {
                    Enum.TryParse(Enum.GetName(systemKey), out returnKey);
                }
                else
                {
                    Enum.TryParse(Enum.GetName(keyboardKey), out returnKey);
                }
            }
            catch { }
            return returnKey;
        }

        public static KeysHid ConvertInputToHid(KeysInput keyboardKey, KeysInput systemKey)
        {
            KeysHid returnKey = KeysHid.None;
            try
            {
                if (systemKey != KeysInput.None)
                {
                    Enum.TryParse(Enum.GetName(systemKey), out returnKey);
                }
                else
                {
                    Enum.TryParse(Enum.GetName(keyboardKey), out returnKey);
                }
            }
            catch { }
            return returnKey;
        }
    }
}