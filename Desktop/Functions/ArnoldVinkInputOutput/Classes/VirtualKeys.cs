﻿namespace ArnoldVinkCode
{
    public partial class AVInputOutputClass
    {
        //Matches: Windows.System.VirtualKeyModifiers
        public enum KeysModifierVirtual : byte
        {
            None = 0,
            Ctrl = 1,
            Alt = 2,
            Shift = 4,
            Windows = 8
        }

        //Matches: System.Windows.Forms.Keys and Windows.System.VirtualKey
        public enum KeysVirtual : byte
        {
            None = 0,
            LeftButton = 1,
            RightButton = 2,
            Cancel = 3,
            MiddleButton = 4,
            XButton1 = 5,
            XButton2 = 6,
            BackSpace = 8,
            Tab = 9,
            LineFeed = 10,
            Clear = 12,
            Enter = 13,
            Shift = 16,
            ShiftLeft = 160,
            ShiftRight = 161,
            Control = 17,
            ControlLeft = 162,
            ControlRight = 163,
            Alt = 18,
            AltLeft = 164,
            AltRight = 165,
            Pause = 19,
            CapsLock = 20,
            KanaMode = 21,
            HangulMode = 21,
            IMEOn = 22,
            JunjaMode = 23,
            FinalMode = 24,
            HanjaMode = 25,
            KanjiMode = 25,
            IMEOff = 26,
            Escape = 27,
            IMEConvert = 28,
            IMENonConvert = 29,
            IMEAccept = 30,
            IMEModeChange = 31,
            Space = 32,
            PageUp = 33,
            PageDown = 34,
            End = 35,
            Home = 36,
            ArrowLeft = 37,
            ArrowUp = 38,
            ArrowRight = 39,
            ArrowDown = 40,
            Select = 41,
            Print = 42,
            Execute = 43,
            PrintScreen = 44,
            Insert = 45,
            Delete = 46,
            Help = 47,
            Digit0 = 48,
            Digit1 = 49,
            Digit2 = 50,
            Digit3 = 51,
            Digit4 = 52,
            Digit5 = 53,
            Digit6 = 54,
            Digit7 = 55,
            Digit8 = 56,
            Digit9 = 57,
            A = 65,
            B = 66,
            C = 67,
            D = 68,
            E = 69,
            F = 70,
            G = 71,
            H = 72,
            I = 73,
            J = 74,
            K = 75,
            L = 76,
            M = 77,
            N = 78,
            O = 79,
            P = 80,
            Q = 81,
            R = 82,
            S = 83,
            T = 84,
            U = 85,
            V = 86,
            W = 87,
            X = 88,
            Y = 89,
            Z = 90,
            WindowsLeft = 91,
            WindowsRight = 92,
            ContextMenu = 93,
            Sleep = 95,
            Numpad0 = 96,
            Numpad1 = 97,
            Numpad2 = 98,
            Numpad3 = 99,
            Numpad4 = 100,
            Numpad5 = 101,
            Numpad6 = 102,
            Numpad7 = 103,
            Numpad8 = 104,
            Numpad9 = 105,
            NumpadMultiply = 106,
            NumpadAdd = 107,
            NumpadSeparator = 108,
            NumpadSubtract = 109,
            NumpadDecimal = 110,
            NumpadDivide = 111,
            F1 = 112,
            F2 = 113,
            F3 = 114,
            F4 = 115,
            F5 = 116,
            F6 = 117,
            F7 = 118,
            F8 = 119,
            F9 = 120,
            F10 = 121,
            F11 = 122,
            F12 = 123,
            F13 = 124,
            F14 = 125,
            F15 = 126,
            F16 = 127,
            F17 = 128,
            F18 = 129,
            F19 = 130,
            F20 = 131,
            F21 = 132,
            F22 = 133,
            F23 = 134,
            F24 = 135,
            NumpadLock = 144,
            ScrollLock = 145,
            BrowserBack = 166,
            BrowserForward = 167,
            BrowserRefresh = 168,
            BrowserStop = 169,
            BrowserSearch = 170,
            BrowserFavorites = 171,
            BrowserHome = 172,
            VolumeMute = 173,
            VolumeDown = 174,
            VolumeUp = 175,
            MediaNextTrack = 176,
            MediaPreviousTrack = 177,
            MediaStop = 178,
            MediaPlayPause = 179,
            LaunchMail = 180,
            SelectMedia = 181,
            LaunchApplication1 = 182,
            LaunchApplication2 = 183,
            OEMSemicolon = 186,
            OEMPlus = 187,
            OEMComma = 188,
            OEMMinus = 189,
            OEMPeriod = 190,
            OEMQuestion = 191,
            OEMTilde = 192,
            OEMOpenBracket = 219,
            OEMPipe = 220,
            OEMCloseBracket = 221,
            OEMQuote = 222,
            OEMBackSlash = 226,
            Play = 250,
            Zoom = 251
        }

        //Virtual keys that prefer extended
        public readonly static KeysVirtual[] KeysVirtualExtended =
        {
            KeysVirtual.ArrowUp,
            KeysVirtual.ArrowDown,
            KeysVirtual.ArrowLeft,
            KeysVirtual.ArrowRight,
            KeysVirtual.Delete
        };
    }
}