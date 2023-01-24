using System;
using System.Threading.Tasks;
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
        private bool SkipChangedEvent = false;
        private DispatcherTimer DispatcherTimerDelay = new DispatcherTimer();

        public async void TextSkipEvent(dynamic newValue)
        {
            SkipChangedEvent = true;
            await Task.Delay(10);
            base.Text = newValue;
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