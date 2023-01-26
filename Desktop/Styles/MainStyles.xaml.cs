using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ArnoldVinkCode.Styles
{
    public partial class MainStyles : ResourceDictionary
    {
        //Handle horizontal scrollviewer scrolling
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            try
            {
                ScrollViewer scrollViewer = sender as ScrollViewer;
                if (e.Delta < 0) { scrollViewer.LineRight(); } else { scrollViewer.LineLeft(); }
            }
            catch { }
        }

        private void Slider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Slider slider = sender as Slider;
                slider.AutoToolTipPrecision = AVFunctions.DecimalGetLength(slider.TickFrequency, 2);
            }
            catch { }
        }
    }
}