using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace ArnoldVinkCode
{
    public partial class AVAnimations
    {
        //Storyboard - Show menu swipe hint animation vertical
        public static void Ani_SwipeHintVertical(FrameworkElement ObjFrameworkElement, double Offset)
        {
            try
            {
                //Not available on desktop
            }
            catch { }
        }

        //Storyboard - Show menu swipe hint animation horizontal
        public static void Ani_SwipeHintHorizontal(FrameworkElement ObjFrameworkElement, double Offset)
        {
            try
            {
                //Not available on desktop
            }
            catch { }
        }

        //Storyboard - Fade in and fade out animation
        public static void Ani_FadeInandOut(FrameworkElement ObjFrameworkElement, Task AniTask)
        {
            try
            {
                //Not available on desktop
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
                if (HitTest)
                {
                    ObjFrameworkElement.IsEnabled = true;
                    ObjFrameworkElement.IsHitTestVisible = true;
                }
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
                Storyboard.SetTargetProperty(sb_Visibility, new PropertyPath("Opacity"));

                //On animation completed event
                sb_Visibility.Completed += delegate
                {
                    if (!Visible) { ObjFrameworkElement.Visibility = Visibility.Collapsed; }
                    if (!HitTest)
                    {
                        ObjFrameworkElement.IsEnabled = false;
                        ObjFrameworkElement.IsHitTestVisible = false;
                    }
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
                    ObjFrameworkElement.IsEnabled = true;
                    ObjFrameworkElement.IsHitTestVisible = true;
                    DoubleAnimation.From = ObjFrameworkElement.Opacity;
                    DoubleAnimation.To = 1.0;
                }
                else
                {
                    ObjFrameworkElement.IsEnabled = false;
                    ObjFrameworkElement.IsHitTestVisible = false;
                    DoubleAnimation.From = ObjFrameworkElement.Opacity;
                    DoubleAnimation.To = Opacity;
                }

                Storyboard sb_Opacity = new Storyboard();
                sb_Opacity.Children.Add(DoubleAnimation);

                Storyboard.SetTarget(sb_Opacity, ObjFrameworkElement);
                Storyboard.SetTargetProperty(sb_Opacity, new PropertyPath("Opacity"));

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
        public static void Ani_Width(FrameworkElement ObjFrameworkElement, int ToWidth, bool Visible, bool HitTest, double Speed)
        {
            try
            {
                DoubleAnimation DoubleAnimation = new DoubleAnimation();
                DoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(Speed));
                if (HitTest)
                {
                    ObjFrameworkElement.IsEnabled = true;
                    ObjFrameworkElement.IsHitTestVisible = true;
                }
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
                Storyboard.SetTargetProperty(sb_Ani_Width, new PropertyPath("Width"));

                //On animation completed event
                sb_Ani_Width.Completed += delegate
                {
                    if (!HitTest)
                    {
                        ObjFrameworkElement.IsEnabled = false;
                        ObjFrameworkElement.IsHitTestVisible = false;
                    }
                };
                sb_Ani_Width.Begin();
            }
            catch { }
        }

        //Storyboard - Change the Height
        public static void Ani_Height(FrameworkElement ObjFrameworkElement, int ToHeight, bool Visible, bool HitTest, double Speed)
        {
            try
            {
                DoubleAnimation DoubleAnimation = new DoubleAnimation();
                DoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(Speed));
                if (HitTest)
                {
                    ObjFrameworkElement.IsEnabled = true;
                    ObjFrameworkElement.IsHitTestVisible = true;
                }
                if (Visible)
                {
                    DoubleAnimation.From = ObjFrameworkElement.Height;
                    DoubleAnimation.To = ToHeight;
                }
                else
                {
                    DoubleAnimation.From = ObjFrameworkElement.Height;
                    DoubleAnimation.To = 0;
                }

                Storyboard sb_Ani_Height = new Storyboard();
                sb_Ani_Height.Children.Add(DoubleAnimation);

                Storyboard.SetTarget(sb_Ani_Height, ObjFrameworkElement);
                Storyboard.SetTargetProperty(sb_Ani_Height, new PropertyPath("Height"));

                //On animation completed event
                sb_Ani_Height.Completed += delegate
                {
                    if (!HitTest)
                    {
                        ObjFrameworkElement.IsEnabled = false;
                        ObjFrameworkElement.IsHitTestVisible = false;
                    }
                };
                sb_Ani_Height.Begin();
            }
            catch { }
        }
    }
}