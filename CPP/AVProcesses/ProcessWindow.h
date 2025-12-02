#pragma once
#include <windows.h>

namespace ArnoldVinkCode::AVProcesses
{
	//Enumerate all windows by process id (including uwp and fullscreen)
	inline std::vector<HWND> Get_WindowHandlesByProcessId(int targetProcessId)
	{
		//AVDebugWriteLine(L"Getting window handles by process id: " << targetProcessId);
		std::vector<HWND> listWindows{};
		try
		{
			HWND childWindow = NULL;
			while ((childWindow = FindWindowExW(NULL, childWindow, NULL, NULL)) != NULL)
			{
				try
				{
					DWORD foundProcessId = 0;
					GetWindowThreadProcessId(childWindow, &foundProcessId);
					if (foundProcessId == targetProcessId)
					{
						listWindows.push_back(childWindow);
					}
				}
				catch (...) {}
			};
		}
		catch (...) {}
		return listWindows;
	}
}