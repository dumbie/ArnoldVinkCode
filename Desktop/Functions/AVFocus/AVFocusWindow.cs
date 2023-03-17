using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace ArnoldVinkCode
{
    public partial class AVFocus
    {
        //Focus window element
        public static void FocusWindow(FrameworkElement windowElement)
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    //Check if window element is null
                    if (windowElement == null)
                    {
                        Debug.WriteLine("Focus window element is null.");
                        return;
                    }

                    //Get and check window element type
                    Type focusType = windowElement.GetType().BaseType;
                    if (focusType != typeof(Window))
                    {
                        Debug.WriteLine("Focus window element is not a window: " + focusType);
                        return;
                    }

                    //Update element layout
                    windowElement.UpdateLayout();

                    //Logical focus on element
                    windowElement.Focus();

                    //Keyboard focus on element
                    Keyboard.Focus(windowElement);
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed focusing on window: " + ex.Message);
            }
        }
    }
}