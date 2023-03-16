using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace ArnoldVinkCode.Styles
{
    //Import:
    //xmlns:styles="clr-namespace:ArnoldVinkCode.Styles"
    //Usage:
    //Visibility="{Binding Name, Converter={styles:CheckVisibility}}"/>

    //Check if binding is visible or collapsed
    public class CheckVisibility : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null)
                {
                    return Visibility.Collapsed;
                }

                Type valueType = value.GetType();
                if (valueType == typeof(string))
                {
                    return string.IsNullOrWhiteSpace((string)value) ? Visibility.Collapsed : Visibility.Visible;
                }

                if (valueType == typeof(bool))
                {
                    return (bool)value ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            catch { }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}