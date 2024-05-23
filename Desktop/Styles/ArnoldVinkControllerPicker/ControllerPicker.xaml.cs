using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static ArnoldVinkCode.AVInputOutputClass;

namespace ArnoldVinkCode.Styles
{
    public partial class ControllerPicker : UserControl
    {
        //Variables
        private bool ComboboxSaveEnabled = true;
        public event Action<ControllerButtons[]> TriggerChanged;

        //Window Initialize
        public ControllerPicker()
        {
            try
            {
                InitializeComponent();
            }
            catch { }
        }

        public void Set(ControllerButtons[] setList)
        {
            try
            {
                ComboboxSaveEnabled = false;

                //Set items source
                Array keysArray = Enum.GetValues(typeof(ControllerButtons));
                combobox_Hotkey0.ItemsSource = keysArray;
                combobox_Hotkey1.ItemsSource = keysArray;

                //Select items
                if (setList.Count() >= 1)
                {
                    combobox_Hotkey0.SelectedItem = setList[0];
                }
                else
                {
                    combobox_Hotkey0.SelectedItem = ControllerButtons.None;
                }
                if (setList.Count() >= 2)
                {
                    combobox_Hotkey1.SelectedItem = setList[1];
                }
                else
                {
                    combobox_Hotkey1.SelectedItem = ControllerButtons.None;
                }

                ComboboxSaveEnabled = true;
            }
            catch { }
        }

        public ControllerButtons[] Get()
        {
            try
            {
                return [(ControllerButtons)combobox_Hotkey0.SelectedItem, (ControllerButtons)combobox_Hotkey1.SelectedItem];
            }
            catch
            {
                return [ControllerButtons.None, ControllerButtons.None];
            }
        }

        private void combobox_Hotkey_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //Check if combobox saving is enabled
                if (!ComboboxSaveEnabled) { return; }

                //Signal changed event
                if (TriggerChanged != null)
                {
                    TriggerChanged(Get());
                }
            }
            catch { }
        }

        private void combobox_Hotkey_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ComboBox senderInterface = (ComboBox)sender;
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    senderInterface.SelectedItem = ControllerButtons.None;
                }
            }
            catch { }
        }

        private void button_Hotkey_Unmap_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Reset combobox selection
                ComboboxSaveEnabled = false;
                combobox_Hotkey0.SelectedItem = ControllerButtons.None;
                combobox_Hotkey1.SelectedItem = ControllerButtons.None;
                ComboboxSaveEnabled = true;

                //Signal changed event
                if (TriggerChanged != null)
                {
                    TriggerChanged(Get());
                }
            }
            catch { }
        }
    }
}