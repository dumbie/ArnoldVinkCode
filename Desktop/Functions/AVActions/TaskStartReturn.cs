using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public partial class AVActions
    {
        ///<param name="actionRun">void TaskAction() { void(); }</param>
        ///<example>await AVActions.TaskStartReturn(TaskAction);</example>
        ///<summary>Don't forget to use try and catch to improve stability</summary>
        public static async Task<bool> TaskStartReturn(Action actionRun)
        {
            try
            {
                await Task.Run(actionRun);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start return task: " + ex.Message);
                return false;
            }
        }

        ///<param name="actionRun">void TaskAction() { void(); }</param>
        ///<example>AVActions.TaskStartStaReturn(TaskAction);</example>
        ///<summary>Don't forget to use try and catch to improve stability</summary>
        public static bool TaskStartStaReturn(Action actionRun)
        {
            try
            {
                Thread staThread = new Thread(() =>
                {
                    actionRun();
                });
                staThread.SetApartmentState(ApartmentState.STA);
                staThread.Start();
                staThread.Join();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start sta return task: " + ex.Message);
                return false;
            }
        }

        ///<param name="actionRun">string TaskAction() { return ""; }</param>
        ///<example>await AVActions.TaskStartReturn(TaskAction);</example>
        ///<summary>Don't forget to use try and catch to improve stability</summary>
        public static async Task<T> TaskStartReturn<T>(Func<T> actionRun) where T : class
        {
            try
            {
                return await Task.Run(actionRun);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start return task: " + ex.Message);
                return null;
            }
        }

        ///<param name="actionRun">async Task TaskAction() { void(); }</param>
        ///<example>await AVActions.TaskStartReturn(TaskAction);</example>
        ///<summary>Don't forget to use try and catch to improve stability</summary>
        public static async Task<bool> TaskStartReturn(Func<Task> actionRun)
        {
            try
            {
                await Task.Run(actionRun);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start return task: " + ex.Message);
                return false;
            }
        }

        ///<param name="actionRun">async Task<string> TaskAction() { return ""; }</param>
        ///<example>await AVActions.TaskStartReturn(TaskAction);</example>
        ///<summary>Don't forget to use try and catch to improve stability</summary>
        public static async Task<T> TaskStartReturn<T>(Func<Task<T>> actionRun) where T : class
        {
            try
            {
                return await Task.Run(actionRun);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start return task: " + ex.Message);
                return null;
            }
        }

        ///<param name="actionRun">string TaskAction() { return ""; }</param>
        ///<example>AVActions.TaskStartStaReturn(TaskAction);</example>
        ///<summary>Don't forget to use try and catch to improve stability</summary>
        public static T TaskStartStaReturn<T>(Func<T> actionRun) where T : class
        {
            T result = null;
            try
            {
                Thread staThread = new Thread(() =>
                {
                    result = actionRun();
                });
                staThread.SetApartmentState(ApartmentState.STA);
                staThread.Start();
                staThread.Join();
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start sta return task: " + ex.Message);
                return result;
            }
        }
    }
}