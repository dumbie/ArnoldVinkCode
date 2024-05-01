using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVSettings;

namespace ArnoldVinkHotkeyPicker
{
    public partial class HotkeyPicker : UserControl
    {
        //Variables
        private bool ComboboxSaveEnabled = true;
        public string HotkeyName { get; set; } = string.Empty;
        public Configuration Configuration { get; set; } = null;
        public bool MonitorKeyboardInput { get; set; } = true;

        //Window Initialize
        public HotkeyPicker()
        {
            try
            {
                InitializeComponent();
                this.Loaded += OnLoaded;
            }
            catch { }
        }

        //Window Initialized
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ComboboxSaveEnabled = false;
                Load_Hotkey_Selection();
                Load_Hotkey_Settings();
                ComboboxSaveEnabled = true;
            }
            catch { }
        }

        private void Load_Hotkey_Selection()
        {
            try
            {
                Array keysVirtualArray = Enum.GetValues(typeof(KeysVirtual));
                //Fix read and set key names using GetVirtualKeyName

                combobox_Hotkey0.ItemsSource = keysVirtualArray;
                combobox_Hotkey0.SelectedItem = KeysVirtual.None;
                combobox_Hotkey1.ItemsSource = keysVirtualArray;
                combobox_Hotkey1.SelectedItem = KeysVirtual.None;
                combobox_Hotkey2.ItemsSource = keysVirtualArray;
                combobox_Hotkey2.SelectedItem = KeysVirtual.None;
            }
            catch { }
        }

        private void Load_Hotkey_Settings()
        {
            try
            {
                KeysVirtual usedKey0 = (KeysVirtual)SettingLoad(Configuration, "Hotkey0" + HotkeyName, typeof(byte));
                KeysVirtual usedKey1 = (KeysVirtual)SettingLoad(Configuration, "Hotkey1" + HotkeyName, typeof(byte));
                KeysVirtual usedKey2 = (KeysVirtual)SettingLoad(Configuration, "Hotkey2" + HotkeyName, typeof(byte));

                combobox_Hotkey0.SelectedItem = usedKey0;
                combobox_Hotkey1.SelectedItem = usedKey1;
                combobox_Hotkey2.SelectedItem = usedKey2;

                Debug.WriteLine("Loaded hotkey settings: " + HotkeyName);
            }
            catch { }
        }

        private void combobox_Hotkey_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //Check if combobox saving is enabled
                if (!ComboboxSaveEnabled) { return; }

                //Get selected keys
                KeysVirtual usedKey0 = (KeysVirtual)combobox_Hotkey0.SelectedItem;
                KeysVirtual usedKey1 = (KeysVirtual)combobox_Hotkey1.SelectedItem;
                KeysVirtual usedKey2 = (KeysVirtual)combobox_Hotkey2.SelectedItem;

                //Save hotkey keys
                SettingSave(Configuration, "Hotkey0" + HotkeyName, (byte)usedKey0);
                SettingSave(Configuration, "Hotkey1" + HotkeyName, (byte)usedKey1);
                SettingSave(Configuration, "Hotkey2" + HotkeyName, (byte)usedKey2);

                Debug.WriteLine("Saved keyboard hotkey: " + HotkeyName + " / " + usedKey0 + " / " + usedKey1 + " / " + usedKey2);
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

                //Save hotkey keys
                SettingSave(Configuration, "Hotkey0" + HotkeyName, (byte)KeysVirtual.None);
                SettingSave(Configuration, "Hotkey1" + HotkeyName, (byte)KeysVirtual.None);
                SettingSave(Configuration, "Hotkey2" + HotkeyName, (byte)KeysVirtual.None);

                Debug.WriteLine("Unmapped keyboard hotkeys: " + HotkeyName);
            }
            catch { }
        }
    }
}