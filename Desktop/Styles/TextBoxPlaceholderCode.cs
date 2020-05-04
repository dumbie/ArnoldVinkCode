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
        private static void PlaceholderChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            TextBox targetTextbox = dependencyObject as TextBox;
            if (targetTextbox == null) { return; }

            targetTextbox.LostFocus -= OnLostFocus;
            targetTextbox.GotFocus -= OnGotFocus;

            if (args.NewValue != null)
            {
                targetTextbox.GotFocus += OnGotFocus;
                targetTextbox.LostFocus += OnLostFocus;
            }
        }

        private static void OnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            TextBox targetTextbox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(targetTextbox.Text))
            {
                targetTextbox.Text = GetPlaceholder(targetTextbox);
            }
        }

        private static void OnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            TextBox targetTextbox = sender as TextBox;
            string placeholderText = GetPlaceholder(targetTextbox);
            if (targetTextbox.Text == placeholderText)
            {
                targetTextbox.Text = string.Empty;
            }
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