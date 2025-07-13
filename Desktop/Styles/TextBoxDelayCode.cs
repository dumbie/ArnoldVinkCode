using ArnoldVinkCode;
using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ArnoldVinkStyles
{
    //Import:
    //xmlns:AVStyles="clr-namespace:ArnoldVinkStyles;assembly=ArnoldVinkCode"
    //Usage:
    //<AVStyles:TextBoxDelay/>

    public class TextBoxDelay : TextBox
    {
        private bool SkipChangedEvent = false;
        private DispatcherTimer DispatcherTimerDelay = new DispatcherTimer();

        public void TextSkipEvent(dynamic newText)
        {
            SkipChangedEvent = true;
            base.Text = newText;
            SkipChangedEvent = false;
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            if (SkipChangedEvent) { return; }
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