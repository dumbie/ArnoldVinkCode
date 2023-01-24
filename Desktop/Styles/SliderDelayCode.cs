using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace ArnoldVinkCode.Styles
{
    //Import:
    //xmlns:AVStyles="clr-namespace:ArnoldVinkCode.Styles;assembly=ArnoldVinkCode"
    //Usage:
    //<AVStyles:SliderDelay/>

    public class SliderDelay : Slider
    {
        public double DelayTime { get; set; } = 500;
        public bool DelayIgnoreDrag { get; set; } = false;
        public bool SliderThumbDragging { get; protected set; } = false;
        public bool MouseWheelScrollEnabled { get; set; } = true;
        public DateTime LastValueChange { get; protected set; } = DateTime.Now;
        private DispatcherTimer DispatcherTimerDelay = new DispatcherTimer();
        private bool SkipChangedEvent = false;
        public bool RecentValueChange()
        {
            double changeDifference = (DateTime.Now - LastValueChange).TotalMilliseconds;
            if (changeDifference > 2500 && !SliderThumbDragging)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.MouseWheel += SliderDelay_MouseWheel;
            base.OnInitialized(e);
        }

        protected void SliderDelay_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (MouseWheelScrollEnabled)
            {
                if (e.Delta > 0)
                {
                    base.Value += base.LargeChange;
                }
                else
                {
                    base.Value -= base.LargeChange;
                }
            }
        }

        protected override void OnThumbDragCompleted(DragCompletedEventArgs e)
        {
            SliderThumbDragging = false;
            base.OnThumbDragCompleted(e);
        }

        protected override void OnThumbDragStarted(DragStartedEventArgs e)
        {
            SliderThumbDragging = true;
            base.OnThumbDragStarted(e);
        }

        public void ValueSkipEvent(dynamic newValue, bool checkRecentChange)
        {
            if (checkRecentChange && RecentValueChange()) { return; }
            SkipChangedEvent = true;
            base.Value = newValue;
            SkipChangedEvent = false;
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            if (SkipChangedEvent) { return; }
            LastValueChange = DateTime.Now;
            AVFunctions.TimerRenew(ref DispatcherTimerDelay);
            DispatcherTimerDelay.Interval = TimeSpan.FromMilliseconds(DelayTime);
            DispatcherTimerDelay.Tick += delegate
            {
                if (DelayIgnoreDrag || !SliderThumbDragging)
                {
                    //Debug.WriteLine("Slider value change delayed.");
                    DispatcherTimerDelay.Stop();
                    base.OnValueChanged(oldValue, newValue);
                }
            };
            DispatcherTimerDelay.Start();
        }
    }
}