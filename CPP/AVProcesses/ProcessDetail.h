#pragma once
#include <windows.h>
#include "ProcessWindow.h"

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
}