using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace ArnoldVinkGifPlayer
{
    //xmlns:ArnoldVinkGifPlayer="clr-namespace:ArnoldVinkGifPlayer"
    //<ArnoldVinkGifPlayer:GifPlayer GifSource="/Assets/Icons/Loading.gif"/>
    public class GifPlayer : Image
    {
        private GifBitmapDecoder _gifDecoder;
        private Int32Animation _animation;
        private double _speedRatio = 1.0;
        private bool _isInitialized;

        private int FrameIndex
        {
            get { return (int)GetValue(FrameIndexProperty); }
            set { SetValue(FrameIndexProperty, value); }
        }

        private static readonly DependencyProperty FrameIndexProperty = DependencyProperty.Register("FrameIndex", typeof(int), typeof(GifPlayer), new FrameworkPropertyMetadata(0, new PropertyChangedCallback(ChangingFrameIndex)));

        private static void ChangingFrameIndex(DependencyObject obj, DependencyPropertyChangedEventArgs ev)
        {
            GifPlayer image = obj as GifPlayer;
            image.Source = image._gifDecoder.Frames[(int)ev.NewValue];
        }

        /// <summary>
        /// Defines whether the animation starts on it's own
        /// </summary>
        public bool AutoStart
        {
            get { return (bool)GetValue(AutoStartProperty); }
            set { SetValue(AutoStartProperty, value); }
        }

        public static readonly DependencyProperty AutoStartProperty = DependencyProperty.Register("AutoStart", typeof(bool), typeof(GifPlayer), new UIPropertyMetadata(false, AutoStartPropertyChanged));

        private static void AutoStartPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                (sender as GifPlayer).StartAnimation();
            }
        }

        public Uri GifSource
        {
            get { return (Uri)GetValue(GifSourceProperty); }
            set { SetValue(GifSourceProperty, value); }
        }

        public static readonly DependencyProperty GifSourceProperty = DependencyProperty.Register("GifSource", typeof(Uri), typeof(GifPlayer), new UIPropertyMetadata(null, GifSourcePropertyChanged));

        private static void GifSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as GifPlayer).InitializeGif();
        }

        private void InitializeGif()
        {
            try
            {
                if (this.GifSource != null)
                {
                    _gifDecoder = new GifBitmapDecoder(this.GifSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                    _animation = new Int32Animation(0, _gifDecoder.Frames.Count - 1, new Duration(new TimeSpan(0, 0, 0, _gifDecoder.Frames.Count / 10, (int)((_gifDecoder.Frames.Count / 10.0 - _gifDecoder.Frames.Count / 10) * 1000))));
                    _animation.RepeatBehavior = RepeatBehavior.Forever;
                    _animation.SpeedRatio = _speedRatio;
                    _isInitialized = true;
                }
            }
            catch { }
        }

        /// <summary>
        /// Shows and starts the gif animation
        /// </summary>
        public void Show()
        {
            this.Visibility = Visibility.Visible;
            this.StartAnimation();
        }

        /// <summary>
        /// Hides and stops the gif animation
        /// </summary>
        public void Hide()
        {
            this.Visibility = Visibility.Collapsed;
            this.StopAnimation();
        }

        /// <summary>
        /// Starts the animation
        /// </summary>
        public void StartAnimation()
        {
            if (!_isInitialized)
            {
                this.InitializeGif();
            }

            BeginAnimation(FrameIndexProperty, _animation);
        }

        /// <summary>
        /// Stops the animation
        /// </summary>
        public void StopAnimation()
        {
            BeginAnimation(FrameIndexProperty, null);
        }
    }
}