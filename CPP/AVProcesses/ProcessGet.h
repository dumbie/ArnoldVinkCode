#pragma once
#include <windows.h>
#include "ProcessCheck.h"

namespace ArnoldVinkCode::AVProcesses
{
	//Get main window handle by process id
	inline HWND Get_WindowHandleMainByProcessId(int targetProcessId, bool checkVisibility)
	{
		try
		{
			for (HWND windowHandle : Get_WindowHandlesByProcessId(targetProcessId))
			{
				try
				{
					if (Check_WindowHandleValid(windowHandle, true, checkVisibility))
					{
						return windowHandle;
					}
				}
				catch (...) {}
			}
		}
		catch (...) {}
		return NULL;
	}

	//Get main window handle by AppUserModelId
	inline HWND Get_WindowHandleMainByAppUserModelId(std::string targetAppUserModelId, bool checkVisibility)
	{
		try
		{
			for (HWND windowHandle : Get_WindowHandlesByAppUserModelId(targetAppUserModelId))
			{
				try
				{
					if (Check_WindowHandleValid(windowHandle, true, checkVisibility))
					{
						return windowHandle;
					}
				}
				catch (...) {}
			}
		}
		catch (...) {}
		return NULL;
	}

	//Get process handle by process identifier
	inline HANDLE Get_ProcessHandleByProcessId(int targetProcessId)
	{
		try
		{
			return OpenProcess(MAXIMUM_ALLOWED, false, targetProcessId);
		}
		catch (...) {}
		return NULL;
	}
}