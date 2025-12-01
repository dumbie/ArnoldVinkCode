#pragma once
#pragma comment(lib, "ntdll.lib")
#include <winternl.h>
#include <ntstatus.h>
#include <string>
#include <vector>

namespace AVProcesses
{
	//Enumerate all windows by process id (including uwp and fullscreen)
	static std::vector<HWND> Get_WindowHandlesByProcessId(int targetProcessId)
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