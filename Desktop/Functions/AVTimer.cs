using System;
using System.Windows.Threading;

namespace ArnoldVinkCode
{
    public partial class AVTimer
    {
        //Dispatchtimer
        private DispatcherTimer dispatchTimer = null;

        //Set interval
        public void Interval(int intervalMs)
        {
            try
            {
                if (dispatchTimer == null)
                {
                    dispatchTimer = new DispatcherTimer();
                }

                dispatchTimer.Interval = TimeSpan.FromMilliseconds(intervalMs);
            }
            catch { }
        }

        //Set action
        public void Action(EventHandler action)
        {
            try
            {
                if (dispatchTimer == null)
                {
                    dispatchTimer = new DispatcherTimer();
                }

                dispatchTimer.Tick += action;
            }
            catch { }
        }

        //Reset timer tick estimate
        public void Reset()
        {
            try
            {
                if (dispatchTimer == null)
                {
                    dispatchTimer = new DispatcherTimer();
                }

                dispatchTimer.Stop();
                dispatchTimer.Start();
            }
            catch { }
        }

        //Stop timer
        public void Stop()
        {
            try
            {
                if (dispatchTimer == null)
                {
                    dispatchTimer = new DispatcherTimer();
                }

                dispatchTimer.Stop();
            }
            catch { }
        }

        //Start timer
        public void Start()
        {
            try
            {
                if (dispatchTimer == null)
                {
                    dispatchTimer = new DispatcherTimer();
                }

                dispatchTimer.Start();
            }
            catch { }
        }

        //Renew timer
        public void Renew()
        {
            try
            {
                if (dispatchTimer == null)
                {
                    dispatchTimer = new DispatcherTimer();
                }
                else
                {
                    dispatchTimer.Stop();
                    dispatchTimer = new DispatcherTimer();
                }
            }
            catch { }
        }
    }
}