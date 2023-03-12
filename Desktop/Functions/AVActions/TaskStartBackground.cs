using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public partial class AVActions
    {
        ///<param name="actionRun">void TaskAction() { void(); }</param>
        ///<example>AVActions.TaskStartBackground(TaskAction);</example>
        ///<summary>Don't forget to use try and catch to improve stability</summary>
        public static void TaskStartBackground(Action actionRun)
        {
            try
            {
                Task.Run(actionRun);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start background task: " + ex.Message);
            }
        }

        ///<param name="actionRun">async Task TaskAction() { void(); }</param>
        ///<example>AVActions.TaskStartBackground(TaskAction);</example>
        ///<summary>Don't forget to use try and catch to improve stability</summary>
        public static void TaskStartBackground(Func<Task> actionRun)
        {
            try
            {
                Task.Run(actionRun);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start background task: " + ex.Message);
            }
        }
    }
}