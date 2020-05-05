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
            Process returnProcess = null;
            try
            {
                //Prepare the process launch
                Task timeTask = Task.Run(delegate
                {
                    try
                    {
                        //Check if the exe file exists
                        if (!File.Exists(pathExe))
                        {
                            Debug.WriteLine("Launch executable not found.");
                            returnProcess = null;
                            return;
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
                        returnProcess = launchProcess;
                        Debug.WriteLine("Launched Win32 process identifier: " + returnProcess.Id);
                    }
                    catch { }
                });

                //Launch the process with timeout
                Task delayTask = Task.Delay(4000);
                Task timeoutTask = await Task.WhenAny(timeTask, delayTask);
                return returnProcess;
            }
            catch { }
            Debug.WriteLine("Failed launching Win32: " + Path.GetFileNameWithoutExtension(pathExe));
            return returnProcess;
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