using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

namespace ArnoldVinkCode
{
    public class AVAnimations
    {
        //Storyboard - Show menu swipe hint animation vertical
        public static void Ani_SwipeHintVertical(FrameworkElement ObjFrameworkElement, double Offset)
        {
            try
            {
                Storyboard sb_SwipeHintThemeAnimation = new Storyboard();
                SwipeHintThemeAnimation SwipeHintThemeAnimation = new SwipeHintThemeAnimation();
                SwipeHintThemeAnimation.ToVerticalOffset = Offset;
                SwipeHintThemeAnimation.ToHorizontalOffset = 0;
                sb_SwipeHintThemeAnimation.Children.Add(SwipeHintThemeAnimation);
                Storyboard.SetTarget(sb_SwipeHintThemeAnimation, ObjFrameworkElement);
                sb_SwipeHintThemeAnimation.Begin();
            }
            catch { }
        }

        //Storyboard - Show menu swipe hint animation horizontal
        public static void Ani_SwipeHintHorizontal(FrameworkElement ObjFrameworkElement, double Offset)
        {
            try
            {
                Storyboard sb_SwipeHintThemeAnimation = new Storyboard();
                SwipeHintThemeAnimation SwipeHintThemeAnimation = new SwipeHintThemeAnimation();
                SwipeHintThemeAnimation.ToHorizontalOffset = Offset;
                SwipeHintThemeAnimation.ToVerticalOffset = 0;
                sb_SwipeHintThemeAnimation.Children.Add(SwipeHintThemeAnimation);
                Storyboard.SetTarget(sb_SwipeHintThemeAnimation, ObjFrameworkElement);
                sb_SwipeHintThemeAnimation.Begin();
            }
            catch { }
        }

        //Storyboard - Image fade in and fade out animation
        public static void Ani_ImageFadeInandOut(FrameworkElement FadeOut, FrameworkElement FadeIn, BitmapImage ImageIn)
        {
            try
            {
                Image FadeInImageElement = (FadeIn as Image);
                Image FadeOutImageElement = (FadeOut as Image);
                FadeOutImageElement.Source = FadeInImageElement.Source;

                Storyboard sb_FadeOut = new Storyboard();
                sb_FadeOut.Children.Add(new FadeOutThemeAnimation());
                Storyboard.SetTarget(sb_FadeOut, FadeInImageElement);

                Storyboard sb_FadeIn = new Storyboard();
                sb_FadeIn.Children.Add(new FadeInThemeAnimation());
                Storyboard.SetTarget(sb_FadeIn, FadeInImageElement);

                sb_FadeOut.Completed += delegate
                {
                    FadeInImageElement.Source = ImageIn;
                    sb_FadeIn.Begin();
                };

                sb_FadeOut.Begin();
            }
            catch { }
        }

        //Storyboard - Text fade in and fade out animation
        public static void Ani_TextFadeInandOut(FrameworkElement FadeElement, string FadeText)
        {
            try
            {
                TextBlock FadeTextElement = (FadeElement as TextBlock);

                Storyboard sb_FadeOut = new Storyboard();
                sb_FadeOut.Children.Add(new FadeOutThemeAnimation());
                Storyboard.SetTarget(sb_FadeOut, FadeTextElement);

                Storyboard sb_FadeIn = new Storyboard();
                sb_FadeIn.Children.Add(new FadeInThemeAnimation());
                Storyboard.SetTarget(sb_FadeIn, FadeTextElement);

                sb_FadeOut.Completed += delegate
                {
                    FadeTextElement.Text = FadeText;
                    sb_FadeIn.Begin();
                };

                sb_FadeOut.Begin();
            }
            catch { }
        }

        //Storyboard - Change the Visibilty and Hittest
        public static void Ani_Visibility(FrameworkElement ObjFrameworkElement, bool Visible, bool HitTest, double Speed)
        {
            try
            {
                DoubleAnimation DoubleAnimation = new DoubleAnimation();
                DoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(Speed));
                if (HitTest) { ObjFrameworkElement.IsHitTestVisible = true; }
                if (Visible)
                {
                    ObjFrameworkElement.Visibility = Visibility.Visible;
                    DoubleAnimation.From = ObjFrameworkElement.Opacity;
                    DoubleAnimation.To = 1.0;
                }
                else
                {
                    DoubleAnimation.From = ObjFrameworkElement.Opacity;
                    DoubleAnimation.To = 0;
                }

                Storyboard sb_Visibility = new Storyboard();
                sb_Visibility.Children.Add(DoubleAnimation);

                Storyboard.SetTarget(sb_Visibility, ObjFrameworkElement);
                Storyboard.SetTargetProperty(sb_Visibility, "Opacity");

                //On animation completed event
                sb_Visibility.Completed += delegate
                {
                    if (!Visible) { ObjFrameworkElement.Visibility = Visibility.Collapsed; }
                    if (!HitTest) { ObjFrameworkElement.IsHitTestVisible = false; }
                };
                sb_Visibility.Begin();
            }
            catch { }
        }

        //Storyboard - Change the Opacity and Hittest
        public static void Ani_Opacity(FrameworkElement ObjFrameworkElement, double Opacity, bool Visible, bool HitTest, double Speed)
        {
            try
            {
                DoubleAnimation DoubleAnimation = new DoubleAnimation();
                DoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(Speed));
                if (Visible) { ObjFrameworkElement.Visibility = Visibility.Visible; }
                if (HitTest)
                {
                    ObjFrameworkElement.IsHitTestVisible = true;
                    DoubleAnimation.From = ObjFrameworkElement.Opacity;
                    DoubleAnimation.To = 1.0;
                }
                else
                {
                    ObjFrameworkElement.IsHitTestVisible = false;
                    DoubleAnimation.From = ObjFrameworkElement.Opacity;
                    DoubleAnimation.To = Opacity;
                }

                Storyboard sb_Opacity = new Storyboard();
                sb_Opacity.Children.Add(DoubleAnimation);

                Storyboard.SetTarget(sb_Opacity, ObjFrameworkElement);
                Storyboard.SetTargetProperty(sb_Opacity, "Opacity");

                //On animation completed event
                sb_Opacity.Completed += delegate
                {
                    if (!Visible) { ObjFrameworkElement.Visibility = Visibility.Collapsed; }
                };
                sb_Opacity.Begin();
            }
            catch { }
        }

        //Storyboard - Change the Width
        public static void Ani_Width(FrameworkElement ObjFrameworkElement, Int32 ToWidth, bool Visible, bool HitTest, double Speed)
        {
            try
            {
                DoubleAnimation DoubleAnimation = new DoubleAnimation();
                DoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(Speed));
                DoubleAnimation.EnableDependentAnimation = true;

                if (HitTest) { ObjFrameworkElement.IsHitTestVisible = true; }
                if (Visible)
                {
                    DoubleAnimation.From = ObjFrameworkElement.Width;
                    DoubleAnimation.To = ToWidth;
                }
                else
                {
                    DoubleAnimation.From = ObjFrameworkElement.Width;
                    DoubleAnimation.To = 0;
                }

                Storyboard sb_Ani_Width = new Storyboard();
                sb_Ani_Width.Children.Add(DoubleAnimation);

                Storyboard.SetTarget(sb_Ani_Width, ObjFrameworkElement);
                Storyboard.SetTargetProperty(sb_Ani_Width, "Width");

                //On animation completed event
                sb_Ani_Width.Completed += delegate
                {
                    if (!HitTest) { ObjFrameworkElement.IsHitTestVisible = false; }
                };
                sb_Ani_Width.Begin();
            }
            catch { }
        }

        //Storyboard - Change the Height
        public static void Ani_Height(FrameworkElement ObjFrameworkElement, Int32 ToHeight, bool HitTest, double Speed)
        {
            try
            {
                DoubleAnimation DoubleAnimation = new DoubleAnimation();
                DoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(Speed));
                DoubleAnimation.EnableDependentAnimation = true;

                DoubleAnimation.From = ObjFrameworkElement.Height;
                DoubleAnimation.To = ToHeight;

                Storyboard sb_Ani_Height = new Storyboard();
                sb_Ani_Height.Children.Add(DoubleAnimation);

                Storyboard.SetTarget(sb_Ani_Height, ObjFrameworkElement);
                Storyboard.SetTargetProperty(sb_Ani_Height, "Height");

                //On animation completed event
                sb_Ani_Height.Completed += delegate { ObjFrameworkElement.IsHitTestVisible = HitTest; };
                sb_Ani_Height.Begin();
            }
            catch { }
        }
    }
}