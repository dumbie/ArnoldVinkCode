#pragma once
#pragma comment(lib, "ntdll.lib")
#include <winternl.h>
#include <ntstatus.h>
#include <string>
#include <vector>

namespace AVProcesses
{
	//Get class name by window handle
	static std::string Detail_ClassNameByWindowHandle(HWND targetWindowHandle)
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
	static std::string Detail_ExecutablePathByProcessHandle(HANDLE targetProcessHandle)
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
	static HWND Detail_WindowHandleMainByProcessId(int targetProcessId)
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