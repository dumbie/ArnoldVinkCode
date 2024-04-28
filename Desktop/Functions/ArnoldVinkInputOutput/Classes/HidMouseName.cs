using System;

namespace ArnoldVinkCode
{
    public partial class AVInputOutputClass
    {
        public static string GetMouseButtonsName(MouseHidButtons mouseButton, bool shortName)
        {
            try
            {
                switch (mouseButton)
                {
                    case MouseHidButtons.LeftButton:
                        if (shortName) { return "Left"; } else { return "Left Button"; }
                    case MouseHidButtons.RightButton:
                        if (shortName) { return "Right"; } else { return "Right Button"; }
                    case MouseHidButtons.MiddleButton:
                        if (shortName) { return "Middle"; } else { return "Middle Button"; }
                    case MouseHidButtons.XButton1:
                        if (shortName) { return "Side1"; } else { return "Side Button 1"; }
                    case MouseHidButtons.XButton2:
                        if (shortName) { return "Side2"; } else { return "Side Button 2"; }
                }
            }
            catch { }
            return Enum.GetName(mouseButton);
        }
    }
}