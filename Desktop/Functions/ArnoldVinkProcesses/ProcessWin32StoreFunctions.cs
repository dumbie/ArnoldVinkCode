using System.IO;
using System.Threading.Tasks;
using static ArnoldVinkCode.ProcessFunctions;
using static ArnoldVinkCode.ProcessUwpFunctions;

namespace ArnoldVinkCode
{
    public partial class ProcessWin32StoreFunctions
    {
        //Restart a Win32Store process or app
        public static async Task RestartProcessWin32Store(int processId, string executableName, string pathExe, string argument)
        {
            try
            {
                //Close the process or app
                if (processId > 0)
                {
                    CloseProcessById(processId);
                    await Task.Delay(1000);
                }
                else
                {
                    CloseProcessesByNameOrTitle(Path.GetFileNameWithoutExtension(executableName), false);
                    await Task.Delay(1000);
                }

                //Launch the Win32Store application
                ProcessLauncherUwp(pathExe, argument);
            }
            catch { }
        }
    }
}