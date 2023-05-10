namespace ArnoldVinkCode
{
    public partial class AVInputOutputClass
    {
        public enum MouseHidButtons : byte
        {
            None = 0,
            LeftButton = 1,
            RightButton = 2,
            MiddleButton = 4,
            XButton1 = 8,
            XButton2 = 10
        }

        public class MouseHidAction
        {
            public int MoveHorizontal { get; set; } = 0;
            public int MoveVertical { get; set; } = 0;
            public int ScrollHorizontal { get; set; } = 0;
            public int ScrollVertical { get; set; } = 0;
            public MouseHidButtons Button { get; set; } = MouseHidButtons.None;
        }
    }
}