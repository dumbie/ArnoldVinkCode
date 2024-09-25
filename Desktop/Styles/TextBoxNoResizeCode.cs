using System.Windows;
using System.Windows.Controls;

namespace ArnoldVinkCode.Styles
{
    //Import:
    //xmlns:AVStyles="clr-namespace:ArnoldVinkCode.Styles;assembly=ArnoldVinkCode"
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