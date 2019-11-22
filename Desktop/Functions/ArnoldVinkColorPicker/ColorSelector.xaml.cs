using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ArnoldVinkColorPicker
{
    public partial class ColorSelector : UserControl
    {
        public bool Cancelled = true;

        private Color _customColor = Colors.Black;
        public Color CustomColor
        {
            get { return _customColor; }
            set
            {
                if (_customColor != value)
                {
                    _customColor = value;
                    UpdatePreview();
                }
            }
        }

        public ColorSelector()
        {
            InitializeComponent();
            slider_Opacity.ValueChanged += slider_Opacity_ValueChanged;
            canvas_ColorSelect.PreviewMouseLeftButtonUp += canvas_ColorSelect_PreviewMouseLeftButtonUp;
            grid_ColorSelected.PreviewMouseLeftButtonUp += grid_ColorSelected_PreviewMouseLeftButtonUp;
        }

        void slider_Opacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                CustomColor = Color.FromArgb((byte)slider_Opacity.Value, CustomColor.R, CustomColor.G, CustomColor.B);
                e.Handled = true;
            }
            catch { }
        }

        void grid_ColorSelected_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Cancelled = false;
            }
            catch { }
        }

        void canvas_ColorSelect_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                double PositionX = Mouse.GetPosition(canvas_ColorSelect).X - 3;
                double PositionY = Mouse.GetPosition(canvas_ColorSelect).Y - 3;
                UpdateColorFromImage((int)PositionX, (int)PositionY);
                MoveEclipsePointer(PositionX, PositionY);
                e.Handled = true;
            }
            catch { }
        }

        void UpdateColorFromImage(int PositionX, int PositionY)
        {
            try
            {
                CroppedBitmap croppedBitmap = new CroppedBitmap(image_ColorSelector.Source as BitmapSource, new Int32Rect(PositionX, PositionY, 1, 1));
                byte[] colorBytes = new byte[4];
                croppedBitmap.CopyPixels(colorBytes, 4, 0);
                CustomColor = Color.FromArgb((byte)slider_Opacity.Value, colorBytes[2], colorBytes[1], colorBytes[0]);
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
                rectangle_ColorSelected.Fill = new SolidColorBrush(CustomColor);
                string alphaHex = CustomColor.A.ToString("X").PadLeft(2, '0');
                string redHex = CustomColor.R.ToString("X").PadLeft(2, '0');
                string greenHex = CustomColor.G.ToString("X").PadLeft(2, '0');
                string blueHex = CustomColor.B.ToString("X").PadLeft(2, '0');
                textbox_ColorHex.Text = string.Format("#{0}{1}{2}{3}", alphaHex, redHex, greenHex, blueHex);
                slider_Opacity.Value = CustomColor.A;
            }
            catch { }
        }
    }
}