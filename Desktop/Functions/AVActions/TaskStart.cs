using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public partial class AVActions
    {
        ///<param name="actionRun">void TaskAction() { void(); }</param>
        ///<example>AVActions.TaskStart(TaskAction);</example>
        ///<summary>Don't forget to use try and catch to improve stability</summary>
        public static void TaskStart(Action actionRun)
        {
            try
            {
                Task.Run(actionRun);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start regular task: " + ex.Message);
            }
        }

        ///<param name="actionRun">async Task TaskAction() { void(); }</param>
        ///<example>AVActions.TaskStart(TaskAction);</example>
        ///<summary>Don't forget to use try and catch to improve stability</summary>
        public static void TaskStart(Func<Task> actionRun)
        {
            try
            {
                Task.Run(actionRun);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start regular task: " + ex.Message);
            }
        }
    }
}