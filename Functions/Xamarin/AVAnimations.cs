using System;
using Xamarin.Forms;

namespace ArnoldVinkCode
{
    class AVAnimations
    {
        //Storyboard - Show menu swipe hint animation vertical
        internal static void Ani_SwipeHintVertical(Element ObjFrameworkElement, double Offset)
        {
            try
            {
                //Storyboard sb_SwipeHintThemeAnimation = new Storyboard();
                //SwipeHintThemeAnimation SwipeHintThemeAnimation = new SwipeHintThemeAnimation();
                //SwipeHintThemeAnimation.ToVerticalOffset = Offset;
                //SwipeHintThemeAnimation.ToHorizontalOffset = 0;
                //sb_SwipeHintThemeAnimation.Children.Add(SwipeHintThemeAnimation);
                //Storyboard.SetTarget(sb_SwipeHintThemeAnimation, ObjFrameworkElement);
                //sb_SwipeHintThemeAnimation.Begin();
            }
            catch { }
        }

        //Storyboard - Show menu swipe hint animation horizontal
        internal static void Ani_SwipeHintHorizontal(Element ObjFrameworkElement, double Offset)
        {
            try
            {
                //Storyboard sb_SwipeHintThemeAnimation = new Storyboard();
                //SwipeHintThemeAnimation SwipeHintThemeAnimation = new SwipeHintThemeAnimation();
                //SwipeHintThemeAnimation.ToHorizontalOffset = Offset;
                //SwipeHintThemeAnimation.ToVerticalOffset = 0;
                //sb_SwipeHintThemeAnimation.Children.Add(SwipeHintThemeAnimation);
                //Storyboard.SetTarget(sb_SwipeHintThemeAnimation, ObjFrameworkElement);
                //sb_SwipeHintThemeAnimation.Begin();
            }
            catch { }
        }

        //Storyboard - Image fade in and fade out animation
        internal static void Ani_ImageFadeInandOut(Element FadeOut, Element FadeIn, Image ImageIn)
        {
            try
            {
                //Image FadeInImageElement = (FadeIn as Image);
                //Image FadeOutImageElement = (FadeOut as Image);
                //FadeOutImageElement.Source = FadeInImageElement.Source;

                //Storyboard sb_FadeOut = new Storyboard();
                //sb_FadeOut.Children.Add(new FadeOutThemeAnimation());
                //Storyboard.SetTarget(sb_FadeOut, FadeInImageElement);

                //Storyboard sb_FadeIn = new Storyboard();
                //sb_FadeIn.Children.Add(new FadeInThemeAnimation());
                //Storyboard.SetTarget(sb_FadeIn, FadeInImageElement);

                //sb_FadeOut.Completed += delegate
                //{
                //    FadeInImageElement.Source = ImageIn;
                //    sb_FadeIn.Begin();
                //};

                //sb_FadeOut.Begin();
            }
            catch { }
        }

        //Storyboard - Text fade in and fade out animation
        internal static void Ani_TextFadeInandOut(Element FadeElement, string FadeText)
        {
            try
            {
                //TextBlock FadeTextElement = (FadeElement as TextBlock);

                //Storyboard sb_FadeOut = new Storyboard();
                //sb_FadeOut.Children.Add(new FadeOutThemeAnimation());
                //Storyboard.SetTarget(sb_FadeOut, FadeTextElement);

                //Storyboard sb_FadeIn = new Storyboard();
                //sb_FadeIn.Children.Add(new FadeInThemeAnimation());
                //Storyboard.SetTarget(sb_FadeIn, FadeTextElement);

                //sb_FadeOut.Completed += delegate
                //{
                //    FadeTextElement.Text = FadeText;
                //    sb_FadeIn.Begin();
                //};

                //sb_FadeOut.Begin();
            }
            catch { }
        }

        //Storyboard - Change the Visibilty and Hittest
        internal static void Ani_IsVisible(Element ObjFrameworkElement, bool Visible, bool HitTest, double Speed)
        {
            try
            {
                //DoubleAnimation DoubleAnimation = new DoubleAnimation();
                //DoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(Speed));
                //if (HitTest) { ObjFrameworkElement.IsEnabled = true; }
                //if (Visible)
                //{
                //    ObjFrameworkElement.IsVisible = true;
                //    DoubleAnimation.From = ObjFrameworkElement.Opacity;
                //    DoubleAnimation.To = 1.0;
                //}
                //else
                //{
                //    DoubleAnimation.From = ObjFrameworkElement.Opacity;
                //    DoubleAnimation.To = 0;
                //}

                //Storyboard sb_IsVisible = new Storyboard();
                //sb_IsVisible.Children.Add(DoubleAnimation);

                //Storyboard.SetTarget(sb_IsVisible, ObjFrameworkElement);
                //Storyboard.SetTargetProperty(sb_IsVisible, "Opacity");

                ////On animation completed event
                //sb_IsVisible.Completed += delegate
                //{
                //    if (!Visible) { ObjFrameworkElement.IsVisible = false; }
                //    if (!HitTest) { ObjFrameworkElement.IsEnabled = false; }
                //};
                //sb_IsVisible.Begin();
            }
            catch { }
        }

        //Storyboard - Change the Opacity and Hittest
        internal static void Ani_Opacity(Element ObjFrameworkElement, double Opacity, bool Visible, bool HitTest, double Speed)
        {
            try
            {
                //DoubleAnimation DoubleAnimation = new DoubleAnimation();
                //DoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(Speed));
                //if (Visible) { ObjFrameworkElement.IsVisible = true; }
                //if (HitTest)
                //{
                //    ObjFrameworkElement.IsEnabled = true;
                //    DoubleAnimation.From = ObjFrameworkElement.Opacity;
                //    DoubleAnimation.To = 1.0;
                //}
                //else
                //{
                //    ObjFrameworkElement.IsEnabled = false;
                //    DoubleAnimation.From = ObjFrameworkElement.Opacity;
                //    DoubleAnimation.To = Opacity;
                //}

                //Storyboard sb_Opacity = new Storyboard();
                //sb_Opacity.Children.Add(DoubleAnimation);

                //Storyboard.SetTarget(sb_Opacity, ObjFrameworkElement);
                //Storyboard.SetTargetProperty(sb_Opacity, "Opacity");

                ////On animation completed event
                //sb_Opacity.Completed += delegate
                //{
                //    if (!Visible) { ObjFrameworkElement.IsVisible = false; }
                //};
                //sb_Opacity.Begin();
            }
            catch { }
        }

        //Storyboard - Change the Width
        internal static void Ani_Width(Element ObjFrameworkElement, int ToWidth, bool Visible, bool HitTest, double Speed)
        {
            try
            {
                //DoubleAnimation DoubleAnimation = new DoubleAnimation();
                //DoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(Speed));
                //DoubleAnimation.EnableDependentAnimation = true;

                //if (HitTest) { ObjFrameworkElement.IsEnabled = true; }
                //if (Visible)
                //{
                //    DoubleAnimation.From = ObjFrameworkElement.Width;
                //    DoubleAnimation.To = ToWidth;
                //}
                //else
                //{
                //    DoubleAnimation.From = ObjFrameworkElement.Width;
                //    DoubleAnimation.To = 0;
                //}

                //Storyboard sb_Ani_Width = new Storyboard();
                //sb_Ani_Width.Children.Add(DoubleAnimation);

                //Storyboard.SetTarget(sb_Ani_Width, ObjFrameworkElement);
                //Storyboard.SetTargetProperty(sb_Ani_Width, "Width");

                ////On animation completed event
                //sb_Ani_Width.Completed += delegate
                //{
                //    if (!HitTest) { ObjFrameworkElement.IsEnabled = false; }
                //};
                //sb_Ani_Width.Begin();
            }
            catch { }
        }

        //Storyboard - Change the Height
        internal static void Ani_Height(Element ObjFrameworkElement, int ToHeight, bool Visible, bool HitTest, double Speed)
        {
            try
            {
                //DoubleAnimation DoubleAnimation = new DoubleAnimation();
                //DoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(Speed));
                //DoubleAnimation.EnableDependentAnimation = true;

                //if (HitTest) { ObjFrameworkElement.IsEnabled = true; }
                //if (Visible)
                //{
                //    DoubleAnimation.From = ObjFrameworkElement.Height;
                //    DoubleAnimation.To = ToHeight;
                //}
                //else
                //{
                //    DoubleAnimation.From = ObjFrameworkElement.Height;
                //    DoubleAnimation.To = 0;
                //}

                //Storyboard sb_Ani_Height = new Storyboard();
                //sb_Ani_Height.Children.Add(DoubleAnimation);

                //Storyboard.SetTarget(sb_Ani_Height, ObjFrameworkElement);
                //Storyboard.SetTargetProperty(sb_Ani_Height, "Height");

                ////On animation completed event
                //sb_Ani_Height.Completed += delegate
                //{
                //    if (!HitTest) { ObjFrameworkElement.IsEnabled = false; }
                //};
                //sb_Ani_Height.Begin();
            }
            catch { }
        }
    }
}