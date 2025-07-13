using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ArnoldVinkStyles
{
    //Import:
    //xmlns:AVStyles="clr-namespace:ArnoldVinkStyles"
    //Usage:
    //<Image AVStyles:SourceCustom.Source="Assets/Image.png"/>
    public class SourceCustom
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached("Source", typeof(string), typeof(SourceCustom), new PropertyMetadata(default(string), SourceChanged));
        private static void SourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                Uri imageUri = new Uri(e.NewValue.ToString(), UriKind.RelativeOrAbsolute);

                BitmapImage imageBitmap = new BitmapImage();
                imageBitmap.BeginInit();
                imageBitmap.CreateOptions = BitmapCreateOptions.None;
                //imageBitmap.CacheOption = BitmapCacheOption.OnLoad; //Notworkinginpreview
                imageBitmap.UriSource = imageUri;
                imageBitmap.EndInit();
                //imageBitmap.Freeze(); //Notworkinginpreview

                Image imageCustom = sender as Image;
                imageCustom.Source = imageBitmap;
            }
            catch { }
        }

        [AttachedPropertyBrowsableForType(typeof(Image))]
        public static void SetSource(DependencyObject element, string value)
        {
            element.SetValue(SourceProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(Image))]
        public static string GetSource(DependencyObject element)
        {
            return (string)element.GetValue(SourceProperty);
        }
    }
}