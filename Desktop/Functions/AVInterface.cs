using System;
using System.Diagnostics;
using System.Windows;

namespace ArnoldVinkCode
{
    public partial class AVInterface
    {
        //Get framework element dpi scale
        public static void GetDpiFromFrameworkElement(FrameworkElement dpiElement, out double dpiScaleHorizontal, out double dpiScaleVertical)
        {
            try
            {
                PresentationSource presentationSource = PresentationSource.FromVisual(dpiElement);
                dpiScaleHorizontal = presentationSource.CompositionTarget.TransformToDevice.M11;
                dpiScaleVertical = presentationSource.CompositionTarget.TransformToDevice.M22;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get dpi from framework element: " + ex.Message);
                dpiScaleHorizontal = 1;
                dpiScaleVertical = 1;
            }
        }
    }
}