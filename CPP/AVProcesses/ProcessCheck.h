#pragma once
#include <string>
#include <vector>
#include <wtypes.h>
#include <dwmapi.h>
#pragma comment(lib, "Dwmapi.lib")

namespace ArnoldVinkCode::AVProcesses
{
	//Check if path is uwp application
	inline bool Check_PathUwpApplication(std::wstring targetPath)
	{
		try
		{
			if (targetPath.empty()) { return false; }
			return !wstring_contains(targetPath, L"\\") && !wstring_contains(targetPath, L"/") && wstring_contains(targetPath, L"!") && wstring_contains(targetPath, L"_");
		}
		catch (...) {}
		return false;
	}

	//Check if path is url protocol
	inline bool Check_PathUrlProtocol(std::wstring targetPath)
	{
		try
		{
			bool dividerPosition = targetPath.find(L":") > 1;
			bool urlProtocol = wstring_contains(targetPath, L":/") || wstring_contains(targetPath, L":\\");
			return urlProtocol && dividerPosition;
		}
		catch (...) {}
		return false;
	}

	//Check if process is running by process id
	inline bool Check_RunningProcessByProcessId(int targetProcessId)
	{
		try
		{
			return Get_ProcessAll(targetProcessId).size() > 0;
		}
		catch (...)
		{
			AVDebugWriteLine("Failed to check process by id: " << targetProcessId);
			return false;
		}
	}

	//Check if process is running by window handle
	inline bool Check_RunningProcessByWindowHandle(HWND targetWindowHandle)
	{
		try
		{
			return Detail_ProcessIdByWindowHandle(targetWindowHandle) > 0;
		}
		catch (...)
		{
			AVDebugWriteLine("Failed to check process by window handle.");
			return false;
		}
	}

	/// <summary>
	/// Check if process is running by name
	/// </summary>
	/// <param name="targetProcessName">Process name without extension</param>
	/// <param name="exactName">Search for exact process name</param>
	inline bool Check_RunningProcessByName(std::string targetProcessName, bool exactName)
	{
		try
		{
			return Get_ProcessByName(targetProcessName, exactName).size() > 0;
		}
		catch (...)
		{
			AVDebugWriteLine("Failed to check running process by name.");
			return false;
		}
	}

	/// <summary>
	/// Check if process is running by AppUserModelId
	/// </summary>
	/// <param name="targetAppUserModelId">UWP or Win32Store AppUserModelId</param>
	inline bool Check_RunningProcessByAppUserModelId(std::string targetAppUserModelId)
	{
		try
		{
			return Get_ProcessByAppUserModelId(targetAppUserModelId).size() > 0;
		}
		catch (...)
		{
			AVDebugWriteLine("Failed to check running process by AppUserModelId.");
			return false;
		}
	}

	//Check process is foreground uwp application
	inline bool Check_ProcessIsForegroundUwpApp(int targetProcessId)
	{
		try
		{
			for (HWND windowHandle : Detail_WindowHandlesByProcessId(targetProcessId))
			{
				std::string classNameString = Detail_ClassNameByWindowHandle(windowHandle);
				if (classNameString == "MSCTFIME UI")
				{
					return true;
				}
			}
		}
		catch (...) {}
		return false;
	}

	//Check if window class name is from uwp application
	inline bool Check_WindowClassNameIsUwpApp(std::string targetClassName)
	{
		try
		{
			std::string checkString = string_to_lower(targetClassName);
			std::vector<std::string> matchStrings{ "ApplicationFrameWindow", "Windows.UI.Core.CoreWindow" };
			for (std::string matchString : matchStrings)
			{
				if (string_contains(checkString, string_to_lower(matchString))) { return true; }
			}
		}
		catch (...) {}
		return false;
	}

	//Check if window class name is valid
	inline bool Check_WindowClassNameIsValid(std::string targetClassName)
	{
		try
		{
			std::string checkString = string_to_lower(targetClassName);
			std::vector<std::string> matchStrings{ "ApplicationManager_ImmersiveShellWindow", "Windows.Internal.Shell.TabProxyWindow", "ADLXEventWindowClass" };
			for (std::string matchString : matchStrings)
			{
				if (string_contains(checkString, string_to_lower(matchString))) { return false; }
			}
		}
		catch (...) {}
		return true;
	}

	//Check if window process name is valid
	inline bool Check_WindowProcessNameIsValid(std::string targetProcessName)
	{
		try
		{
			std::string checkString = string_to_lower(targetProcessName);
			std::vector<std::string> matchStrings{ "ApplicationFrameHost.exe", "StartMenuExperienceHost.exe", "WebExperienceHostApp.exe", "SearchHost.exe", "TextInputHost.exe", "backgroundTaskHost.exe", "ShellHost.exe", "ShellExperienceHost.exe", "WWAHost.exe", "StoreDesktopExtension.exe", "msedgewebview2.exe" };
			for (std::string matchString : matchStrings)
			{
				if (string_contains(checkString, string_to_lower(matchString))) { return false; }
			}
		}
		catch (...) {}
		return true;
	}

	//Check if window handle is a valid window
	inline bool Check_WindowHandleValid(HWND targetWindowHandle, bool checkMainWindow, bool checkVisibility)
	{
		try
		{
			//Check if handle is empty
			if (targetWindowHandle == NULL)
			{
				//AVDebugWriteLine(L"Window handle is empty.");
				return false;
			}

			//Check window styles
			DWORD windowStyle = GetWindowLongW(targetWindowHandle, GWL_STYLE);
			if ((windowStyle & WS_DISABLED) == true)
			{
				//AVDebugWriteLine(L"Window has disabled style and can't be shown or hidden: " << targetWindowHandle);
				return false;
			}
			if (checkVisibility)
			{
				if ((windowStyle & WS_VISIBLE) == false)
				{
					//AVDebugWriteLine(L"Window missing visible style and can't be shown or hidden: " << targetWindowHandle);
					return false;
				}
			}

			//Check window styles ex
			DWORD windowStyleEx = GetWindowLongW(targetWindowHandle, GWL_EXSTYLE);
			if ((windowStyleEx & WS_EX_TOOLWINDOW) == true)
			{
				//AVDebugWriteLine("Window has tool style and can't be shown or hidden: " << targetWindowHandle);
				return false;
			}

			//Check window is cloaked
			int dwmCloakedFlag;
			DwmGetWindowAttribute(targetWindowHandle, DWMWA_CLOAKED, &dwmCloakedFlag, sizeof(dwmCloakedFlag));
			if (dwmCloakedFlag != 0x00000000)
			{
				//AVDebugWriteLine("Window has cloaked flag: " << targetWindowHandle << " / " << dwmCloakedFlag);
				return false;
			}

			//Check window class name
			std::string windowClassName = Detail_ClassNameByWindowHandle(targetWindowHandle);
			if (!Check_WindowClassNameIsValid(windowClassName))
			{
				//AVDebugWriteLine("Window class name is invalid: " << targetWindowHandle);
				return false;
			}

			//Check window title length
			if (GetWindowTextLengthW(targetWindowHandle) <= 0)
			{
				//AVDebugWriteLine("Window has no title and can't be shown or hidden: " << targetWindowHandle);
				return false;
			}

			//Check if window is main or top
			if (checkMainWindow)
			{
				HWND windowOwner = GetWindow(targetWindowHandle, GW_OWNER);
				if (windowOwner != NULL)
				{
					HWND windowPopup = GetLastActivePopup(windowOwner);
					if (windowPopup != targetWindowHandle)
					{
						//AVDebugWriteLine("Window is not main or top: " << targetWindowHandle);
						return false;
					}
				}
			}
		}
		catch (...) {}
		return true;
	}
}