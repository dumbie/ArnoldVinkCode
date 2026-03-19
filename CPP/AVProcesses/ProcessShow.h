#pragma once
#include <windows.h>

namespace ArnoldVinkCode::AVProcesses
{
	//Close Windows prompts
	inline void Close_WindowsPrompts()
	{
		try
		{
			//Windows administrator consent prompt
			if (Check_RunningProcessByName("consent", true))
			{
				AVDebugWriteLine("Windows administrator consent prompt is open, killing the process.");
				Close_ProcessByName("consent", true);
			}

			//Windows feature installation prompt
			if (Check_RunningProcessByName("fondue", true))
			{
				AVDebugWriteLine("Windows feature installation prompt is open, killing the process.");
				Close_ProcessByName("fondue", true);
			}
		}
		catch (...) {}
	}

	//Show window by window handle
	inline bool Show_ProcessByWindowHandle(HWND windowHandle)
	{
		try
		{
			//Check the window handle
			if (windowHandle == NULL)
			{
				AVDebugWriteLine("Failed showing process, window handle is empty.");
				return false;
			}

			AVDebugWriteLine("Showing process by window handle: " << windowHandle);

			//Close Windows prompts
			Close_WindowsPrompts();

			//Get current window placement
			WINDOWPLACEMENT windowPlacement{};
			GetWindowPlacement(windowHandle, &windowPlacement);

			//Check current window placement
			int windowSystemCommand = SC_RESTORE;
			int windowShowCommand = SW_RESTORE;
			if (windowPlacement.flags == WPF_RESTORETOMAXIMIZED)
			{
				windowSystemCommand = SC_MAXIMIZE;
				windowShowCommand = SW_SHOWMAXIMIZED;
			}

			//Allow set foreground window
			AllowSetForegroundWindow(ASFW_ANY);
			AVHighResDelay(50);

			//Retry to show the window
			int showCommandDelay = 25;
			for (int i = 0; i < 3; i++)
			{
				try
				{
					//Post message window
					PostMessageW(windowHandle, WM_SYSCOMMAND, windowSystemCommand, 0);
					AVHighResDelay(showCommandDelay);

					//Show window async
					ShowWindowAsync(windowHandle, windowShowCommand);
					AVHighResDelay(showCommandDelay);

					//Set foreground window
					SetForegroundWindow(windowHandle);
					AVHighResDelay(showCommandDelay);

					//Bring window to top
					//Locks thread when target window is not responding
					//BringWindowToTop(windowHandle);
					//AVHighResDelay(showCommandDelay);

					//Switch to the window
					SwitchToThisWindow(windowHandle, true);
					AVHighResDelay(showCommandDelay);
				}
				catch (...) {}
			}

			AVDebugWriteLine("Showed process window handle: " << windowHandle << "/Show command: " << windowShowCommand);
			return true;
		}
		catch (...)
		{
			AVDebugWriteLine("Failed showing process window handle: " << windowHandle);
			return false;
		}
	}

	//Show window by process id
	inline bool Show_ProcessByProcessId(int processId)
	{
		try
		{
			AVDebugWriteLine("Showing process by id: " << processId);

			//Get process
			AVProcess process = Get_ProcessByProcessId(processId).value();

			//Check process
			if (process.Identifier() <= 0)
			{
				AVDebugWriteLine("Failed showing process by id: " << processId);
				return false;
			}

			//Check window handle main
			if (process.WindowHandleMain() == NULL)
			{
				AVDebugWriteLine("Failed showing process by id: " << processId);
				return false;
			}

			//Show process window
			return Show_ProcessByWindowHandle(process.WindowHandleMain());
		}
		catch (...) {}
		return false;
	}
}