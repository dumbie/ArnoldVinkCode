using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public partial class AVActions
    {
        ///<example>await AVActions.TaskDelay(1);</example>
        ///<summary>
        ///Automatically choose between high resolution or regular delay.
        ///High resolution delay, allows to delay under 10 milliseconds.
        ///</summary>
        public static async Task TaskDelay(int milliSecondsDelay)
        {
            try
            {
                if (milliSecondsDelay < 50)
                {
                    AVHighResDelay.Delay((uint)milliSecondsDelay);
                }
                else
                {
                    await Task.Delay(milliSecondsDelay);
                }
            }
            catch { }
        }

        ///<example>await AVActions.TaskDelay(1, AVTask);</example>
        ///<summary>
        ///Automatically choose between high resolution or regular delay.
        ///High resolution delay, allows to delay under 10 milliseconds.
        ///</summary>
        public static async Task TaskDelay(int milliSecondsDelay, AVTaskDetails avTask)
        {
            try
            {
                if (milliSecondsDelay < 50)
                {
                    AVHighResDelay.Delay((uint)milliSecondsDelay);
                }
                else
                {
                    await Task.Delay(milliSecondsDelay, avTask.TokenSource.Token);
                }
            }
            catch { }
        }
    }
}