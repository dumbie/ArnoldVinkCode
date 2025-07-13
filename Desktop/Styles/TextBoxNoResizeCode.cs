using System.Windows;
using System.Windows.Controls;

namespace ArnoldVinkStyles
{
    //Import:
    //xmlns:AVStyles="clr-namespace:ArnoldVinkStyles;assembly=ArnoldVinkCode"
    //Usage:
    //<AVStyles:TextBoxNoResize/>

    public class TextBoxNoResize : TextBox
    {
        protected override Size MeasureOverride(Size constraint)
        {
            return Size.Empty;
        }
    }
}