using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace ArnoldVinkCode.Styles
{
    public partial class MainColors : ResourceDictionary
    {
        //Change application accent color
        public static void ChangeApplicationAccentColor(string colorLightHex)
        {
            try
            {
                Debug.WriteLine("Changing the application accent color.");

                SolidColorBrush targetSolidColorBrushLight = new BrushConverter().ConvertFrom(colorLightHex) as SolidColorBrush;
                Application.Current.Resources["ApplicationAccentLightColor"] = targetSolidColorBrushLight.Color;
                Application.Current.Resources["ApplicationAccentLightBrush"] = targetSolidColorBrushLight;
                Debug.WriteLine("Accent light color: " + targetSolidColorBrushLight.Color);

                SolidColorBrush targetSolidColorBrushDim = AVColors.AdjustColorBrightness(targetSolidColorBrushLight, 0.80);
                Application.Current.Resources["ApplicationAccentDimColor"] = targetSolidColorBrushDim.Color;
                Application.Current.Resources["ApplicationAccentDimBrush"] = targetSolidColorBrushDim;
                Debug.WriteLine("Accent dim color: " + targetSolidColorBrushDim.Color);

                SolidColorBrush targetSolidColorBrushDark = AVColors.AdjustColorBrightness(targetSolidColorBrushLight, 0.50);
                Application.Current.Resources["ApplicationAccentDarkColor"] = targetSolidColorBrushDark.Color;
                Application.Current.Resources["ApplicationAccentDarkBrush"] = targetSolidColorBrushDark;
                Debug.WriteLine("Accent dark color: " + targetSolidColorBrushDark.Color);
            }
            catch { }
        }
    }
}