using System;

namespace ArnoldVinkCode
{
    public partial class AVInputOutputClass
    {
        public static KeysHid ConvertVirtualToHid(KeysVirtual keyboardKey)
        {
            KeysHid returnKey = KeysHid.None;
            try
            {
                Enum.TryParse(Enum.GetName(keyboardKey), out returnKey);
            }
            catch { }
            return returnKey;
        }

        public static KeysInput ConvertVirtualToInput(KeysVirtual keyboardKey)
        {
            KeysInput returnKey = KeysInput.None;
            try
            {
                Enum.TryParse(Enum.GetName(keyboardKey), out returnKey);
            }
            catch { }
            return returnKey;
        }

        public static KeysModifierHid ConvertVirtualToModifierHid(KeysModifierVirtual keyboardModifier)
        {
            KeysModifierHid pressedModifiers = KeysModifierHid.None;
            try
            {
                if (keyboardModifier.HasFlag(KeysModifierVirtual.Alt)) { pressedModifiers |= KeysModifierHid.AltLeft; }
                if (keyboardModifier.HasFlag(KeysModifierVirtual.Control)) { pressedModifiers |= KeysModifierHid.ControlLeft; }
                if (keyboardModifier.HasFlag(KeysModifierVirtual.Shift)) { pressedModifiers |= KeysModifierHid.ShiftLeft; }
                if (keyboardModifier.HasFlag(KeysModifierVirtual.Windows)) { pressedModifiers |= KeysModifierHid.WindowsLeft; }
            }
            catch { }
            return pressedModifiers;
        }

        public static KeysMediaHid ConvertVirtualToMediaHid(KeysVirtual keyboardMedia)
        {
            try
            {
                switch (keyboardMedia)
                {
                    case KeysVirtual.MediaNextTrack:
                        return KeysMediaHid.Next;
                    case KeysVirtual.MediaPreviousTrack:
                        return KeysMediaHid.Previous;
                    case KeysVirtual.MediaStop:
                        return KeysMediaHid.Stop;
                    case KeysVirtual.MediaPlayPause:
                        return KeysMediaHid.PlayPause;
                    case KeysVirtual.VolumeMute:
                        return KeysMediaHid.VolumeMute;
                    case KeysVirtual.VolumeDown:
                        return KeysMediaHid.VolumeDown;
                    case KeysVirtual.VolumeUp:
                        return KeysMediaHid.VolumeUp;
                }
            }
            catch { }
            return KeysMediaHid.None;
        }
    }
}