using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;

namespace ArnoldVinkCode
{
    public partial class AVInterface
    {
        //Force focus on a framework element
        public static async Task FocusOnElement(FrameworkElement focusElement, bool mouseCapture, IntPtr mainWindowHandle)
        {
            try
            {
                if (focusElement != null && focusElement.Focusable && focusElement.Visibility == Visibility.Visible)
                {
                    int WhileLoop = 0;
                    while (Keyboard.FocusedElement != focusElement)
                    {
                        //Update the element layout
                        focusElement.UpdateLayout();

                        //Logical focus on the element
                        focusElement.Focus();

                        //Keyboard focus on the element
                        Keyboard.Focus(focusElement);

                        //Mouse capture the element
                        if (mouseCapture)
                        {
                            Mouse.Capture(focusElement);
                        }

                        if (WhileLoop >= 30)
                        {
                            Debug.WriteLine("Failed focusing on the element after " + WhileLoop + " times, pressing tab key.");
                            KeySendSingle((byte)KeysVirtual.Tab, mainWindowHandle);
                            return;
                        }

                        WhileLoop++;
                        await Task.Delay(10);
                    }

                    //Debug.WriteLine("Forced keyboard focus on: " + focusElement);
                }
                else
                {
                    Debug.WriteLine("Focus element cannot be focused on, pressing tab key.");
                    KeySendSingle((byte)KeysVirtual.Tab, mainWindowHandle);
                }
            }
            catch
            {
                Debug.WriteLine("Failed focusing on the element, pressing tab key.");
                KeySendSingle((byte)KeysVirtual.Tab, mainWindowHandle);
            }
        }
    }
}