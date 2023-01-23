using System;
using System.Windows.Controls;
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
        public bool MouseWheelScrollEnabled { get; set; } = true;
        private bool SliderThumbDragging = false;
        private DispatcherTimer DispatcherTimerDelay = new DispatcherTimer();

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

        protected override void OnThumbDragCompleted(System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            SliderThumbDragging = false;
            base.OnThumbDragCompleted(e);
        }

        protected override void OnThumbDragStarted(System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            SliderThumbDragging = true;
            base.OnThumbDragStarted(e);
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
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