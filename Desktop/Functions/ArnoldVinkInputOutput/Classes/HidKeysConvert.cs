namespace ArnoldVinkCode
{
    public partial class AVInputOutputClass
    {
        public static KeysModifierHid ConvertVirtualToKeysModifierHid(KeysModifierVirtual keyboardModifier)
        {
            KeysModifierHid pressedModifiers = KeysModifierHid.None;
            try
            {
                if (keyboardModifier.HasFlag(KeysModifierVirtual.Alt)) { pressedModifiers |= KeysModifierHid.AltLeft; }
                if (keyboardModifier.HasFlag(KeysModifierVirtual.Ctrl)) { pressedModifiers |= KeysModifierHid.CtrlLeft; }
                if (keyboardModifier.HasFlag(KeysModifierVirtual.Shift)) { pressedModifiers |= KeysModifierHid.ShiftLeft; }
                if (keyboardModifier.HasFlag(KeysModifierVirtual.Windows)) { pressedModifiers |= KeysModifierHid.WindowsLeft; }
            }
            catch { }
            return pressedModifiers;
        }

        public static KeysMediaHid ConvertVirtualToKeysMediaHid(KeysVirtual keyboardMedia)
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

        public static KeysHid ConvertVirtualToKeysHid(KeysVirtual keyboardKey)
        {
            try
            {
                switch (keyboardKey)
                {
                    case KeysVirtual.A:
                        return KeysHid.A;
                    case KeysVirtual.B:
                        return KeysHid.B;
                    case KeysVirtual.C:
                        return KeysHid.C;
                    case KeysVirtual.D:
                        return KeysHid.D;
                    case KeysVirtual.E:
                        return KeysHid.E;
                    case KeysVirtual.F:
                        return KeysHid.F;
                    case KeysVirtual.G:
                        return KeysHid.G;
                    case KeysVirtual.H:
                        return KeysHid.H;
                    case KeysVirtual.I:
                        return KeysHid.I;
                    case KeysVirtual.J:
                        return KeysHid.J;
                    case KeysVirtual.K:
                        return KeysHid.K;
                    case KeysVirtual.L:
                        return KeysHid.L;
                    case KeysVirtual.M:
                        return KeysHid.M;
                    case KeysVirtual.N:
                        return KeysHid.N;
                    case KeysVirtual.O:
                        return KeysHid.O;
                    case KeysVirtual.P:
                        return KeysHid.P;
                    case KeysVirtual.Q:
                        return KeysHid.Q;
                    case KeysVirtual.R:
                        return KeysHid.R;
                    case KeysVirtual.S:
                        return KeysHid.S;
                    case KeysVirtual.T:
                        return KeysHid.T;
                    case KeysVirtual.U:
                        return KeysHid.U;
                    case KeysVirtual.V:
                        return KeysHid.V;
                    case KeysVirtual.W:
                        return KeysHid.W;
                    case KeysVirtual.X:
                        return KeysHid.X;
                    case KeysVirtual.Y:
                        return KeysHid.Y;
                    case KeysVirtual.Z:
                        return KeysHid.Z;
                    case KeysVirtual.Digit1:
                        return KeysHid.Digit1;
                    case KeysVirtual.Digit2:
                        return KeysHid.Digit2;
                    case KeysVirtual.Digit3:
                        return KeysHid.Digit3;
                    case KeysVirtual.Digit4:
                        return KeysHid.Digit4;
                    case KeysVirtual.Digit5:
                        return KeysHid.Digit5;
                    case KeysVirtual.Digit6:
                        return KeysHid.Digit6;
                    case KeysVirtual.Digit7:
                        return KeysHid.Digit7;
                    case KeysVirtual.Digit8:
                        return KeysHid.Digit8;
                    case KeysVirtual.Digit9:
                        return KeysHid.Digit9;
                    case KeysVirtual.Digit0:
                        return KeysHid.Digit0;
                    case KeysVirtual.Enter:
                        return KeysHid.Enter;
                    case KeysVirtual.Escape:
                        return KeysHid.Escape;
                    case KeysVirtual.BackSpace:
                        return KeysHid.BackSpace;
                    case KeysVirtual.Tab:
                        return KeysHid.Tab;
                    case KeysVirtual.Space:
                        return KeysHid.Space;
                    case KeysVirtual.OEMMinus:
                        return KeysHid.Minus;
                    case KeysVirtual.OEMPlus:
                        return KeysHid.Plus;
                    case KeysVirtual.OEMCloseBracket:
                        return KeysHid.CloseBracket;
                    case KeysVirtual.OEMOpenBracket:
                        return KeysHid.OpenBracket;
                    case KeysVirtual.OEMBackSlash:
                        return KeysHid.BackSlash;
                    case KeysVirtual.OEMSemicolon:
                        return KeysHid.Semicolon;
                    case KeysVirtual.OEMQuote:
                        return KeysHid.Quote;
                    case KeysVirtual.OEMTilde:
                        return KeysHid.Tilde;
                    case KeysVirtual.OEMComma:
                        return KeysHid.Comma;
                    case KeysVirtual.OEMPeriod:
                        return KeysHid.Period;
                    case KeysVirtual.OEMQuestion:
                        return KeysHid.Question;
                    case KeysVirtual.CapsLock:
                        return KeysHid.CapsLock;
                    case KeysVirtual.F1:
                        return KeysHid.F1;
                    case KeysVirtual.F2:
                        return KeysHid.F2;
                    case KeysVirtual.F3:
                        return KeysHid.F3;
                    case KeysVirtual.F4:
                        return KeysHid.F4;
                    case KeysVirtual.F5:
                        return KeysHid.F5;
                    case KeysVirtual.F6:
                        return KeysHid.F6;
                    case KeysVirtual.F7:
                        return KeysHid.F7;
                    case KeysVirtual.F8:
                        return KeysHid.F8;
                    case KeysVirtual.F9:
                        return KeysHid.F9;
                    case KeysVirtual.F10:
                        return KeysHid.F10;
                    case KeysVirtual.F11:
                        return KeysHid.F11;
                    case KeysVirtual.F12:
                        return KeysHid.F12;
                    case KeysVirtual.PrintScreen:
                        return KeysHid.PrintScreen;
                    case KeysVirtual.ScrollLock:
                        return KeysHid.ScrollLock;
                    case KeysVirtual.Pause:
                        return KeysHid.Pause;
                    case KeysVirtual.Insert:
                        return KeysHid.Insert;
                    case KeysVirtual.Home:
                        return KeysHid.Home;
                    case KeysVirtual.PageUp:
                        return KeysHid.PageUp;
                    case KeysVirtual.Delete:
                        return KeysHid.Delete;
                    case KeysVirtual.End:
                        return KeysHid.End;
                    case KeysVirtual.PageDown:
                        return KeysHid.PageDown;
                    case KeysVirtual.ArrowRight:
                        return KeysHid.ArrowRight;
                    case KeysVirtual.ArrowLeft:
                        return KeysHid.ArrowLeft;
                    case KeysVirtual.ArrowDown:
                        return KeysHid.ArrowDown;
                    case KeysVirtual.ArrowUp:
                        return KeysHid.ArrowUp;
                    case KeysVirtual.NumpadLock:
                        return KeysHid.NumpadLock;
                    case KeysVirtual.NumpadDivide:
                        return KeysHid.NumpadDivide;
                    case KeysVirtual.NumpadMultiply:
                        return KeysHid.NumpadMultiply;
                    case KeysVirtual.NumpadSubtract:
                        return KeysHid.NumpadSubtract;
                    case KeysVirtual.NumpadAdd:
                        return KeysHid.NumpadAdd;
                    case KeysVirtual.Numpad1:
                        return KeysHid.Numpad1;
                    case KeysVirtual.Numpad2:
                        return KeysHid.Numpad2;
                    case KeysVirtual.Numpad3:
                        return KeysHid.Numpad3;
                    case KeysVirtual.Numpad4:
                        return KeysHid.Numpad4;
                    case KeysVirtual.Numpad5:
                        return KeysHid.Numpad5;
                    case KeysVirtual.Numpad6:
                        return KeysHid.Numpad6;
                    case KeysVirtual.Numpad7:
                        return KeysHid.Numpad7;
                    case KeysVirtual.Numpad8:
                        return KeysHid.Numpad8;
                    case KeysVirtual.Numpad9:
                        return KeysHid.Numpad9;
                    case KeysVirtual.Numpad0:
                        return KeysHid.Numpad0;
                    case KeysVirtual.NumpadDecimal:
                        return KeysHid.NumpadDecimal;
                    case KeysVirtual.ContextMenu:
                        return KeysHid.ContextMenu;
                }
            }
            catch { }
            return KeysHid.None;
        }
    }
}