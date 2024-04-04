using System;
using System.Diagnostics;
using System.Windows;

namespace ArnoldVinkCode.Styles
{
    public partial class AVResourceDictionary
    {
        //Load styles to resource dictionary
        public static bool LoadStyles()
        {
            try
            {
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add
                (
                    new ResourceDictionary { Source = new Uri("pack://application:,,,/ArnoldVinkCode;component/Styles/MainColors.xaml", UriKind.RelativeOrAbsolute) }
                );
                Application.Current.Resources.MergedDictionaries.Add
                (
                    new ResourceDictionary { Source = new Uri("pack://application:,,,/ArnoldVinkCode;component/Styles/ControlTemplates/Button.xaml", UriKind.RelativeOrAbsolute) }
                );
                Application.Current.Resources.MergedDictionaries.Add
                (
                    new ResourceDictionary { Source = new Uri("pack://application:,,,/ArnoldVinkCode;component/Styles/ControlTemplates/CheckBox.xaml", UriKind.RelativeOrAbsolute) }
                );
                Application.Current.Resources.MergedDictionaries.Add
                (
                    new ResourceDictionary { Source = new Uri("pack://application:,,,/ArnoldVinkCode;component/Styles/ControlTemplates/ComboBox.xaml", UriKind.RelativeOrAbsolute) }
                );
                Application.Current.Resources.MergedDictionaries.Add
                (
                    new ResourceDictionary { Source = new Uri("pack://application:,,,/ArnoldVinkCode;component/Styles/ControlTemplates/ListBox.xaml", UriKind.RelativeOrAbsolute) }
                );
                Application.Current.Resources.MergedDictionaries.Add
                (
                    new ResourceDictionary { Source = new Uri("pack://application:,,,/ArnoldVinkCode;component/Styles/ControlTemplates/ProgressBar.xaml", UriKind.RelativeOrAbsolute) }
                );
                Application.Current.Resources.MergedDictionaries.Add
                (
                    new ResourceDictionary { Source = new Uri("pack://application:,,,/ArnoldVinkCode;component/Styles/ControlTemplates/Scrollbar.xaml", UriKind.RelativeOrAbsolute) }
                );
                Application.Current.Resources.MergedDictionaries.Add
                (
                    new ResourceDictionary { Source = new Uri("pack://application:,,,/ArnoldVinkCode;component/Styles/ControlTemplates/Thumb.xaml", UriKind.RelativeOrAbsolute) }
                );
                Application.Current.Resources.MergedDictionaries.Add
                (
                    new ResourceDictionary { Source = new Uri("pack://application:,,,/ArnoldVinkCode;component/Styles/ControlTemplates/Slider.xaml", UriKind.RelativeOrAbsolute) }
                );
                Application.Current.Resources.MergedDictionaries.Add
                (
                    new ResourceDictionary { Source = new Uri("pack://application:,,,/ArnoldVinkCode;component/Styles/ControlTemplates/TextBox.xaml", UriKind.RelativeOrAbsolute) }
                );
                Application.Current.Resources.MergedDictionaries.Add
                (
                    new ResourceDictionary { Source = new Uri("pack://application:,,,/ArnoldVinkCode;component/Styles/MainStyles.xaml", UriKind.RelativeOrAbsolute) }
                );

                Debug.WriteLine("Loaded styles to resource dictionary: " + Application.Current.Resources.MergedDictionaries.Count);
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed loading styles to resource dictionary.");
                return false;
            }
        }
    }
}