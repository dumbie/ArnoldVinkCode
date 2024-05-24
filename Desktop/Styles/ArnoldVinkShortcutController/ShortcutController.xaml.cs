using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static ArnoldVinkCode.AVClasses;
using static ArnoldVinkCode.AVInputOutputClass;

namespace ArnoldVinkCode.Styles
{
    public partial class ShortcutController : UserControl
    {
        //Variables
        private bool ComboboxSaveEnabled = true;
        public event Action<ShortcutTriggerController> TriggerChanged;
        public string TriggerName { get; set; } = string.Empty;

        //Window Initialize
        public ShortcutController()
        {
            try
            {
                InitializeComponent();
            }
            catch { }
        }

        public void Set(ShortcutTriggerController setList)
        {
            try
            {
                if (setList == null) { return; }
                ComboboxSaveEnabled = false;

                //Set items source
                Array keysArray = Enum.GetValues(typeof(ControllerButtons));
                combobox_Hotkey0.ItemsSource = keysArray;
                combobox_Hotkey1.ItemsSource = keysArray;

                //Set hold
                checkbox_Hold.IsChecked = setList.Hold;

                //Select items
                if (setList.Trigger.Count() >= 1)
                {
                    combobox_Hotkey0.SelectedItem = setList.Trigger[0];
                }
                else
                {
                    combobox_Hotkey0.SelectedItem = ControllerButtons.None;
                }
                if (setList.Trigger.Count() >= 2)
                {
                    combobox_Hotkey1.SelectedItem = setList.Trigger[1];
                }
                else
                {
                    combobox_Hotkey1.SelectedItem = ControllerButtons.None;
                }

                ComboboxSaveEnabled = true;
            }
            catch { }
        }

        public ShortcutTriggerController Get()
        {
            try
            {
                ShortcutTriggerController shortcutTrigger = new ShortcutTriggerController();
                shortcutTrigger.Name = TriggerName;
                shortcutTrigger.Trigger = [(ControllerButtons)combobox_Hotkey0.SelectedItem, (ControllerButtons)combobox_Hotkey1.SelectedItem];
                shortcutTrigger.Hold = (bool)checkbox_Hold.IsChecked;
                return shortcutTrigger;
            }
            catch
            {
                ShortcutTriggerController shortcutTrigger = new ShortcutTriggerController();
                shortcutTrigger.Name = TriggerName;
                shortcutTrigger.Trigger = [ControllerButtons.None, ControllerButtons.None];
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