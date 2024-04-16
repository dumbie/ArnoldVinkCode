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
        public static async Task TaskDelay(float milliSecondsDelay)
        {
            try
            {
                if (milliSecondsDelay <= 50)
                {
                    AVHighResDelay.Delay(milliSecondsDelay);
                }
                else
                {
                    await Task.Delay((int)milliSecondsDelay);
                }
            }
            catch { }
        }

        ///<example>await AVActions.TaskDelay(1, AVTask);</example>
        ///<summary>
        ///Automatically choose between high resolution or regular delay.
        ///High resolution delay, allows to delay under 10 milliseconds.
        ///</summary>
        public static async Task TaskDelay(float milliSecondsDelay, AVTaskDetails avTask)
        {
            try
            {
                if (milliSecondsDelay <= 50)
                {
                    AVHighResDelay.Delay(milliSecondsDelay);
                }
                else
                {
                    await Task.Delay((int)milliSecondsDelay, avTask.TokenSource.Token);
                }
            }
            catch { }
        }
    }
}