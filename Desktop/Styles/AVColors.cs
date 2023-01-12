using System.Windows.Media;

namespace ArnoldVinkCode.Styles
{
    public partial class AVColors
    {
        //Adjust color brightness
        public static SolidColorBrush AdjustColorBrightness(SolidColorBrush solidColorBrush, double brightness)
        {
            try
            {
                Color adjustedColor = Color.FromRgb((byte)(solidColorBrush.Color.R * brightness), (byte)(solidColorBrush.Color.G * brightness), (byte)(solidColorBrush.Color.B * brightness));
                return new SolidColorBrush(adjustedColor);
            }
            catch { }
            return solidColorBrush;
        }

        //Adjust color opacity
        public static SolidColorBrush AdjustColorOpacity(SolidColorBrush solidColorBrush, double opacity)
        {
            try
            {
                Color adjustedColor = Color.FromArgb((byte)(solidColorBrush.Color.A * opacity), solidColorBrush.Color.R, solidColorBrush.Color.G, solidColorBrush.Color.B);
                return new SolidColorBrush(adjustedColor);
            }
            catch { }
            return solidColorBrush;
        }
    }
}