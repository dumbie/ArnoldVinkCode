using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static ArnoldVinkCode.AVInputOutputClass;

namespace ArnoldVinkCode.Styles
{
    public partial class KeyboardPicker : UserControl
    {
        //Variables
        private bool ComboboxSaveEnabled = true;
        public event Action<KeysVirtual[]> TriggerChanged;
        public bool MonitorKeyboardInput { get; set; } = true;

        //Window Initialize
        public KeyboardPicker()
        {
            try
            {
                InitializeComponent();
            }
            catch { }
        }

        public void Set(KeysVirtual[] setList)
        {
            try
            {
                ComboboxSaveEnabled = false;

                //Set items source
                Array keysArray = Enum.GetValues(typeof(KeysVirtual));
                combobox_Hotkey0.ItemsSource = keysArray;
                combobox_Hotkey1.ItemsSource = keysArray;
                combobox_Hotkey2.ItemsSource = keysArray;

                //Select items
                if (setList.Count() >= 1)
                {
                    combobox_Hotkey0.SelectedItem = setList[0];
                }
                else
                {
                    combobox_Hotkey0.SelectedItem = KeysVirtual.None;
                }
                if (setList.Count() >= 2)
                {
                    combobox_Hotkey1.SelectedItem = setList[1];
                }
                else
                {
                    combobox_Hotkey1.SelectedItem = KeysVirtual.None;
                }
                if (setList.Count() >= 3)
                {
                    combobox_Hotkey2.SelectedItem = setList[2];
                }
                else
                {
                    combobox_Hotkey2.SelectedItem = KeysVirtual.None;
                }

                ComboboxSaveEnabled = true;
            }
            catch { }
        }

        public KeysVirtual[] Get()
        {
            try
            {
                return [(KeysVirtual)combobox_Hotkey0.SelectedItem, (KeysVirtual)combobox_Hotkey1.SelectedItem, (KeysVirtual)combobox_Hotkey2.SelectedItem];
            }
            catch
            {
                return [KeysVirtual.None, KeysVirtual.None, KeysVirtual.None];
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

        private void combobox_Hotkey_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (MonitorKeyboardInput)
                {
                    ComboBox senderInterface = (ComboBox)sender;
                    senderInterface.SelectedItem = ConvertInputToVirtual((KeysInput)e.Key, (KeysInput)e.SystemKey);
                    e.Handled = true;
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
                    senderInterface.SelectedItem = KeysVirtual.None;
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
                combobox_Hotkey0.SelectedItem = KeysVirtual.None;
                combobox_Hotkey1.SelectedItem = KeysVirtual.None;
                combobox_Hotkey2.SelectedItem = KeysVirtual.None;
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