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
        //Listbox focus or select an index
        public static async Task ListBoxFocusOrSelectIndex(ListBox focusListBox, bool lastIndex, int indexNumber, FrameworkElement windowElement, IntPtr windowHandle)
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
                        await ListBoxFocusIndex(focusListBox, lastIndex, indexNumber, windowElement, windowHandle);
                    }
                    else
                    {
                        ListBoxSelectIndex(focusListBox, lastIndex, indexNumber);
                    }
                });
            }
            catch { }
        }

        //Focus on listbox index
        public static async Task ListBoxFocusIndex(ListBox focusListBox, bool lastIndex, int indexNumber, FrameworkElement windowElement, IntPtr windowHandle)
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

                    //Select a listbox index
                    ListBoxSelectIndex(focusListBox, lastIndex, indexNumber);

                    //Focus on the listbox and item
                    int selectedIndex = focusListBox.SelectedIndex;

                    //Scroll to the listbox item
                    object scrollListBoxItem = focusListBox.Items[selectedIndex];
                    focusListBox.ScrollIntoView(scrollListBoxItem);

                    //Force focus on element
                    ListBoxItem focusListBoxItem = (ListBoxItem)focusListBox.ItemContainerGenerator.ContainerFromInd‌​ex(selectedIndex);
                    await FocusElement(focusListBoxItem, windowElement, windowHandle);

                    Debug.WriteLine("Focusing on listbox index: " + selectedIndex);
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed focusing on the listbox index, pressing tab key: " + ex.Message);
                KeySendSingle(KeysVirtual.Tab, windowHandle);
            }
        }

        //Select listbox index
        public static void ListBoxSelectIndex(ListBox focusListBox, bool lastIndex, int indexNumber)
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    //Update the listbox layout
                    focusListBox.UpdateLayout();

                    //Get listbox item count
                    int itemsCount = focusListBox.Items.Count;

                    //Select the listbox index
                    if (lastIndex)
                    {
                        focusListBox.SelectedIndex = itemsCount - 1;
                        Debug.WriteLine("Selecting last listbox index.");
                    }
                    else if (indexNumber > -1)
                    {
                        if (indexNumber >= itemsCount)
                        {
                            focusListBox.SelectedIndex = itemsCount - 1;
                            Debug.WriteLine("Selecting last listbox index.");
                        }
                        else
                        {
                            focusListBox.SelectedIndex = indexNumber;
                            Debug.WriteLine("Selecting listbox index: " + indexNumber);
                        }
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
                Debug.WriteLine("Failed selecting the listbox index: " + ex.Message);
            }
        }
    }
}