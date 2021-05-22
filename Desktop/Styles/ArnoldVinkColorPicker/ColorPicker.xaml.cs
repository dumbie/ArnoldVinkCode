using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace ArnoldVinkColorPicker
{
    public partial class ColorPicker : UserControl
    {
        public event Action<Color> SelectedColorChanged;

        public ColorPicker()
        {
            InitializeComponent();
            button_ColorPicker.ContextMenu.Closed += button_ColorPicker_ContextMenu_Closed;
            button_ColorPicker.PreviewMouseLeftButtonUp += button_ColorPicker_PreviewMouseLeftButtonUp;
        }

        void button_ColorPicker_ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!button_ColorPicker.ContextMenu.IsOpen && !ColorPicker_Selector.Cancelled)
                {
                    SelectedColorChanged(ColorPicker_Selector.CustomColor);
                    this.Background = new SolidColorBrush(ColorPicker_Selector.CustomColor);
                }
            }
            catch { }
        }

        void button_ColorPicker_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (button_ColorPicker.ContextMenu != null && !button_ColorPicker.ContextMenu.IsOpen)
                {
                    button_ColorPicker.ContextMenu.PlacementTarget = button_ColorPicker;
                    button_ColorPicker.ContextMenu.Placement = PlacementMode.Bottom;
                    button_ColorPicker.ContextMenu.IsOpen = true;
                    ColorPicker_Selector.Cancelled = true;
                }
            }
            catch { }
        }
    }
}