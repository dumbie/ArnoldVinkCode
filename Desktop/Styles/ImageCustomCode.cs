using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ArnoldVinkCode.Styles
{
    //Import:
    //xmlns:styles="clr-namespace:ArnoldVinkCode.Styles"
    //Usage:
    //<Image Source="Assets/Image.png"/>
    public class ImageCustom : Image
    {
        /// <summary>
        /// Defines the custom uri source
        /// </summary>
        public Uri SourceCustom
        {
            get { return new Uri(GetValue(SourceCustomProperty).ToString(), UriKind.RelativeOrAbsolute); }
            set { SetValue(SourceCustomProperty, value); }
        }
        public static readonly DependencyProperty SourceCustomProperty = DependencyProperty.Register("SourceCustom", typeof(Uri), typeof(ImageCustom), new FrameworkPropertyMetadata(null, SourceCustomPropertyChanged));
        private static void SourceCustomPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CreateOptions = BitmapCreateOptions.None;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.UriSource = new Uri(e.NewValue.ToString(), UriKind.RelativeOrAbsolute);
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                ImageCustom imageCustom = sender as ImageCustom;
                imageCustom.Source = bitmapImage;
            }
            catch { }
        }
    }
}