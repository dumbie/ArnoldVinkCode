using System;

namespace ArnoldVinkCode
{
    public partial class AVInputOutputClass
    {
        //Get virtual key name
        public static string GetVirtualKeyName(KeysVirtual virtualKey, bool shortName)
        {
            try
            {
                switch (virtualKey)
                {
                    //Letters
                    case KeysVirtual.A:
                    case KeysVirtual.B:
                    case KeysVirtual.C:
                    case KeysVirtual.D:
                    case KeysVirtual.E:
                    case KeysVirtual.F:
                    case KeysVirtual.G:
                    case KeysVirtual.H:
                    case KeysVirtual.I:
                    case KeysVirtual.J:
                    case KeysVirtual.K:
                    case KeysVirtual.L:
                    case KeysVirtual.M:
                    case KeysVirtual.N:
                    case KeysVirtual.O:
                    case KeysVirtual.P:
                    case KeysVirtual.Q:
                    case KeysVirtual.R:
                    case KeysVirtual.S:
                    case KeysVirtual.T:
                    case KeysVirtual.U:
                    case KeysVirtual.V:
                    case KeysVirtual.W:
                    case KeysVirtual.X:
                    case KeysVirtual.Y:
                    case KeysVirtual.Z:
                        return Enum.GetName(typeof(KeysVirtual), virtualKey);

                    //Digits
                    case KeysVirtual.Digit0:
                        return "0";
                    case KeysVirtual.Numpad0:
                        if (shortName) { return "Pad 0"; } else { return "Numpad 0"; }
                    case KeysVirtual.Digit1:
                        return "1";
                    case KeysVirtual.Numpad1:
                        if (shortName) { return "Pad 1"; } else { return "Numpad 1"; }
                    case KeysVirtual.Digit2:
                        return "2";
                    case KeysVirtual.Numpad2:
                        if (shortName) { return "Pad 2"; } else { return "Numpad 2"; }
                    case KeysVirtual.Digit3:
                        return "3";
                    case KeysVirtual.Numpad3:
                        if (shortName) { return "Pad 3"; } else { return "Numpad 3"; }
                    case KeysVirtual.Digit4:
                        return "4";
                    case KeysVirtual.Numpad4:
                        if (shortName) { return "Pad 4"; } else { return "Numpad 4"; }
                    case KeysVirtual.Digit5:
                        return "5";
                    case KeysVirtual.Numpad5:
                        if (shortName) { return "Pad 5"; } else { return "Numpad 5"; }
                    case KeysVirtual.Digit6:
                        return "6";
                    case KeysVirtual.Numpad6:
                        if (shortName) { return "Pad 6"; } else { return "Numpad 6"; }
                    case KeysVirtual.Digit7:
                        return "7";
                    case KeysVirtual.Numpad7:
                        if (shortName) { return "Pad 7"; } else { return "Numpad 7"; }
                    case KeysVirtual.Digit8:
                        return "8";
                    case KeysVirtual.Numpad8:
                        if (shortName) { return "Pad 8"; } else { return "Numpad 8"; }
                    case KeysVirtual.Digit9:
                        return "9";
                    case KeysVirtual.Numpad9:
                        if (shortName) { return "Pad 9"; } else { return "Numpad 9"; }

                    //Numpad
                    case KeysVirtual.NumpadAdd:
                        if (shortName) { return "Pad +"; } else { return "Numpad +"; }
                    case KeysVirtual.NumpadSubtract:
                        if (shortName) { return "Pad -"; } else { return "Numpad -"; }
                    case KeysVirtual.NumpadDivide:
                        if (shortName) { return "Pad /"; } else { return "Numpad /"; }
                    case KeysVirtual.NumpadMultiply:
                        if (shortName) { return "Pad *"; } else { return "Numpad *"; }
                    case KeysVirtual.NumpadDecimal:
                        if (shortName) { return "Pad ."; } else { return "Numpad ."; }
                    case KeysVirtual.Space:
                        if (shortName) { return "Spc"; } else { return "Spacebar"; }

                    //Characters
                    case KeysVirtual.OEMSemicolon:
                        return ";";
                    case KeysVirtual.OEMQuestion:
                        return "?";
                    case KeysVirtual.OEMTilde:
                        return "~";
                    case KeysVirtual.OEMOpenBracket:
                        return "[";
                    case KeysVirtual.OEMCloseBracket:
                        return "]";
                    case KeysVirtual.OEMPipe:
                        return "|";
                    case KeysVirtual.OEMBackSlash:
                        return "\\";
                    case KeysVirtual.OEMQuote:
                        return "'";
                    case KeysVirtual.OEMPlus:
                        return "+";
                    case KeysVirtual.OEMMinus:
                        return "-";
                    case KeysVirtual.OEMComma:
                        return ",";
                    case KeysVirtual.OEMPeriod:
                        return ".";

                    //Function
                    case KeysVirtual.F1:
                    case KeysVirtual.F2:
                    case KeysVirtual.F3:
                    case KeysVirtual.F4:
                    case KeysVirtual.F5:
                    case KeysVirtual.F6:
                    case KeysVirtual.F7:
                    case KeysVirtual.F8:
                    case KeysVirtual.F9:
                    case KeysVirtual.F10:
                    case KeysVirtual.F11:
                    case KeysVirtual.F12:
                    case KeysVirtual.F13:
                    case KeysVirtual.F14:
                    case KeysVirtual.F15:
                    case KeysVirtual.F16:
                    case KeysVirtual.F17:
                    case KeysVirtual.F18:
                    case KeysVirtual.F19:
                    case KeysVirtual.F20:
                    case KeysVirtual.F21:
                    case KeysVirtual.F22:
                    case KeysVirtual.F23:
                    case KeysVirtual.F24:
                        return Enum.GetName(typeof(KeysVirtual), virtualKey);

                    //Navigation
                    case KeysVirtual.ArrowUp:
                        if (shortName) { return "⯅"; } else { return "Arrow Up"; }
                    case KeysVirtual.ArrowDown:
                        if (shortName) { return "⯆"; } else { return "Arrow Down"; }
                    case KeysVirtual.ArrowLeft:
                        if (shortName) { return "⯇"; } else { return "Arrow Left"; }
                    case KeysVirtual.ArrowRight:
                        if (shortName) { return "⯈"; } else { return "Arrow Right"; }
                    case KeysVirtual.PageUp:
                        if (shortName) { return "PgUp"; } else { return "Page Up"; }
                    case KeysVirtual.PageDown:
                        if (shortName) { return "PgDn"; } else { return "Page Down"; }
                    case KeysVirtual.Home:
                        return "Home";
                    case KeysVirtual.End:
                        return "End";

                    //Action
                    case KeysVirtual.Cancel:
                        return "Cancel";
                    case KeysVirtual.Clear:
                        return "Clear";
                    case KeysVirtual.BackSpace:
                        if (shortName) { return "Bspc"; } else { return "Backspace"; }
                    case KeysVirtual.Tab:
                        return "Tab";
                    case KeysVirtual.Escape:
                        if (shortName) { return "Esc"; } else { return "Escape"; }
                    case KeysVirtual.Enter:
                        return "Enter";
                    case KeysVirtual.Shift:
                    case KeysVirtual.ShiftLeft:
                    case KeysVirtual.ShiftRight:
                        return "Shift";
                    case KeysVirtual.Control:
                    case KeysVirtual.ControlLeft:
                    case KeysVirtual.ControlRight:
                        if (shortName) { return "Ctrl"; } else { return "Control"; }
                    case KeysVirtual.Alt:
                    case KeysVirtual.AltLeft:
                    case KeysVirtual.AltRight:
                        return "Alt";
                    case KeysVirtual.Pause:
                        return "Pause";
                    case KeysVirtual.CapsLock:
                        if (shortName) { return "Caps"; } else { return "Caps Lock"; }
                    case KeysVirtual.NumpadLock:
                        if (shortName) { return "NLck"; } else { return "Numpad Lock"; }
                    case KeysVirtual.ScrollLock:
                        if (shortName) { return "SLck"; } else { return "Scroll Lock"; }
                    case KeysVirtual.PrintScreen:
                        if (shortName) { return "PrtSc"; } else { return "Print Screen"; }
                    case KeysVirtual.WindowsLeft:
                    case KeysVirtual.WindowsRight:
                        if (shortName) { return "Win"; } else { return "Windows"; }
                    case KeysVirtual.Insert:
                        return "Insert";
                    case KeysVirtual.Delete:
                        return "Delete";
                    case KeysVirtual.Help:
                        return "Help";
                    case KeysVirtual.ContextMenu:
                        return "Menu";
                    case KeysVirtual.Zoom:
                        return "Zoom";

                    //Browser
                    case KeysVirtual.BrowserFavorites:
                        if (shortName) { return "BFav"; } else { return "Browser Favorites"; }
                    case KeysVirtual.BrowserBack:
                        if (shortName) { return "BBack"; } else { return "Browser Back"; }
                    case KeysVirtual.BrowserForward:
                        if (shortName) { return "BFrwd"; } else { return "Browser Forward"; }
                    case KeysVirtual.BrowserHome:
                        if (shortName) { return "BHome"; } else { return "Browser Home"; }
                    case KeysVirtual.BrowserRefresh:
                        if (shortName) { return "BRfsh"; } else { return "Browser Refresh"; }
                    case KeysVirtual.BrowserSearch:
                        if (shortName) { return "BSrch"; } else { return "Browser Search"; }
                    case KeysVirtual.BrowserStop:
                        if (shortName) { return "BStop"; } else { return "Browser Stop"; }

                    //Media
                    case KeysVirtual.VolumeUp:
                        if (shortName) { return "VolUp"; } else { return "Volume Up"; }
                    case KeysVirtual.VolumeDown:
                        if (shortName) { return "VolDn"; } else { return "Volume Down"; }
                    case KeysVirtual.VolumeMute:
                        if (shortName) { return "Mute"; } else { return "Volume Mute"; }
                    case KeysVirtual.MediaNextTrack:
                        if (shortName) { return "Next"; } else { return "Media Next"; }
                    case KeysVirtual.MediaPreviousTrack:
                        if (shortName) { return "Prev"; } else { return "Media Previous"; }
                    case KeysVirtual.MediaPlayPause:
                        if (shortName) { return "Play"; } else { return "Media Play/Pause"; }
                    case KeysVirtual.MediaStop:
                        if (shortName) { return "Stop"; } else { return "Media Stop"; }

                    //Launch
                    case KeysVirtual.SelectMedia:
                        if (shortName) { return "Media"; } else { return "Select Media"; }
                    case KeysVirtual.LaunchMail:
                        if (shortName) { return "Mail"; } else { return "Launch Mail"; }
                    case KeysVirtual.LaunchApplication1:
                        if (shortName) { return "App 1"; } else { return "Launch App 1"; }
                    case KeysVirtual.LaunchApplication2:
                        if (shortName) { return "App 2"; } else { return "Launch App 2"; }
                }
            }
            catch { }
            return Enum.GetName(typeof(KeysVirtual), virtualKey);
        }
    }
}