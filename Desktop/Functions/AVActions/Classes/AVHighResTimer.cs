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

            //Initialize
            public AVHighResTimer(uint milliSecondsTickRate)
            {
                try
                {
                    if (milliSecondsTickRate == 0) { milliSecondsTickRate = 1; }
                    createEvent = CreateEvent(IntPtr.Zero, true, false, null);
                    timeSetEventId = timeSetEvent(milliSecondsTickRate, 0, MultimediaTimerDelegate, UIntPtr.Zero, TimeSetEventFlags.TIME_PERIODIC);
                }
                catch { }
            }

            //Delegate
            private void MultimediaTimerDelegate(uint uTimerID, uint uMsg, UIntPtr dwUser, UIntPtr dw1, UIntPtr dw2)
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
                    if (createEvent != IntPtr.Zero)
                    {
                        SetEvent(createEvent);
                    }
                    SafeCloseHandle(ref createEvent);
                }
                catch { }
            }
        }
    }
}