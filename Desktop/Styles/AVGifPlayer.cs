using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace ArnoldVinkCode.Styles
{
    //Import:
    //xmlns:styles="clr-namespace:ArnoldVinkCode.Styles"
    //Usage:
    //<styles:AVGifPlayer GifSource="Assets/Icons/Loading.gif"/>
    public class AVGifPlayer : Image
    {
        //GifPlayer variables
        private GifBitmapDecoder vGifDecoder = null;
        private Int32Animation vInt32Animation = null;
        private bool vAnimating = false;

        /// <summary>
        /// Defines the gif frame index
        /// </summary>
        private int FrameIndex
        {
            get { return (int)GetValue(FrameIndexProperty); }
            set { SetValue(FrameIndexProperty, value); }
        }
        private static readonly DependencyProperty FrameIndexProperty = DependencyProperty.Register("FrameIndex", typeof(int), typeof(AVGifPlayer), new FrameworkPropertyMetadata(0, FrameIndexPropertyChanged));
        private static void FrameIndexPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs ev)
        {
            try
            {
                AVGifPlayer gifPlayer = sender as AVGifPlayer;
                gifPlayer.Source = gifPlayer.vGifDecoder.Frames[(int)ev.NewValue];
            }
            catch { }
        }

        /// <summary>
        /// Defines if the gif automatically starts
        /// </summary>
        public bool AutoStart
        {
            get { return (bool)GetValue(AutoStartProperty); }
            set { SetValue(AutoStartProperty, value); }
        }
        public static readonly DependencyProperty AutoStartProperty = DependencyProperty.Register("AutoStart", typeof(bool), typeof(AVGifPlayer), new FrameworkPropertyMetadata(false, AutoStartPropertyChanged));
        private static void AutoStartPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if ((bool)e.NewValue)
                {
                    AVGifPlayer gifPlayer = sender as AVGifPlayer;
                    gifPlayer.StartAnimation();
                }
            }
            catch { }
        }

        /// <summary>
        /// Defines the gif uri source
        /// </summary>
        public Uri GifSource
        {
            get { return new Uri(GetValue(GifSourceProperty).ToString(), UriKind.RelativeOrAbsolute); }
            set { SetValue(GifSourceProperty, value); }
        }
        public static readonly DependencyProperty GifSourceProperty = DependencyProperty.Register("GifSource", typeof(Uri), typeof(AVGifPlayer), new FrameworkPropertyMetadata(null, GifSourcePropertyChanged));
        private static void GifSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                AVGifPlayer gifPlayer = sender as AVGifPlayer;
                gifPlayer.InitializeGif();
            }
            catch { }
        }

        /// <summary>
        /// Defines the gif playback speed
        /// </summary>
        public double SpeedRatio
        {
            get { return (double)GetValue(SpeedRatioProperty); }
            set { SetValue(SpeedRatioProperty, value); }
        }
        public static readonly DependencyProperty SpeedRatioProperty = DependencyProperty.Register("SpeedRatio", typeof(double), typeof(AVGifPlayer), new FrameworkPropertyMetadata(1.00, SpeedRatioPropertyChanged));
        private static void SpeedRatioPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                AVGifPlayer gifPlayer = sender as AVGifPlayer;
                if (gifPlayer.vInt32Animation != null)
                {
                    gifPlayer.vInt32Animation.SpeedRatio = (double)e.NewValue;
                    if (gifPlayer.vAnimating)
                    {
                        gifPlayer.StopAnimation();
                        gifPlayer.StartAnimation();
                    }
                }
            }
            catch { }
        }

        private void InitializeGif()
        {
            try
            {
                if (GifSource != null)
                {
                    vGifDecoder = new GifBitmapDecoder(GifSource, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    vInt32Animation = new Int32Animation(0, vGifDecoder.Frames.Count - 1, new Duration(new TimeSpan(0, 0, 0, vGifDecoder.Frames.Count / 10, (int)((vGifDecoder.Frames.Count / 10.0 - vGifDecoder.Frames.Count / 10) * 1000))));
                    vInt32Animation.RepeatBehavior = RepeatBehavior.Forever;
                    vInt32Animation.SpeedRatio = SpeedRatio;
                }
            }
            catch { }
        }

        /// <summary>
        /// Shows and starts the gif animation
        /// </summary>
        public void Show()
        {
            try
            {
                Visibility = Visibility.Visible;
                StartAnimation();
            }
            catch { }
        }

        /// <summary>
        /// Hides and stops the gif animation
        /// </summary>
        public void Hide()
        {
            try
            {
                Visibility = Visibility.Collapsed;
                StopAnimation();
            }
            catch { }
        }

        /// <summary>
        /// Starts the animation
        /// </summary>
        public void StartAnimation()
        {
            try
            {
                if (vInt32Animation == null)
                {
                    InitializeGif();
                }

                BeginAnimation(FrameIndexProperty, vInt32Animation);
                vAnimating = true;
            }
            catch { }
        }

        /// <summary>
        /// Stops the animation
        /// </summary>
        public void StopAnimation()
        {
            try
            {
                BeginAnimation(FrameIndexProperty, null);
                vAnimating = false;
            }
            catch { }
        }
    }
}