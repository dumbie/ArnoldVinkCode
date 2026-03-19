#pragma once
#include <windows.h>

namespace ArnoldVinkCode::AVProcesses
{
	//Restart process by process id
	inline bool Restart_ProcessByProcessId(int processId, std::wstring newArgs, bool withoutArgs)
	{
		try
		{
			AVDebugWriteLine("Restarting process by id: " << processId);

			//Get process
			AVProcess restartProcess = Get_ProcessByProcessId(processId).value();

			//Check process
			if (restartProcess.Identifier() <= 0)
			{
				AVDebugWriteLine("Failed to get restart process by id: " << processId);
				return false;
			}

			//Cache process
			restartProcess.Cache();

			//Check launch argument
			std::wstring launchArgument = L"";
			if (!withoutArgs)
			{
				launchArgument = newArgs;
				if (launchArgument.empty())
				{
					launchArgument = string_to_wstring(restartProcess.Argument());
				}
			}

			//Close current process
			Close_ProcessTreeByProcessId(processId);

			//Wait for process to have closed
			AVHighResDelay(500);

			//Launch process
			if (restartProcess.Type() == ProcessType::UWP || restartProcess.Type() == ProcessType::Win32Store)
			{
				std::wstring appUserModelIdW = string_to_wstring(restartProcess.AppUserModelId());
				return Launch_ApplicationUwp(appUserModelIdW, launchArgument);
			}
			else
			{
				std::wstring exePathW = string_to_wstring(restartProcess.ExePath());
				std::wstring workPathW = string_to_wstring(restartProcess.WorkPath());
				bool adminAccess = restartProcess.AccessStatus().AdminAccess;
				return Launch_ApplicationDesktop(exePathW, workPathW, launchArgument, adminAccess);
			}
		}
		catch (...)
		{
			AVDebugWriteLine("Failed to restart process id: " << processId);
			return false;
		}
	}
}