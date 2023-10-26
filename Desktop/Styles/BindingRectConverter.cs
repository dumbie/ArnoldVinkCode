using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ArnoldVinkCode.Styles
{
    //Import:
    //xmlns:styles="clr-namespace:ArnoldVinkCode.Styles"
    //Usage:
    //<MultiBinding>
    //    <MultiBinding.Converter>
    //        <styles:RectConverter/>
    //    </MultiBinding.Converter>
    //    <Binding Path="ActualWidth" RelativeSource="{RelativeSource TemplatedParent}"/>
    //    <Binding Path="ActualHeight" RelativeSource="{RelativeSource TemplatedParent}"/>
    //</MultiBinding>
    public class RectConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return new Rect(0, 0, (double)values[0], (double)values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}