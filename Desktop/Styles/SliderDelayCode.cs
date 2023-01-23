using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ArnoldVinkCode.Styles
{
    //Import:
    //xmlns:AVStyles="clr-namespace:ArnoldVinkCode.Styles;assembly=ArnoldVinkCode"
    //Usage:
    //<AVStyles:SliderDelay/>

    public class SliderDelay : Slider
    {
        private bool SliderThumbDragging = false;
        private DispatcherTimer DispatcherTimerDelay = new DispatcherTimer();

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
            DispatcherTimerDelay.Interval = TimeSpan.FromMilliseconds(250);
            DispatcherTimerDelay.Tick += delegate
            {
                if (!SliderThumbDragging)
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