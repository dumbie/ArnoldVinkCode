#pragma once
#include <windows.h>

namespace ArnoldVinkCode::AVProcesses
{
	inline bool Detail_ProcessRespondingByWindowHandle(HWND targetWindowHandle)
	{
		bool processResponding = true;
		try
		{
			if (targetWindowHandle == NULL) { return processResponding; }
			processResponding = !IsHungAppWindow(targetWindowHandle);
			AVDebugWriteLine("Process window handle is responding: " << processResponding << "/" << targetWindowHandle);
		}
		catch (...) { }
		return processResponding;
	}
}