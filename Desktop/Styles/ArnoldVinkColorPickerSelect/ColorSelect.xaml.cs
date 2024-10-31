using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ArnoldVinkCode.Styles
{
    public partial class ColorPickerSelect : Window
    {
        //Popup Variables
        private bool vPopupDone = false;
        private Color? vPopupResult = null;
        private Color? vCurrentColor = null;

        //Window initialize
        public ColorPickerSelect() { InitializeComponent(); }

        //Show and close popup
        public async Task<Color?> Popup(FrameworkElement disableElement)
        {
            try
            {
                //Disable the source frameworkelement
                if (disableElement != null)
                {
                    disableElement.IsEnabled = false;
                }

                //Reset popup variables
                vPopupResult = null;
                vPopupDone = false;

                //Show the popup
                Show();

                //Wait for user messagebox input
                while (vPopupResult == null && !vPopupDone && this.IsVisible) { await Task.Delay(500); }

                //Enable the source frameworkelement
                if (disableElement != null)
                {
                    disableElement.IsEnabled = true;
                }

                //Close the messagebox popup
                Close();
            }
            catch { }
            return vPopupResult;
        }

        void canvas_ColorSelect_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                double positionX = Mouse.GetPosition(canvas_ColorSelect).X;
                double positionY = Mouse.GetPosition(canvas_ColorSelect).Y;
                UpdateColorFromImage((int)positionX, (int)positionY);
                MoveEclipsePointer(positionX - 3, positionY - 3);
            }
            catch { }
        }

        void UpdateColorFromImage(int PositionX, int PositionY)
        {
            try
            {
                BitmapSource bitmapSource = (BitmapSource)image_ColorSelector.Source;
                TransformedBitmap transformedBitmap = new TransformedBitmap(bitmapSource, new ScaleTransform(image_ColorSelector.Width / bitmapSource.PixelWidth, image_ColorSelector.Height / bitmapSource.PixelHeight));
                CroppedBitmap croppedBitmap = new CroppedBitmap(transformedBitmap, new Int32Rect(PositionX, PositionY, 1, 1));
                byte[] colorBytes = new byte[4];
                croppedBitmap.CopyPixels(colorBytes, 4, 0);

                //Update current color
                vCurrentColor = Color.FromArgb((byte)slider_Opacity.Value, colorBytes[2], colorBytes[1], colorBytes[0]);
                UpdatePreview();
            }
            catch { }
        }

        void slider_Opacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                //Update current color
                Color currentColor = (Color)vCurrentColor;
                vCurrentColor = Color.FromArgb((byte)slider_Opacity.Value, currentColor.R, currentColor.G, currentColor.B);
                UpdatePreview();
            }
            catch { }
        }

        void MoveEclipsePointer(double PositionX, double PositionY)
        {
            try
            {
                eclipse_Pointer.SetValue(Canvas.LeftProperty, PositionX);
                eclipse_Pointer.SetValue(Canvas.TopProperty, PositionY);
            }
            catch { }
        }

        void UpdatePreview()
        {
            try
            {
                Color currentColor = (Color)vCurrentColor;
                button_ColorSelect.Background = new SolidColorBrush(currentColor);
                string alphaHex = currentColor.A.ToString("X").PadLeft(2, '0');
                string redHex = currentColor.R.ToString("X").PadLeft(2, '0');
                string greenHex = currentColor.G.ToString("X").PadLeft(2, '0');
                string blueHex = currentColor.B.ToString("X").PadLeft(2, '0');
                textbox_ColorHex.Text = string.Format("#{0}{1}{2}{3}", alphaHex, redHex, greenHex, blueHex);
                slider_Opacity.Value = currentColor.A;
            }
            catch { }
        }

        //Set Popup Results
        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vPopupResult = null;
                vPopupDone = true;
            }
            catch { }
        }
        private void button_ColorSelect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (vCurrentColor != null)
                {
                    vPopupResult = vCurrentColor;
                    vPopupDone = true;
                    Debug.WriteLine("Selected color: " + vPopupResult.ToString());
                }
            }
            catch { }
        }
    }
}