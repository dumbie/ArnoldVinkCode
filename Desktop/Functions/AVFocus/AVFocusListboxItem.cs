using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;

namespace ArnoldVinkCode
{
    public partial class AVFocus
    {
        //Listbox focus or select an item
        public static async Task ListBoxFocusOrSelectItem(ListBox focusListBox, object selectItem, IntPtr windowHandle)
        {
            try
            {
                await AVActions.DispatcherInvoke(async delegate
                {
                    //Get the currently focused element
                    FrameworkElement frameworkElement = (FrameworkElement)Keyboard.FocusedElement;

                    //Check if focused element is disconnected
                    bool disconnectedSource = frameworkElement == null || frameworkElement.DataContext == BindingOperations.DisconnectedSource;

                    //Focus on the listbox or select index
                    if (disconnectedSource || frameworkElement == focusListBox)
                    {
                        await ListBoxFocusItem(focusListBox, selectItem, windowHandle);
                    }
                    else
                    {
                        ListBoxSelectItem(focusListBox, selectItem);
                    }
                });
            }
            catch { }
        }

        //Focus on listbox item
        public static async Task ListBoxFocusItem(ListBox focusListBox, object selectItem, IntPtr windowHandle)
        {
            try
            {
                await AVActions.DispatcherInvoke(async delegate
                {
                    //Check if listbox is null
                    if (focusListBox == null)
                    {
                        Debug.WriteLine("Listbox cannot be focused on, pressing tab key.");
                        KeySendSingle(KeysVirtual.Tab, windowHandle);
                        return;
                    }

                    //Check if listbox is disabled
                    if (!focusListBox.IsEnabled)
                    {
                        Debug.WriteLine("Listbox is disabled, pressing tab key.");
                        KeySendSingle(KeysVirtual.Tab, windowHandle);
                        return;
                    }

                    //Check if listbox is focusable
                    if (!focusListBox.Focusable)
                    {
                        Debug.WriteLine("Listbox cannot be focused on, pressing tab key.");
                        KeySendSingle(KeysVirtual.Tab, windowHandle);
                        return;
                    }

                    //Check if listbox is visible
                    if (focusListBox.Visibility != Visibility.Visible)
                    {
                        Debug.WriteLine("Listbox is not visible, pressing tab key.");
                        KeySendSingle(KeysVirtual.Tab, windowHandle);
                        return;
                    }

                    //Check if listbox has items
                    if (focusListBox.Items.Count == 0)
                    {
                        Debug.WriteLine("Listbox has no items, pressing tab key.");
                        KeySendSingle(KeysVirtual.Tab, windowHandle);
                        return;
                    }

                    //Update the listbox layout
                    focusListBox.UpdateLayout();

                    //Select a listbox item
                    ListBoxSelectItem(focusListBox, selectItem);

                    //Focus on the listbox and item
                    int selectedIndex = focusListBox.SelectedIndex;

                    //Scroll to the listbox item
                    object scrollListBoxItem = focusListBox.Items[selectedIndex];
                    focusListBox.ScrollIntoView(scrollListBoxItem);

                    //Force focus on element
                    ListBoxItem focusListBoxItem = (ListBoxItem)focusListBox.ItemContainerGenerator.ContainerFromInd‌​ex(selectedIndex);
                    await FocusElement(focusListBoxItem, windowHandle);

                    Debug.WriteLine("Focusing on listbox item: " + selectedIndex);
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed focusing on the listbox item, pressing tab key: " + ex.Message);
                KeySendSingle(KeysVirtual.Tab, windowHandle);
            }
        }

        //Select listbox item
        public static void ListBoxSelectItem(ListBox focusListBox, object selectItem)
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    //Update the listbox layout
                    focusListBox.UpdateLayout();

                    //Select the listbox item
                    if (selectItem != null)
                    {
                        try
                        {
                            focusListBox.SelectedItem = selectItem;
                            Debug.WriteLine("Selecting listbox item: " + selectItem);
                        }
                        catch { }
                    }
                    else
                    {
                        Debug.WriteLine("Select listbox item is null.");
                    }

                    //Check the selected index
                    if (focusListBox.SelectedIndex <= -1)
                    {
                        focusListBox.SelectedIndex = 0;
                        Debug.WriteLine("No selection, selecting first listbox index.");
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed selecting the listbox item: " + ex.Message);
            }
        }
    }
}