#pragma once
#include <windows.h>
#include "ProcessDetail.h"

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

	//Enumerate all windows by AppUserModelId (including uwp and fullscreen)
	inline std::vector<HWND> Get_WindowHandlesByAppUserModelId(std::string targetAppUserModelId)
	{
		//AVDebugWriteLine("Getting window handles by AppUserModelId: " << targetAppUserModelId);
		std::vector<HWND> listWindows{};
		try
		{
			HWND childWindow = NULL;
			while ((childWindow = FindWindowExW(NULL, childWindow, NULL, NULL)) != NULL)
			{
				try
				{
					std::string targetAppUserModelIdLower = string_to_lower(targetAppUserModelId);
					std::string foundAppUserModelIdLower = string_to_lower(Detail_AppUserModelIdByWindowHandle(childWindow));
					if (targetAppUserModelIdLower == foundAppUserModelIdLower)
					{
						listWindows.push_back(childWindow);
					}
				}
				catch (...) {}
			}
		}
		catch (...) {}
		return listWindows;
	}
}