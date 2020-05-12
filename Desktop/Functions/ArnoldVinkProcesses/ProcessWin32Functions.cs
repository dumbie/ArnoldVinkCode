using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using static ArnoldVinkCode.ProcessFunctions;

namespace ArnoldVinkCode
{
    public partial class ProcessWin32Functions
    {
        //Launch a win32 application manually async
        public static async Task<Process> ProcessLauncherWin32Async(string pathExe, string pathLaunch, string runArgument, bool runAsAdmin, bool createNoWindow)
        {
            try
            {
                //Prepare the process launch
                Process TaskAction()
                {
                    try
                    {
                        //Check if the exe file exists
                        if (!File.Exists(pathExe))
                        {
                            Debug.WriteLine("Launch executable not found.");
                            return null;
                        }

                        //Show launching message
                        Debug.WriteLine("Launching Win32: " + Path.GetFileNameWithoutExtension(pathExe));

                        //Check the working path
                        if (string.IsNullOrWhiteSpace(pathLaunch)) { pathLaunch = Path.GetDirectoryName(pathExe); }

                        //Create process to start
                        Process launchProcess = new Process();
                        launchProcess.StartInfo.FileName = pathExe;
                        if (!string.IsNullOrWhiteSpace(pathLaunch))
                        {
                            launchProcess.StartInfo.WorkingDirectory = pathLaunch;
                        }
                        if (!string.IsNullOrWhiteSpace(runArgument))
                        {
                            launchProcess.StartInfo.Arguments = runArgument;
                        }
                        if (createNoWindow)
                        {
                            launchProcess.StartInfo.UseShellExecute = false;
                            launchProcess.StartInfo.CreateNoWindow = true;
                        }
                        if (runAsAdmin)
                        {
                            launchProcess.StartInfo.Verb = "runas";
                        }

                        //Start the process
                        launchProcess.Start();

                        //Return process
                        Debug.WriteLine("Launched Win32 process identifier: " + launchProcess.Id);
                        return launchProcess;
                    }
                    catch { }
                    Debug.WriteLine("Failed launching Win32: " + Path.GetFileNameWithoutExtension(pathExe));
                    return null;
                };

                //Launch the process
                return await AVActions.TaskStartReturn(TaskAction);
            }
            catch { }
            Debug.WriteLine("Failed launching Win32: " + Path.GetFileNameWithoutExtension(pathExe));
            return null;
        }

        //Restart a win32 process or app
        public static async Task<Process> RestartProcessWin32(int ProcessId, string PathExe, string PathLaunch, string Argument)
        {
            try
            {
                //Close the process or app
                if (ProcessId > 0)
                {
                    CloseProcessById(ProcessId);
                }
                else
                {
                    CloseProcessesByNameOrTitle(Path.GetFileNameWithoutExtension(PathExe), false);
                }
                await Task.Delay(1000);

                //Relaunch the process or app
                return await ProcessLauncherWin32Async(PathExe, PathLaunch, Argument, true, false);
            }
            catch { }
            return null;
        }
    }
}