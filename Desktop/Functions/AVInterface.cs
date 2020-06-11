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
                    int whileLoopCount = 0;
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

                        //Press tab key when no element is focused
                        if (whileLoopCount >= 30)
                        {
                            if (Keyboard.FocusedElement == null)
                            {
                                Debug.WriteLine("Failed focusing on the element after " + whileLoopCount + " times, pressing tab key.");
                                await KeySendSingle(KeysVirtual.Tab, mainWindowHandle);
                            }
                            return;
                        }

                        whileLoopCount++;
                        await Task.Delay(10);
                    }

                    //Debug.WriteLine("Forced keyboard focus on: " + focusElement);
                }
                else
                {
                    Debug.WriteLine("Focus element cannot be focused on, pressing tab key.");
                    await KeySendSingle(KeysVirtual.Tab, mainWindowHandle);
                }
            }
            catch
            {
                Debug.WriteLine("Failed focusing on the element, pressing tab key.");
                await KeySendSingle(KeysVirtual.Tab, mainWindowHandle);
            }
        }
    }
}