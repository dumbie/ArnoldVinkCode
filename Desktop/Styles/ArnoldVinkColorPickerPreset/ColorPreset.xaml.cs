using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ArnoldVinkCode.Styles
{
    public partial class ColorPickerPreset : Window
    {
        //Popup Variables
        private bool vPopupDone = false;
        private Color? vPopupResult = null;

        //Available color list
        public uint[] uintColors =
        {
            //Grayscale
            0xFF222222,0xFFFFFFFF,0xFFEEEEEE,0xFFD3D3D3,0xFFA9A9A9,

            //Colors
            0xFFFFFF00,0xFFFFE135,0xFFF3DB32,0xFFF8DE7E,0xFFADFF2F,0xFFB2CF5D,0xFF9CB651,0xFFC1D881,0xFF10C96F,
            0xFF00FF00,0xFF7FFF00,0xFF32CD32,0xFF00FF7F,0xFF90EE90,0xFF42B54F,
            0xFF3CB371,0xFF2E8B57,0xFF008000,0xFF548F3B,0xFF0B8645,0xFF808000,0xFFFF0000,
            0xFFED2939,0xFF800000,0xFFA52A2A,0xFFB22222,0xFFDC143C,
            0xFFFF8C00,0xFFFFA500,0xFFEDBA47,0xFFD2691E,0xFFFF7F50,0xFFF4A460,
            0xFFD6A572,0xFFC9915F,0xFF744030,0xFF8C4A2E,0xFF572316,0xFFB2373F,0xFF9B3F53,
            0xFFCD5C5C,0xFFF08080,0xFFFFB6C1,0xFFFFA07A,0xFFFF1493,
            0xFFFF69B4,0xFFFF00FF,0xFFC71585,0xFF800080,0xFF4B0082,
            0xFF8A2BE2,0xFFDA70D6,0xFFDB7093,0xFF76608A,0xFF483D8B,
            0xFF000080,0xFF0000FF,0xFF6495ED,0xFF00C7FF,0xFF00BFFF,
            0xFF1E90FF,0xFFADD8E6,0xFF87CEFA,0xFF006FD5,0xFF7B68EE,
            0xFF6A5ACD,0xFF4169E1,0xFF708090,0xFF4682B4,
            0xFF008080,0xFF217283,0xFF40E0D0,0xFF20B2AA,0xFFA0B3C0,0xFF15B0FC
        };

        //Window initialize
        public ColorPickerPreset() { InitializeComponent(); }

        //Window initialized
        protected override void OnSourceInitialized(EventArgs e)
        {
            try
            {
                //Add colors to the list
                foreach (uint uintColor in uintColors)
                {
                    SolidColorBrush newSolidColorBrush = new SolidColorBrush(Color.FromArgb((byte)(uintColor >> 24), (byte)(uintColor >> 16), (byte)(uintColor >> 8), (byte)(uintColor)));
                    listbox_ColorPicker.Items.Add(newSolidColorBrush);
                }
            }
            catch { }
        }

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

        //Move window
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
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
        private void listbox_ColorPicker_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    listbox_ColorPicker_PreviewMouseUp(null, null);
                }
            }
            catch { }
        }
        private void listbox_ColorPicker_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (listbox_ColorPicker.SelectedItems.Count > 0 && listbox_ColorPicker.SelectedIndex != -1)
                {
                    SolidColorBrush selectedSolidColorBrush = (SolidColorBrush)listbox_ColorPicker.SelectedItem;
                    vPopupResult = selectedSolidColorBrush.Color;
                    vPopupDone = true;
                    Debug.WriteLine("Selected color: " + vPopupResult.ToString());
                }
            }
            catch { }
        }
    }
}