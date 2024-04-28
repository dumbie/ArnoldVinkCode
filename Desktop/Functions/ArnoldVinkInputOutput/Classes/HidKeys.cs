namespace ArnoldVinkCode
{
    public partial class AVInputOutputClass
    {
        public enum KeysMediaHid : byte
        {
            None = 0,
            Next = 1,
            Previous = 2,
            Stop = 4,
            PlayPause = 8,
            VolumeMute = 16,
            VolumeDown = 32,
            VolumeUp = 64
        }

        public enum KeysModifierHid : byte
        {
            None = 0,
            ControlLeft = 1,
            ShiftLeft = 2,
            AltLeft = 4,
            WindowsLeft = 8,
            ControlRight = 16,
            ShiftRight = 32,
            AltRight = 64,
            WindowsRight = 128
        }

        public enum KeysHid : byte
        {
            None = 0,
            A = 4,
            B = 5,
            C = 6,
            D = 7,
            E = 8,
            F = 9,
            G = 10,
            H = 11,
            I = 12,
            J = 13,
            K = 14,
            L = 15,
            M = 16,
            N = 17,
            O = 18,
            P = 19,
            Q = 20,
            R = 21,
            S = 22,
            T = 23,
            U = 24,
            V = 25,
            W = 26,
            X = 27,
            Y = 28,
            Z = 29,
            Digit1 = 30,
            Digit2 = 31,
            Digit3 = 32,
            Digit4 = 33,
            Digit5 = 34,
            Digit6 = 35,
            Digit7 = 36,
            Digit8 = 37,
            Digit9 = 38,
            Digit0 = 39,
            Enter = 40,
            Escape = 41,
            BackSpace = 42,
            Tab = 43,
            Space = 44,
            Minus = 45,
            Plus = 46,
            CloseBracket = 47,
            OpenBracket = 48,
            BackSlash = 49,
            Semicolon = 51,
            Quote = 52,
            Tilde = 53,
            Comma = 54,
            Period = 55,
            Question = 56,
            CapsLock = 57,
            F1 = 58,
            F2 = 59,
            F3 = 60,
            F4 = 61,
            F5 = 62,
            F6 = 63,
            F7 = 64,
            F8 = 65,
            F9 = 66,
            F10 = 67,
            F11 = 68,
            F12 = 69,
            PrintScreen = 70,
            ScrollLock = 71,
            Pause = 72,
            Insert = 73,
            Home = 74,
            PageUp = 75,
            Delete = 76,
            End = 77,
            PageDown = 78,
            ArrowRight = 79,
            ArrowLeft = 80,
            ArrowDown = 81,
            ArrowUp = 82,
            NumpadLock = 83,
            NumpadDivide = 84,
            NumpadMultiply = 85,
            NumpadSubtract = 86,
            NumpadAdd = 87,
            NumpadEnter = 88,
            Numpad1 = 89,
            Numpad2 = 90,
            Numpad3 = 91,
            Numpad4 = 92,
            Numpad5 = 93,
            Numpad6 = 94,
            Numpad7 = 95,
            Numpad8 = 96,
            Numpad9 = 97,
            Numpad0 = 98,
            NumpadDecimal = 99,
            ContextMenu = 101
        }

        public class KeysHidAction
        {
            public KeysModifierHid Modifiers { get; set; } = KeysModifierHid.None;
            public KeysHid Key0 { get; set; } = KeysHid.None;
            public KeysHid Key1 { get; set; } = KeysHid.None;
            public KeysHid Key2 { get; set; } = KeysHid.None;
            public KeysHid Key3 { get; set; } = KeysHid.None;
            public KeysHid Key4 { get; set; } = KeysHid.None;
            public KeysHid Key5 { get; set; } = KeysHid.None;
            public KeysHid Key6 { get; set; } = KeysHid.None;
            public KeysHid Key7 { get; set; } = KeysHid.None;
        }
    }
}