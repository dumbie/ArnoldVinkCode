using System;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVActions
    {
        ///<example>await AVActions.TaskDelay(1);</example>
        ///<summary>Automatically choose between high resolution or regular delay.</summary>
        public static async Task TaskDelay(int millisecondsDelay)
        {
            try
            {
                if (millisecondsDelay < 50)
                {
                    TaskDelayHighRes((uint)millisecondsDelay);
                }
                else
                {
                    await Task.Delay(millisecondsDelay);
                }
            }
            catch { }
        }

        ///<example>await AVActions.TaskDelay(1, AVTask);</example>
        ///<summary>Automatically choose between high resolution or regular delay.</summary>
        public static async Task TaskDelay(int millisecondsDelay, AVTaskDetails avTask)
        {
            try
            {
                if (millisecondsDelay < 50)
                {
                    TaskDelayHighRes((uint)millisecondsDelay);
                }
                else
                {
                    await Task.Delay(millisecondsDelay, avTask.TokenSource.Token);
                }
            }
            catch { }
        }

        ///<example>await AVActions.TaskDelayHighRes(1);</example>
        ///<summary>High resolution delay, allows to delay under 10ms.</summary>
        public static void TaskDelayHighRes(uint millisecondsDelay)
        {
            try
            {
                IntPtr createEvent = CreateEvent(IntPtr.Zero, true, false, null);
                MultimediaTimerCallback callbackDone = delegate
                {
                    SetEvent(createEvent);
                    CloseHandleAuto(createEvent);
                };

                timeSetEvent(millisecondsDelay, 0, callbackDone, 0, 0);
                WaitForSingleObject(createEvent, INFINITE);
                callbackDone.EndInvoke(null);
            }
            catch { }
        }
    }
}