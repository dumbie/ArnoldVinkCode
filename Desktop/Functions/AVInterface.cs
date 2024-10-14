using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace ArnoldVinkCode
{
    public partial class AVInterface
    {
        //Get framework element dpi scale
        public static void FrameworkElementGetDpi(FrameworkElement dpiElement, out double dpiScaleHorizontal, out double dpiScaleVertical)
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

        //Check if framework element is visible for user
        public static bool FrameworkElementVisibleUser(FrameworkElement targetElement, FrameworkElement parentElement)
        {
            try
            {
                if (!targetElement.IsVisible) { return false; }
                Rect rectTargetRender = new Rect(targetElement.RenderSize);
                if (rectTargetRender.Height == 0 || rectTargetRender.Width == 0) { return false; }
                Rect rectParentRender = new Rect(parentElement.RenderSize);
                GeneralTransform transform = targetElement.TransformToAncestor(parentElement);
                Rect rectTargetBounds = transform.TransformBounds(rectTargetRender);
                return rectParentRender.IntersectsWith(rectTargetBounds);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check framework element visibility: " + ex.Message);
                return false;
            }
        }
    }
}