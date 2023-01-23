using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ArnoldVinkCode.Styles
{
    //Import:
    //xmlns:AVStyles="clr-namespace:ArnoldVinkCode.Styles;assembly=ArnoldVinkCode"
    //Usage:
    //<AVStyles:TextBoxDelay/>

    public class TextBoxDelay : TextBox
    {
        private DispatcherTimer DispatcherTimerDelay = new DispatcherTimer();

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            AVFunctions.TimerRenew(ref DispatcherTimerDelay);
            DispatcherTimerDelay.Interval = TimeSpan.FromMilliseconds(1000);
            DispatcherTimerDelay.Tick += delegate
            {
                //Debug.WriteLine("Textbox text change delayed.");
                DispatcherTimerDelay.Stop();
                base.OnTextChanged(e);
            };
            DispatcherTimerDelay.Start();
        }
    }
}