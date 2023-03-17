using System.Windows;
using System.Windows.Controls;

namespace ArnoldVinkCode
{
    public partial class AVFocus
    {
        public class AVFocusDetails
        {
            public int FocusIndex { get; set; } = 0;
            public ListBox FocusListBox { get; set; } = null;
            public FrameworkElement FocusElement { get; set; } = null;

            public void Reset()
            {
                FocusIndex = 0;
                FocusListBox = null;
                FocusElement = null;
            }
        }
    }
}