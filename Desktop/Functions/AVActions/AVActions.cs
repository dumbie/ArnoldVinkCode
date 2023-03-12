using System;
using System.Diagnostics;
using System.Windows;

namespace ArnoldVinkCode
{
    public partial class AVActions
    {
        ///<example>AVActions.GetSystemTicksMs();</example>
        public static long GetSystemTicksMs()
        {
            try
            {
                return Stopwatch.GetTimestamp() / 10000;
            }
            catch { }
            return Environment.TickCount;
        }

        ///<example>AVActions.ElementGetValue(targetElement, targetProperty);</example>
        public static object ElementGetValue(FrameworkElement targetElement, DependencyProperty targetProperty)
        {
            object returnValue = null;
            try
            {
                DispatcherInvoke(delegate
                {
                    try
                    {
                        returnValue = targetElement.GetValue(targetProperty);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Failed getting value: " + ex.Message);
                    }
                });
            }
            catch { }
            return returnValue;
        }

        ///<example>AVActions.ElementSetValue(targetElement, targetProperty, targetValue);</example>
        public static void ElementSetValue(FrameworkElement targetElement, DependencyProperty targetProperty, object targetValue)
        {
            try
            {
                DispatcherInvoke(delegate
                {
                    try
                    {
                        targetElement.SetValue(targetProperty, targetValue);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Failed setting value: " + ex.Message);
                    }
                });
            }
            catch { }
        }
    }
}