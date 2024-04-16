using System.Diagnostics;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVActions
    {
        public class AVHighResDelay
        {
            private static bool vSetTimerResolution = false;
            public static unsafe void Delay(float milliSecondsDelay)
            {
                try
                {
                    if (!vSetTimerResolution)
                    {
                        NtSetTimerResolution(5000, true, out uint currentResolution);
                        Debug.WriteLine("Set timer resolution to: " + currentResolution);
                        vSetTimerResolution = currentResolution == 5000;
                    }

                    if (milliSecondsDelay < 0.1F)
                    {
                        milliSecondsDelay = 0.1F;
                    }

                    long nanoSecondsDelay = (long)(-1.0F * milliSecondsDelay * 10000.0F);
                    NtDelayExecution(false, ref nanoSecondsDelay);
                }
                catch { }
            }
        }
    }
}