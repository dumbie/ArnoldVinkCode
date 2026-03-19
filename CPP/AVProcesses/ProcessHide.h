#pragma once
#include <windows.h>

namespace ArnoldVinkCode::AVProcesses
{
	//Hide window by window handle
	inline bool Hide_ProcessByWindowHandle(HWND windowHandle)
	{
		try
		{
			//Check the window handle
			if (windowHandle == NULL)
			{
				AVDebugWriteLine("Failed hiding process, window handle is empty.");
				return false;
			}

			AVDebugWriteLine("Hiding process by window handle: " << windowHandle);
			int showCommandDelay = 25;

			//Post message window
			PostMessageW(windowHandle, WM_SYSCOMMAND, SC_MINIMIZE, 0);
			AVHighResDelay(showCommandDelay);

			//Hide window async
			ShowWindowAsync(windowHandle, SW_MINIMIZE);
			AVHighResDelay(showCommandDelay);

			AVDebugWriteLine("Hidden process window handle: " << windowHandle);
			return true;
		}
		catch (...)
		{
			AVDebugWriteLine("Failed hiding process window handle: " << windowHandle);
			return false;
		}
	}

	//Hide window by process id
	inline bool Hide_ProcessByProcessId(int processId)
	{
		try
		{
			AVDebugWriteLine("Hiding process by id: " << processId);

			//Get process
			AVProcess process = Get_ProcessByProcessId(processId).value();

			//Check process
			if (process.Identifier() <= 0)
			{
				AVDebugWriteLine("Failed hiding process by id: " << processId);
				return false;
			}

			//Check window handle main
			if (process.WindowHandleMain() == nullptr)
			{
				AVDebugWriteLine("Failed hiding process by id: " << processId);
				return false;
			}

			//Hide process window
			return Hide_ProcessByWindowHandle(process.WindowHandleMain());
		}
		catch (...) {}
		return false;
	}
}