using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public partial class AVActions
    {
        ///<param name="actionRun">string TaskAction() { return ""; }</param>
        ///<example>await AVActions.TaskStartTimeoutReturn(TaskAction, 3000);</example>
        ///<summary>Don't forget to use try and catch to improve stability</summary>
        public static async Task<T> TaskStartTimeoutReturn<T>(Func<T> actionRun, int timeoutMs) where T : class
        {
            try
            {
                Task<T> runTask = Task.Run(actionRun);
                Task delayTask = Task.Delay(timeoutMs);
                Task timeoutTask = await Task.WhenAny(runTask, delayTask);
                if (timeoutTask == runTask)
                {
                    return runTask.Result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start return timeout task: " + ex.Message);
                return null;
            }
        }

        ///<param name="actionRun">async Task<string> TaskAction() { return ""; }</param>
        ///<param name="actionRun">Task<string> TaskAction() { return return ""; }</param>
        ///<example>await AVActions.TaskStartTimeoutReturn(TaskAction, 3000);</example>
        ///<summary>Don't forget to use try and catch to improve stability</summary>
        public static async Task<T> TaskStartTimeoutReturn<T>(Func<Task<T>> actionRun, int timeoutMs) where T : class
        {
            try
            {
                Task<T> runTask = Task.Run(actionRun);
                Task delayTask = Task.Delay(timeoutMs);
                Task timeoutTask = await Task.WhenAny(runTask, delayTask);
                if (timeoutTask == runTask)
                {
                    return runTask.Result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start return timeout task: " + ex.Message);
                return null;
            }
        }
    }
}