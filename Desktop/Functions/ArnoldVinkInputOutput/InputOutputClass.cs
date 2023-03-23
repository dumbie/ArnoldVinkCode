using System;

namespace ArnoldVinkCode
{
    public partial class AVInputOutputClass
    {
        public enum InputTypes : int
        {
            INPUT_MOUSE = 0,
            INPUT_KEYBOARD = 1,
            INPUT_HARDWARE = 2
        }

        public enum KeyboardEventFlags : int
        {
            KEYEVENTF_NONE = 0x0000,
            KEYEVENTF_EXTENDEDKEY = 0x0001,
            KEYEVENTF_KEYUP = 0x0002,
            KEYEVENTF_UNICODE = 0x0004,
            KEYEVENTF_SCANCODE = 0x0008
        }

        public enum MouseEventFlags : int
        {
            MOUSEEVENTF_MOVE = 0x0001,
            MOUSEEVENTF_LEFTDOWN = 0x0002,
            MOUSEEVENTF_LEFTUP = 0x0004,
            MOUSEEVENTF_RIGHTDOWN = 0x0008,
            MOUSEEVENTF_RIGHTUP = 0x0010,
            MOUSEEVENTF_MIDDLEDOWN = 0x0020,
            MOUSEEVENTF_MIDDLEUP = 0x0040,
            MOUSEEVENTF_XDOWN = 0x0080,
            MOUSEEVENTF_XUP = 0x0100,
            MOUSEEVENTF_VWHEEL = 0x0800,
            MOUSEEVENTF_HWHEEL = 0x01000
        }

        public enum KeysVirtualModifier : byte
        {
            None = 0x0000,
            Alt = 0x0001,
            Ctrl = 0x0002,
            Shift = 0x0004,
            Win = 0x0008
        }

        public enum KeysVirtual : byte
        {
            None = 0x00,
            LeftButton = 0x01,
            RightButton = 0x02,
            Break = 0x03,
            MiddleButton = 0x04,
            ExtraButton1 = 0x05,
            ExtraButton2 = 0x06,
            BackSpace = 0x08,
            Tab = 0x09,
            Clear = 0x0C,
            Enter = 0x0D,
            Shift = 0x10,
            ShiftLeft = 0xA0,
            ShiftRight = 0xA1,
            Control = 0x11,
            ControlLeft = 0xA2,
            ControlRight = 0xA3,
            Alt = 0x12,
            AltLeft = 0xA4,
            AltRight = 0xA5,
            Pause = 0x13,
            CapsLock = 0x14,
            Kana = 0x15,
            Hangeul = 0x15,
            Hangul = 0x15,
            Junja = 0x17,
            Final = 0x18,
            Hanja = 0x19,
            Kanji = 0x19,
            Escape = 0x1B,
            Convert = 0x1C,
            NonConvert = 0x1D,
            Accept = 0x1E,
            ModeChange = 0x1F,
            Space = 0x20,
            Prior = 0x21,
            Next = 0x22,
            End = 0x23,
            Home = 0x24,
            Left = 0x25,
            Up = 0x26,
            Right = 0x27,
            Down = 0x28,
            Select = 0x29,
            PrintScreen = 0x2A,
            Execute = 0x2B,
            Snapshot = 0x2C,
            Insert = 0x2D,
            Delete = 0x2E,
            Help = 0x2F,
            Digit0 = 0x30,
            Digit1 = 0x31,
            Digit2 = 0x32,
            Digit3 = 0x33,
            Digit4 = 0x34,
            Digit5 = 0x35,
            Digit6 = 0x36,
            Digit7 = 0x37,
            Digit8 = 0x38,
            Digit9 = 0x39,
            A = 0x41,
            B = 0x42,
            C = 0x43,
            D = 0x44,
            E = 0x45,
            F = 0x46,
            G = 0x47,
            H = 0x48,
            I = 0x49,
            J = 0x4A,
            K = 0x4B,
            L = 0x4C,
            M = 0x4D,
            N = 0x4E,
            O = 0x4F,
            P = 0x50,
            Q = 0x51,
            R = 0x52,
            S = 0x53,
            T = 0x54,
            U = 0x55,
            V = 0x56,
            W = 0x57,
            X = 0x58,
            Y = 0x59,
            Z = 0x5A,
            WindowsLeft = 0x5B,
            WindowsRight = 0x5C,
            ContextMenu = 0x5D,
            Sleep = 0x5F,
            Numpad0 = 0x60,
            Numpad1 = 0x61,
            Numpad2 = 0x62,
            Numpad3 = 0x63,
            Numpad4 = 0x64,
            Numpad5 = 0x65,
            Numpad6 = 0x66,
            Numpad7 = 0x67,
            Numpad8 = 0x68,
            Numpad9 = 0x69,
            Multiply = 0x6A,
            Add = 0x6B,
            Separator = 0x6C,
            Subtract = 0x6D,
            Decimal = 0x6E,
            Divide = 0x6F,
            F1 = 0x70,
            F2 = 0x71,
            F3 = 0x72,
            F4 = 0x73,
            F5 = 0x74,
            F6 = 0x75,
            F7 = 0x76,
            F8 = 0x77,
            F9 = 0x78,
            F10 = 0x79,
            F11 = 0x7A,
            F12 = 0x7B,
            F13 = 0x7C,
            F14 = 0x7D,
            F15 = 0x7E,
            F16 = 0x7F,
            F17 = 0x80,
            F18 = 0x81,
            F19 = 0x82,
            F20 = 0x83,
            F21 = 0x84,
            F22 = 0x85,
            F23 = 0x86,
            F24 = 0x87,
            NumLock = 0x90,
            ScrollLock = 0x91,
            NEC_Equal = 0x92,
            Fujitsu_Jisho = 0x92,
            Fujitsu_Masshou = 0x93,
            Fujitsu_Touroku = 0x94,
            Fujitsu_Loya = 0x95,
            Fujitsu_Roya = 0x96,
            BrowserBack = 0xA6,
            BrowserForward = 0xA7,
            BrowserRefresh = 0xA8,
            BrowserStop = 0xA9,
            BrowserSearch = 0xAA,
            BrowserFavorites = 0xAB,
            BrowserHome = 0xAC,
            VolumeMute = 0xAD,
            VolumeDown = 0xAE,
            VolumeUp = 0xAF,
            MediaNextTrack = 0xB0,
            MediaPreviousTrack = 0xB1,
            MediaStop = 0xB2,
            MediaPlayPause = 0xB3,
            LaunchMail = 0xB4,
            LaunchMedia = 0xB5,
            LaunchApplication1 = 0xB6,
            LaunchApplication2 = 0xB7,
            OEMSemicolon = 0xBA,
            OEMPlus = 0xBB,
            OEMComma = 0xBC,
            OEMMinus = 0xBD,
            OEMPeriod = 0xBE,
            OEMQuestion = 0xBF,
            OEMTilde = 0xC0,
            OEMOpenBracket = 0xDB,
            OEMCloseBracket = 0xDD,
            OEMPipe = 0xDC,
            OEMQuote = 0xDE,
            OEMMisc = 0xDF,
            OEMAx = 0xE1,
            OEMBackslash = 0xE2,
            ICOHelp = 0xE3,
            ICO00 = 0xE4,
            ProcessKey = 0xE5,
            ICOClear = 0xE6,
            Packet = 0xE7,
            OEMReset = 0xE9,
            OEMJump = 0xEA,
            OEMPA1 = 0xEB,
            OEMPA2 = 0xEC,
            OEMPA3 = 0xED,
            OEMWSCtrl = 0xEE,
            OEMCUSel = 0xEF,
            OEMATTN = 0xF0,
            OEMFinish = 0xF1,
            OEMCopy = 0xF2,
            OEMAuto = 0xF3,
            OEMENLW = 0xF4,
            OEMBackTab = 0xF5,
            OEMClear = 0xFE,
            ATTN = 0xF6,
            CRSel = 0xF7,
            EXSel = 0xF8,
            EREOF = 0xF9,
            Play = 0xFA,
            Zoom = 0xFB,
            Noname = 0xFC,
            PA1 = 0xFD
        }

        //Virtual keys that prefer extended
        public static KeysVirtual[] KeysVirtualExtended =
        {
            KeysVirtual.Up,
            KeysVirtual.Down,
            KeysVirtual.Left,
            KeysVirtual.Right,
            KeysVirtual.Delete
        };

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
                    case KeysVirtual.Add:
                        if (shortName) { return "Pad +"; } else { return "Numpad +"; }
                    case KeysVirtual.Subtract:
                        if (shortName) { return "Pad -"; } else { return "Numpad -"; }
                    case KeysVirtual.Divide:
                        if (shortName) { return "Pad /"; } else { return "Numpad /"; }
                    case KeysVirtual.Multiply:
                        if (shortName) { return "Pad *"; } else { return "Numpad *"; }
                    case KeysVirtual.Decimal:
                        if (shortName) { return "Pad ."; } else { return "Numpad ."; }
                    case KeysVirtual.Space:
                        if (shortName) { return "Spc"; } else { return "Spacebar"; }

                    //OEM
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
                    case KeysVirtual.OEMBackslash:
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
                    case KeysVirtual.Up:
                        if (shortName) { return "⯅"; } else { return "Arrow Up"; }
                    case KeysVirtual.Down:
                        if (shortName) { return "⯆"; } else { return "Arrow Down"; }
                    case KeysVirtual.Left:
                        if (shortName) { return "⯇"; } else { return "Arrow Left"; }
                    case KeysVirtual.Right:
                        if (shortName) { return "⯈"; } else { return "Arrow Right"; }
                    case KeysVirtual.Prior:
                        if (shortName) { return "PgUp"; } else { return "Page Up"; }
                    case KeysVirtual.Next:
                        if (shortName) { return "PgDn"; } else { return "Page Down"; }
                    case KeysVirtual.Home:
                        return "Home";
                    case KeysVirtual.End:
                        return "End";

                    //Action
                    case KeysVirtual.Break:
                        return "Break";
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
                    case KeysVirtual.NumLock:
                        if (shortName) { return "NLck"; } else { return "Num Lock"; }
                    case KeysVirtual.ScrollLock:
                        if (shortName) { return "SLck"; } else { return "Scroll Lock"; }
                    case KeysVirtual.Snapshot:
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
                    case KeysVirtual.Play:
                    case KeysVirtual.MediaPlayPause:
                        if (shortName) { return "Play"; } else { return "Media Play"; }
                    case KeysVirtual.MediaStop:
                        if (shortName) { return "Stop"; } else { return "Media Stop"; }

                    //Launch
                    case KeysVirtual.LaunchMedia:
                        if (shortName) { return "Media"; } else { return "Launch Media"; }
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