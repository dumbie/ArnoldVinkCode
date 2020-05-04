using System.Windows;
using System.Windows.Controls;

namespace ArnoldVinkCode.Styles
{
    //Import:
    //xmlns:styles="clr-namespace:ArnoldVinkCode.Styles"
    //Usage:
    //<Textbox Text="Hello" styles:TextboxPlaceholder.Placeholder="Hello"/>
    public class TextboxPlaceholder
    {
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.RegisterAttached("Placeholder", typeof(string), typeof(TextboxPlaceholder), new PropertyMetadata(default(string), PlaceholderChanged));
        private static void PlaceholderChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                TextBox targetTextbox = sender as TextBox;

                targetTextbox.LostFocus -= OnLostFocus;
                targetTextbox.GotFocus -= OnGotFocus;

                if (e.NewValue != null)
                {
                    targetTextbox.GotFocus += OnGotFocus;
                    targetTextbox.LostFocus += OnLostFocus;
                }
            }
            catch { }
        }

        private static void OnLostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBox targetTextbox = sender as TextBox;
                if (string.IsNullOrWhiteSpace(targetTextbox.Text))
                {
                    targetTextbox.Text = GetPlaceholder(targetTextbox);
                }
            }
            catch { }
        }

        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBox targetTextbox = sender as TextBox;
                string placeholderText = GetPlaceholder(targetTextbox);
                if (targetTextbox.Text == placeholderText)
                {
                    targetTextbox.Text = string.Empty;
                }
            }
            catch { }
        }

        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static void SetPlaceholder(DependencyObject element, string value)
        {
            element.SetValue(PlaceholderProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static string GetPlaceholder(DependencyObject element)
        {
            return (string)element.GetValue(PlaceholderProperty);
        }
    }
}