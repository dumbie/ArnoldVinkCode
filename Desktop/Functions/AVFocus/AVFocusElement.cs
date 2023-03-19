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
        //Check if keyboard has focus
        public static void FocusCheckKeyboard(FrameworkElement windowElement, IntPtr windowHandle)
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    //Get focused element
                    FrameworkElement focusedElement = (FrameworkElement)Keyboard.FocusedElement;
                    Debug.WriteLine("Keyboard check focused on: " + focusedElement);

                    //Check currently focused element
                    if (focusedElement == null)
                    {
                        Debug.WriteLine("Keyboard is not focused, pressing tab key.");
                        KeySendSingle(KeysVirtual.Tab, windowHandle);
                        return;
                    }

                    //Check if focused on other window
                    if (focusedElement.GetType().BaseType == typeof(Window) && windowElement != focusedElement)
                    {
                        Debug.WriteLine("Keyboard is focused on other window, focusing this window.");

                        //Focus on this window
                        FocusWindow(windowElement);

                        //Update focused element
                        focusedElement = (FrameworkElement)Keyboard.FocusedElement;
                        Debug.WriteLine("Keyboard check focused on: " + focusedElement);
                    }

                    //Check if focused on window
                    if (focusedElement.GetType().BaseType == typeof(Window))
                    {
                        Debug.WriteLine("Keyboard is focused on a window, pressing tab key.");
                        KeySendSingle(KeysVirtual.Tab, windowHandle);
                        return;
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed checking keyboard focus: " + ex.Message);
            }
        }

        //Focus framework element
        public static async Task FocusElement(FrameworkElement focusElement, IntPtr windowHandle)
        {
            try
            {
                await AVActions.DispatcherInvoke(async delegate
                {
                    //Check if focus element is null
                    if (focusElement == null)
                    {
                        Debug.WriteLine("Focus element is null, pressing tab key.");
                        KeySendSingle(KeysVirtual.Tab, windowHandle);
                        return;
                    }

                    //Check if focus element is disabled
                    if (!focusElement.IsEnabled)
                    {
                        Debug.WriteLine("Focus element is disabled, pressing tab key.");
                        KeySendSingle(KeysVirtual.Tab, windowHandle);
                        return;
                    }

                    //Check if focus element is focusable
                    if (!focusElement.Focusable)
                    {
                        Debug.WriteLine("Focus element cannot be focused on, pressing tab key.");
                        KeySendSingle(KeysVirtual.Tab, windowHandle);
                        return;
                    }

                    //Check if focus element is visible
                    if (focusElement.Visibility != Visibility.Visible)
                    {
                        Debug.WriteLine("Focus element is not visible, pressing tab key.");
                        KeySendSingle(KeysVirtual.Tab, windowHandle);
                        return;
                    }

                    //Check if focus element is disconnected
                    if (focusElement.DataContext == BindingOperations.DisconnectedSource)
                    {
                        Debug.WriteLine("Focus element is disconnected, pressing tab key.");
                        KeySendSingle(KeysVirtual.Tab, windowHandle);
                        return;
                    }

                    //Focus on framework element
                    //Update element layout
                    focusElement.UpdateLayout();

                    //Logical focus on element
                    focusElement.Focus();

                    //Keyboard focus on element
                    Keyboard.Focus(focusElement);

                    //Wait for element focus
                    await Task.Delay(10);

                    //Check focused element
                    FrameworkElement focusedElement = (FrameworkElement)Keyboard.FocusedElement;
                    if (focusedElement == null || focusedElement != focusElement)
                    {
                        Debug.WriteLine("Not focused on target element, pressing tab key: " + focusElement);
                        KeySendSingle(KeysVirtual.Tab, windowHandle);
                        return;
                    }

                    //Debug.WriteLine("Forced keyboard focus on: " + focusElement);
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed focusing on element, pressing tab key: " + ex.Message);
                KeySendSingle(KeysVirtual.Tab, windowHandle);
            }
        }

        //Focus AVFocusDetails
        public static async Task AVFocusDetailsFocus(AVFocusDetails focusElement, IntPtr windowHandle)
        {
            try
            {
                await AVActions.DispatcherInvoke(async delegate
                {
                    //Focus on AVFocusDetails
                    if (focusElement.FocusElement != null)
                    {
                        Debug.WriteLine("Focusing on previous element: " + focusElement.FocusElement);
                        await FocusElement(focusElement.FocusElement, windowHandle);
                    }
                    else if (focusElement.FocusListBox != null)
                    {
                        Debug.WriteLine("Focusing on previous listbox: " + focusElement.FocusListBox);
                        await ListBoxFocusIndex(focusElement.FocusListBox, false, focusElement.FocusIndex, windowHandle);
                    }
                    else
                    {
                        Debug.WriteLine("No previous focus element, pressing tab key.");
                        KeySendSingle(KeysVirtual.Tab, windowHandle);
                    }

                    //Reset AVFocusDetails
                    focusElement.Reset();
                });
            }
            catch { }
        }

        //Save AVFocusDetails
        public static void AVFocusDetailsSave(AVFocusDetails saveElement, FrameworkElement focusedElement)
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    //Get the currently focused element
                    if (focusedElement == null)
                    {
                        focusedElement = (FrameworkElement)Keyboard.FocusedElement;
                    }

                    //Check the currently focused element
                    if (focusedElement != null)
                    {
                        //Get focus type
                        Type focusType = focusedElement.GetType().BaseType;

                        //Validate focus type
                        if (focusType == typeof(Window))
                        {
                            Debug.WriteLine("Invalid element focus type: " + focusType);
                            saveElement = null;
                            return;
                        }

                        //Check if save element is null
                        if (saveElement == null)
                        {
                            Debug.WriteLine("Save element is null creating new.");
                            saveElement = new AVFocusDetails();
                        }

                        //Save focused element
                        saveElement.FocusElement = focusedElement;

                        //Save listbox details
                        if (focusType == typeof(ListBoxItem))
                        {
                            saveElement.FocusListBox = AVFunctions.FindVisualParent<ListBox>(saveElement.FocusElement);
                            saveElement.FocusIndex = saveElement.FocusListBox.SelectedIndex;
                        }
                        else if (focusType == typeof(ListBox))
                        {
                            saveElement.FocusListBox = (ListBox)saveElement.FocusElement;
                            saveElement.FocusIndex = saveElement.FocusListBox.SelectedIndex;
                        }

                        Debug.WriteLine("Saved element focus: " + focusedElement + " / index: " + saveElement.FocusIndex);
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed saving element focus: " + ex.Message);
            }
        }
    }
}