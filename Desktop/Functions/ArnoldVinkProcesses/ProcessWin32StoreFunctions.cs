using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.ProcessFunctions;
using static ArnoldVinkCode.ProcessUwpFunctions;

namespace ArnoldVinkCode
{
    public partial class ProcessWin32StoreFunctions
    {
        //Restart a Win32Store process or app
        public static async Task<Process> RestartProcessWin32Store(string nameExe, string pathExe, int processId, string argument)
        {
            try
            {
                //Close the process or app
                if (processId > 0)
                {
                    KillProcessTreeById(processId, true);
                }
                else
                {
                    CloseProcessesByNameOrTitle(nameExe, false, true);
                }
                await Task.Delay(1000);

                //Relaunch the process or app
                return await ProcessLauncherUwpAndWin32StoreAsync(pathExe, argument);
            }
            catch { }
            return null;
        }
    }
}