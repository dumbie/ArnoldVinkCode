#pragma once
#include <string>
#include <vector>
#include <wtypes.h>

namespace ArnoldVinkCode::AVProcesses
{
	//Check if window class name is valid
	inline bool Check_WindowClassNameIsValid(std::string targetClassName)
	{
		try
		{
			std::vector<std::string> classNamesInvalid{ "ApplicationManager_ImmersiveShellWindow", "Windows.Internal.Shell.TabProxyWindow" };
			for (std::string className : classNamesInvalid)
			{
				if (targetClassName == className) { return false; }
			}
		}
		catch (...) {}
		return true;
	}

	////Check if window class name is valid
	//bool Check_WindowClassNameIsValid(HWND targetWindowHandle)
	//{
	//	try
	//	{
	//		std::string classNameString = Detail_ClassNameByWindowHandle(targetWindowHandle);
	//		return Check_WindowClassNameIsValid(classNameString);
	//	}
	//	catch (...) {}
	//	return false;
	//}

	//Check if window handle is a valid window
	inline bool Check_WindowHandleValid(HWND targetWindowHandle, bool checkMainWindow, bool ignoreVisible)
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
			if (!ignoreVisible)
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

			////Check window class name
			//if (!Check_WindowClassNameIsValid(targetWindowHandle))
			//{
			//	AVDebugWriteLine("Window class name is invalid: " << targetWindowHandle);
			//	return false;
			//}

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