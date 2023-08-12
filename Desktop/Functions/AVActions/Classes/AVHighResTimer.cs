using System;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVActions
    {
        public class AVHighResTimer
        {
            //Variables
            private IntPtr createEvent;
            private uint timeSetEventId;
            public event EventHandler Tick;
            private MultimediaTimerCallback multimediaTimerCallback;

            //Initialize
            public AVHighResTimer(uint milliSecondsTickRate)
            {
                try
                {
                    if (milliSecondsTickRate == 0) { milliSecondsTickRate = 1; }
                    createEvent = CreateEvent(IntPtr.Zero, true, false, null);
                    multimediaTimerCallback = new MultimediaTimerCallback(MultimediaTimerCallbackCode);
                    timeSetEventId = timeSetEvent(milliSecondsTickRate, 0, multimediaTimerCallback, UIntPtr.Zero, TimeSetEventFlags.TIME_PERIODIC);
                }
                catch { }
            }

            //Callback
            private void MultimediaTimerCallbackCode(uint uTimerID, uint uMsg, UIntPtr dwUser, UIntPtr dw1, UIntPtr dw2)
            {
                try
                {
                    Tick(this, null);
                }
                catch { }
            }

            //Functions
            public void Stop()
            {
                try
                {
                    if (timeSetEventId > 0)
                    {
                        timeKillEvent(timeSetEventId);
                    }
                    SafeCloseEvent(createEvent);
                }
                catch { }
            }
        }
    }
}