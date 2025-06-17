using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ArnoldVinkCode
{
    public partial class AVTaskScheduler
    {
        //Run service task
        public static async System.Threading.Tasks.Task<bool> TaskRun(string taskName, string appName, bool silentRun)
        {
            try
            {
                Debug.WriteLine("Running service task: " + taskName);

                //Check task
                TaskStatus taskStatus = TaskCheck(taskName, string.Empty);
                if (taskStatus != TaskStatus.TaskOk)
                {
                    //Show dialog
                    if (!silentRun)
                    {
                        List<string> messageBoxAnswers = new List<string>();
                        messageBoxAnswers.Add("Ok");

                        await new AVMessageBox().Popup(null, "Failed to start", "Please run the " + appName + " launcher manually once.", messageBoxAnswers);
                    }

                    return false;
                }
                else
                {
                    //Run task
                    using (TaskService taskService = new TaskService())
                    {
                        using (Task task = taskService.GetTask(taskName))
                        {
                            task.Run();
                        }
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to run service task: " + ex.Message);
                return false;
            }
        }

        //Check service task
        public static TaskStatus TaskCheck(string taskName, string taskExePath)
        {
            try
            {
                Debug.WriteLine("Checking service task: " + taskName);
                using (TaskService taskService = new TaskService())
                {
                    using (Task task = taskService.GetTask(taskName))
                    {
                        if (task == null)
                        {
                            Debug.WriteLine("Task does not exist.");
                            return TaskStatus.TaskNotFound;
                        }
                        else
                        {
                            //Get action execute path
                            string executePath = task.Definition.Actions.ToString();

                            //Check if application executable exists
                            if (!File.Exists(executePath))
                            {
                                Debug.WriteLine("Task execute file does not exist.");
                                return TaskStatus.ExeNotFound;
                            }

                            //Check if application path has changed
                            if (!string.IsNullOrWhiteSpace(taskExePath) && !executePath.Contains(taskExePath))
                            {
                                Debug.WriteLine("Task execute path has changed.");
                                return TaskStatus.PathChanged;
                            }

                            //Return task status
                            Debug.WriteLine("Task " + taskName + " should be working.");
                            return TaskStatus.TaskOk;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check service task: " + ex.Message);
                return TaskStatus.Unknown;
            }
        }

        //Create service task
        public static bool TaskCreate(string taskName, string taskDescription, string taskAuthor, string taskExePath, string taskRootPath)
        {
            try
            {
                Debug.WriteLine("Creating the service task: " + taskName);
                using (TaskService taskService = new TaskService())
                {
                    using (TaskDefinition taskDefinition = taskService.NewTask())
                    {
                        taskDefinition.RegistrationInfo.Description = taskDescription;
                        taskDefinition.RegistrationInfo.Author = taskAuthor;
                        taskDefinition.Settings.RunOnlyIfIdle = false;
                        taskDefinition.Settings.AllowDemandStart = true;
                        taskDefinition.Settings.DisallowStartIfOnBatteries = false;
                        taskDefinition.Settings.StopIfGoingOnBatteries = false;
                        taskDefinition.Settings.ExecutionTimeLimit = new TimeSpan();
                        taskDefinition.Settings.IdleSettings.StopOnIdleEnd = false;
                        taskDefinition.Settings.AllowHardTerminate = false;
                        taskDefinition.Principal.RunLevel = TaskRunLevel.Highest;
                        taskDefinition.Actions.Add(new ExecAction(taskExePath, null, taskRootPath));
                        taskService.RootFolder.RegisterTaskDefinition(taskName, taskDefinition);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to create service task: " + ex.Message);
                return false;
            }
        }

        //Remove service task
        public static void TaskRemove(string taskName)
        {
            try
            {
                using (TaskService taskService = new TaskService())
                {
                    taskService.RootFolder.DeleteTask(taskName);
                    Debug.WriteLine("Removed service task: " + taskName);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to remove service task: " + ex.Message);
            }
        }
    }
}