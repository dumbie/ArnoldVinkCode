#pragma once
#include <windows.h>
#include <appmodel.h>
#include <propsys.h>
#include <propkey.h>
#include "..\AVTime.h"

#pragma comment(lib, "Oleacc.lib")
extern "C" HANDLE WINAPI GetProcessHandleFromHwnd(IN HWND hWnd);

namespace ArnoldVinkCode::AVProcesses
{
	//Get main window handle by process id
	inline HWND Detail_WindowHandleMainByProcessId(int targetProcessId, bool checkVisibility)
	{
		try
		{
			for (HWND windowHandle : Detail_WindowHandlesByProcessId(targetProcessId))
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

	//Get main window handle by thread id
	inline HWND Detail_WindowHandleMainByThreadId(int targetThreadId, bool checkVisibility)
	{
		try
		{
			for (HWND windowHandle : Detail_WindowHandlesByThreadId(targetThreadId))
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
	inline HWND Detail_WindowHandleMainByAppUserModelId(std::string targetAppUserModelId, bool checkVisibility)
	{
		try
		{
			for (HWND windowHandle : Detail_WindowHandlesByAppUserModelId(targetAppUserModelId))
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

	/// <summary>
	/// Get process handle by process identifier
	/// </summary>
	/// <param name="targetProcessId">Process identifier</param>
	inline HANDLE Detail_ProcessHandleByProcessId(int targetProcessId)
	{
		try
		{
			HANDLE hProcess = OpenProcess(MAXIMUM_ALLOWED, false, targetProcessId);
			if (hProcess == NULL)
			{
				//AVDebugWriteLine("Failed opening process id: " << targetProcessId << "/" << GetLastError());
				return NULL;
			}
			else
			{
				//AVDebugWriteLine("Opened process id: " << targetProcessId << "/" << GetLastError());
				return hProcess;
			}
		}
		catch (...)
		{
			AVDebugWriteLine("Failed to get process handle by id: " << targetProcessId);
			return NULL;
		}
	}

	//Get process start time by process handle
	inline std::tm Detail_ProcessStartTimeByProcessHandle(HANDLE processHandle)
	{
		std::tm startTime;
		try
		{
			//Get process times
			FILETIME creationTime, exitTime, kernelTime, userTime;
			GetProcessTimes(processHandle, &creationTime, &exitTime, &kernelTime, &userTime);

			//Convert start time
			startTime = filetime_to_tm(creationTime);
		}
		catch (...) {}
		return startTime;
	}

	//Get window title by window handle
	inline std::string Detail_WindowTitleByWindowHandle(HWND targetWindowHandle)
	{
		try
		{
			if (targetWindowHandle == NULL)
			{
				return "Unknown";
			}

			int stringLength = GetWindowTextLengthA(targetWindowHandle);
			if (stringLength > 0)
			{
				stringLength += 1;
				std::string buffer;
				buffer.resize(stringLength);
				GetWindowTextA(targetWindowHandle, buffer.data(), buffer.capacity());
				if (!buffer.empty())
				{
					return string_trim(buffer);
				}
			}
		}
		catch (...) {}
		return "Unknown";
	}

	//Get window Z order by window handle
	inline int Detail_WindowZOrderByWindowHandle(HWND windowHandle)
	{
		int zOrder = -1;
		try
		{
			HWND zHandle = windowHandle;
			while (zHandle != NULL)
			{
				zHandle = GetWindow(zHandle, GW_HWNDPREV);
				zOrder++;
			}
			//AVDebugWriteLine("Window: " << windowHandle << " ZOrder: " << zOrder);
		}
		catch (...) {}
		return zOrder;
	}

	//Get class name by window handle
	inline std::string Detail_ClassNameByWindowHandle(HWND targetWindowHandle)
	{
		try
		{
			std::string buffer;
			buffer.resize(1024);
			GetClassNameA(targetWindowHandle, buffer.data(), buffer.capacity());
			return buffer.c_str();
		}
		catch (...) {}
		return "";
	}

	//Get process id by window handle
	inline int Detail_ProcessIdByWindowHandle(HWND targetWindowHandle)
	{
		DWORD processId = -1;
		try
		{
			GetWindowThreadProcessId(targetWindowHandle, &processId);
		}
		catch (...) {}
		try
		{
			if (processId <= 0)
			{
				//AVDebugWriteLine("Process id 0, using GetProcessHandleFromHwnd as backup.");
				processId = GetProcessId(GetProcessHandleFromHwnd(targetWindowHandle));
			}
		}
		catch (...) {}
		return processId;
	}

	//Get full exe path by process handle
	inline std::string Detail_ExecutablePathByProcessHandle(HANDLE targetProcessHandle)
	{
		try
		{
			std::string buffer;
			buffer.resize(1024);
			DWORD bufferSize = buffer.capacity();
			if (QueryFullProcessImageNameA(targetProcessHandle, 0, buffer.data(), &bufferSize))
			{
				return buffer.c_str();
			}
		}
		catch (...) {}
		return "";
	}

	//Get full package name by process handle
	inline std::string Detail_PackageFullNameByProcessHandle(HANDLE targetProcessHandle)
	{
		try
		{
			std::wstring buffer;
			buffer.resize(1024);
			UINT32 bufferSize = buffer.capacity();
			if (GetPackageFullName(targetProcessHandle, &bufferSize, buffer.data()) == 0)
			{
				return wstring_to_string(buffer);
			}
		}
		catch (...) {}
		return "";
	}

	//Get AppUserModelId by process handle
	inline std::string Detail_AppUserModelIdByProcessHandle(HANDLE targetProcessHandle)
	{
		try
		{
			UINT32 stringLength = 1024;
			std::vector<WCHAR> stringBuilder(1024);
			if (GetApplicationUserModelId(targetProcessHandle, &stringLength, stringBuilder.data()) == ERROR_SUCCESS)
			{
				std::wstring appUserModelIdW = stringBuilder.data();
				if (Check_PathUwpApplication(appUserModelIdW))
				{
					return wstring_to_string(appUserModelIdW);
				}
			}
		}
		catch (...) {}
		return "";
	}

	//Get AppUserModelId by window handle
	inline std::string Detail_AppUserModelIdByWindowHandle(HWND targetWindowHandle)
	{
		try
		{
			auto propertyStore = AVFin<IPropertyStore*>(AVFinMethod::ReleaseInterface);
			SHGetPropertyStoreForWindow(targetWindowHandle, IID_PPV_ARGS(&propertyStore.Get()));

			auto propertyVariant = AVFinObj<PROPVARIANT>(AVFinObjMethod::PropVariantClear, PROPVARIANT{});
			propertyStore.Get()->GetValue(PKEY_AppUserModel_ID, &propertyVariant.Get());

			if (propertyVariant.Get().vt == VT_LPWSTR)
			{
				std::wstring appUserModelIdW = propertyVariant.Get().pwszVal;
				if (Check_PathUwpApplication(appUserModelIdW))
				{
					return wstring_to_string(appUserModelIdW);
				}
			}
		}
		catch (...) {}
		return "";
	}
}