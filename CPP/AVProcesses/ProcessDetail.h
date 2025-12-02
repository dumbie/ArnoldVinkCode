#pragma once
#include <windows.h>
#include "ProcessWindow.h"
#include "ProcessCheck.h"

namespace ArnoldVinkCode::AVProcesses
{
	//Get class name by window handle
	inline std::string Detail_ClassNameByWindowHandle(HWND targetWindowHandle)
	{
		try
		{
			CHAR buffer[1024];
			DWORD bufferSize = sizeof(buffer);
			GetClassNameA(targetWindowHandle, buffer, bufferSize);
			return std::string(buffer);
		}
		catch (...) {}
		return "";
	}

	//Get full exe path by process handle
	inline std::string Detail_ExecutablePathByProcessHandle(HANDLE targetProcessHandle)
	{
		try
		{
			CHAR buffer[1024];
			DWORD bufferSize = sizeof(buffer);
			if (QueryFullProcessImageNameA(targetProcessHandle, 0, buffer, &bufferSize))
			{
				return std::string(buffer);
			}
		}
		catch (...) {}
		return "";
	}

	//Get main window handle by process id
	inline HWND Detail_WindowHandleMainByProcessId(int targetProcessId)
	{
		try
		{
			for (HWND windowHandle : Get_WindowHandlesByProcessId(targetProcessId))
			{
				try
				{
					if (Check_WindowHandleValid(windowHandle, true, true))
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
}