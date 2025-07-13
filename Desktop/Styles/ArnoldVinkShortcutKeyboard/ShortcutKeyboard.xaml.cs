using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static ArnoldVinkCode.AVClasses;
using static ArnoldVinkCode.AVInputOutputClass;

namespace ArnoldVinkStyles
{
    public partial class ShortcutKeyboard : UserControl
    {
        //Variables
        private bool ComboboxSaveEnabled = true;
        public Action<ShortcutTriggerKeyboard> TriggerChanged;
        public string TriggerName { get; set; } = string.Empty;
        public bool MonitorKeyboardInput { get; set; } = true;

        //Window Initialize
        public ShortcutKeyboard()
        {
            try
            {
                InitializeComponent();
            }
            catch { }
        }

        public void Set(ShortcutTriggerKeyboard setList)
        {
            try
            {
                if (setList == null) { return; }
                ComboboxSaveEnabled = false;

                //Set items source
                Array keysArray = Enum.GetValues(typeof(KeysVirtual));
                combobox_Hotkey0.ItemsSource = keysArray;
                combobox_Hotkey1.ItemsSource = keysArray;
                combobox_Hotkey2.ItemsSource = keysArray;

                //Select items
                if (setList.Trigger.Count() >= 1)
                {
                    combobox_Hotkey0.SelectedItem = setList.Trigger[0];
                }
                else
                {
                    combobox_Hotkey0.SelectedItem = KeysVirtual.None;
                }
                if (setList.Trigger.Count() >= 2)
                {
                    combobox_Hotkey1.SelectedItem = setList.Trigger[1];
                }
                else
                {
                    combobox_Hotkey1.SelectedItem = KeysVirtual.None;
                }
                if (setList.Trigger.Count() >= 3)
                {
                    combobox_Hotkey2.SelectedItem = setList.Trigger[2];
                }
                else
                {
                    combobox_Hotkey2.SelectedItem = KeysVirtual.None;
                }

                ComboboxSaveEnabled = true;
            }
            catch { }
        }

        public ShortcutTriggerKeyboard Get()
        {
            try
            {
                ShortcutTriggerKeyboard shortcutTrigger = new ShortcutTriggerKeyboard();
                shortcutTrigger.Name = TriggerName;
                shortcutTrigger.Trigger = [(KeysVirtual)combobox_Hotkey0.SelectedItem, (KeysVirtual)combobox_Hotkey1.SelectedItem, (KeysVirtual)combobox_Hotkey2.SelectedItem];
                return shortcutTrigger;
            }
            catch
            {
                ShortcutTriggerKeyboard shortcutTrigger = new ShortcutTriggerKeyboard();
                shortcutTrigger.Name = TriggerName;
                shortcutTrigger.Trigger = [KeysVirtual.None, KeysVirtual.None];
                return shortcutTrigger;
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