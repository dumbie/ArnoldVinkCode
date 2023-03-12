using System;
using System.Threading.Tasks;
using System.Windows;

namespace ArnoldVinkCode
{
    public partial class AVActions
    {
        ///<param name="actionRun">void DispatchAction() { void(); }</param>
        ///<example>AVActions.DispatcherInvoke(DispatchAction);</example>
        ///<example>DispatcherInvoke(delegate { void(); });</example>
        ///<summary>Don't forget to use try and catch to improve stability</summary>
        public static void DispatcherInvoke(Action actionRun)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(actionRun);
            }
            catch { }
        }

        ///<param name="actionRun">async Task DispatchAction() { await void(); }</param>
        ///<example>await AVActions.DispatcherInvoke(DispatchAction);</example>
        ///<example>await AVActions.DispatcherInvoke(async delegate { await void(); });</example>
        ///<summary>Don't forget to use try and catch to improve stability</summary>
        public static async Task DispatcherInvoke(Func<Task> actionRun)
        {
            try
            {
                await Application.Current.Dispatcher.InvokeAsync(actionRun);
            }
            catch { }
        }
    }
}