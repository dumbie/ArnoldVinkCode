using System;

namespace ArnoldVinkCode
{
    public partial class AVInputOutputClass
    {
        public static string GetKeyboardMediaName(KeysMediaHid multimediaKey, bool shortName)
        {
            try
            {
                switch (multimediaKey)
                {
                    case KeysMediaHid.Next:
                        if (shortName) { return "Next"; } else { return "Media Next"; }
                    case KeysMediaHid.Previous:
                        if (shortName) { return "Prev"; } else { return "Media Previous"; }
                    case KeysMediaHid.Stop:
                        if (shortName) { return "Stop"; } else { return "Media Stop"; }
                    case KeysMediaHid.PlayPause:
                        if (shortName) { return "Play"; } else { return "Media Play/Pause"; }
                    case KeysMediaHid.VolumeMute:
                        if (shortName) { return "Mute"; } else { return "Volume Mute"; }
                    case KeysMediaHid.VolumeDown:
                        if (shortName) { return "VolDn"; } else { return "Volume Down"; }
                    case KeysMediaHid.VolumeUp:
                        if (shortName) { return "VolUp"; } else { return "Volume Up"; }
                }
            }
            catch { }
            return Enum.GetName(multimediaKey);
        }

        public static string GetKeyboardModifiersName(KeysModifierHid keyboardModifier, bool shortName)
        {
            try
            {
                switch (keyboardModifier)
                {
                    case KeysModifierHid.ControlLeft:
                    case KeysModifierHid.ControlRight:
                        if (shortName) { return "Ctrl"; } else { return "Control"; }
                    case KeysModifierHid.ShiftLeft:
                    case KeysModifierHid.ShiftRight:
                        return "Shift";
                    case KeysModifierHid.AltLeft:
                    case KeysModifierHid.AltRight:
                        if (shortName) { return "Alt"; } else { return "Alternate"; }
                    case KeysModifierHid.WindowsLeft:
                    case KeysModifierHid.WindowsRight:
                        if (shortName) { return "Win"; } else { return "Windows"; }
                }
            }
            catch { }
            return Enum.GetName(keyboardModifier);
        }

        public static string GetKeyboardKeysName(KeysHid keyboardKey, bool shortName)
        {
            try
            {
                switch (keyboardKey)
                {
                    case KeysHid.A:
                    case KeysHid.B:
                    case KeysHid.C:
                    case KeysHid.D:
                    case KeysHid.E:
                    case KeysHid.F:
                    case KeysHid.G:
                    case KeysHid.H:
                    case KeysHid.I:
                    case KeysHid.J:
                    case KeysHid.K:
                    case KeysHid.L:
                    case KeysHid.M:
                    case KeysHid.N:
                    case KeysHid.O:
                    case KeysHid.P:
                    case KeysHid.Q:
                    case KeysHid.R:
                    case KeysHid.S:
                    case KeysHid.T:
                    case KeysHid.U:
                    case KeysHid.V:
                    case KeysHid.W:
                    case KeysHid.X:
                    case KeysHid.Y:
                    case KeysHid.Z:
                        return Enum.GetName(keyboardKey);
                    case KeysHid.Digit1:
                        return "1";
                    case KeysHid.Digit2:
                        return "2";
                    case KeysHid.Digit3:
                        return "3";
                    case KeysHid.Digit4:
                        return "4";
                    case KeysHid.Digit5:
                        return "5";
                    case KeysHid.Digit6:
                        return "6";
                    case KeysHid.Digit7:
                        return "7";
                    case KeysHid.Digit8:
                        return "8";
                    case KeysHid.Digit9:
                        return "9";
                    case KeysHid.Digit0:
                        return "0";
                    case KeysHid.Enter:
                        return "Enter";
                    case KeysHid.Escape:
                        if (shortName) { return "Esc"; } else { return "Escape"; }
                    case KeysHid.BackSpace:
                        if (shortName) { return "BkSpc"; } else { return "Backspace"; }
                    case KeysHid.Tab:
                        return "Tab";
                    case KeysHid.Space:
                        if (shortName) { return "Spc"; } else { return "Spacebar"; }
                    case KeysHid.Minus:
                        return "-";
                    case KeysHid.Plus:
                        return "=";
                    case KeysHid.CloseBracket:
                        return "[";
                    case KeysHid.OpenBracket:
                        return "]";
                    case KeysHid.BackSlash:
                        return "\\";
                    case KeysHid.Semicolon:
                        return ";";
                    case KeysHid.Quote:
                        return "'";
                    case KeysHid.Tilde:
                        return "`";
                    case KeysHid.Comma:
                        return ",";
                    case KeysHid.Period:
                        return ".";
                    case KeysHid.Question:
                        return "/";
                    case KeysHid.CapsLock:
                        if (shortName) { return "CpLck"; } else { return "Caps Lock"; }
                    case KeysHid.F1:
                    case KeysHid.F2:
                    case KeysHid.F3:
                    case KeysHid.F4:
                    case KeysHid.F5:
                    case KeysHid.F6:
                    case KeysHid.F7:
                    case KeysHid.F8:
                    case KeysHid.F9:
                    case KeysHid.F10:
                    case KeysHid.F11:
                    case KeysHid.F12:
                        return Enum.GetName(keyboardKey);
                    case KeysHid.PrintScreen:
                        if (shortName) { return "PrtSc"; } else { return "Print Screen"; }
                    case KeysHid.ScrollLock:
                        if (shortName) { return "ScLck"; } else { return "Scroll Lock"; }
                    case KeysHid.Pause:
                        return "Pause";
                    case KeysHid.Insert:
                        return "Insert";
                    case KeysHid.Home:
                        return "Home";
                    case KeysHid.PageUp:
                        if (shortName) { return "PgUp"; } else { return "Page Up"; }
                    case KeysHid.Delete:
                        return "Delete";
                    case KeysHid.End:
                        return "End";
                    case KeysHid.PageDown:
                        if (shortName) { return "PgDn"; } else { return "Page Down"; }
                    case KeysHid.ArrowRight:
                        if (shortName) { return "⯈"; } else { return "Arrow Right"; }
                    case KeysHid.ArrowLeft:
                        if (shortName) { return "⯇"; } else { return "Arrow Left"; }
                    case KeysHid.ArrowDown:
                        if (shortName) { return "⯆"; } else { return "Arrow Down"; }
                    case KeysHid.ArrowUp:
                        if (shortName) { return "⯅"; } else { return "Arrow Up"; }
                    case KeysHid.NumpadLock:
                        if (shortName) { return "NmLck"; } else { return "Numpad Lock"; }
                    case KeysHid.NumpadDivide:
                        if (shortName) { return "Pad /"; } else { return "Numpad /"; }
                    case KeysHid.NumpadMultiply:
                        if (shortName) { return "Pad *"; } else { return "Numpad *"; }
                    case KeysHid.NumpadSubtract:
                        if (shortName) { return "Pad -"; } else { return "Numpad -"; }
                    case KeysHid.NumpadAdd:
                        if (shortName) { return "Pad +"; } else { return "Numpad +"; }
                    case KeysHid.NumpadEnter:
                        if (shortName) { return "Pad Ent"; } else { return "Numpad Enter"; }
                    case KeysHid.Numpad1:
                        if (shortName) { return "Pad 1"; } else { return "Numpad 1"; }
                    case KeysHid.Numpad2:
                        if (shortName) { return "Pad 2"; } else { return "Numpad 2"; }
                    case KeysHid.Numpad3:
                        if (shortName) { return "Pad 3"; } else { return "Numpad 3"; }
                    case KeysHid.Numpad4:
                        if (shortName) { return "Pad 4"; } else { return "Numpad 4"; }
                    case KeysHid.Numpad5:
                        if (shortName) { return "Pad 5"; } else { return "Numpad 5"; }
                    case KeysHid.Numpad6:
                        if (shortName) { return "Pad 6"; } else { return "Numpad 6"; }
                    case KeysHid.Numpad7:
                        if (shortName) { return "Pad 7"; } else { return "Numpad 7"; }
                    case KeysHid.Numpad8:
                        if (shortName) { return "Pad 8"; } else { return "Numpad 8"; }
                    case KeysHid.Numpad9:
                        if (shortName) { return "Pad 9"; } else { return "Numpad 9"; }
                    case KeysHid.Numpad0:
                        if (shortName) { return "Pad 0"; } else { return "Numpad 0"; }
                    case KeysHid.NumpadDecimal:
                        if (shortName) { return "Pad ."; } else { return "Numpad ."; }
                }
            }
            catch { }
            return Enum.GetName(keyboardKey);
        }
    }
}