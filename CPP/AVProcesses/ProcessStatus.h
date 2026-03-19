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
			//AVDebugWriteLine("Process window handle is responding: " << processResponding << "/" << targetWindowHandle);
		}
		catch (...) {}
		return processResponding;
	}

	inline ProcessAccessStatus Detail_ProcessAccessStatusByProcessId(int targetProcessId, bool currentProcess)
	{
		ProcessAccessStatus processAccessStatus = ProcessAccessStatus{};
		try
		{
			//Open process token
			auto processTokenHandle = AVFin<HANDLE>(AVFinMethod::CloseHandle);
			if (currentProcess)
			{
				processTokenHandle.Set(Token_Open_Current());
			}
			else
			{
				processTokenHandle.Set(Token_Open_Process(targetProcessId, PROCESS_QUERY_LIMITED_INFORMATION, TOKEN_QUERY));
			}

			//Check process token
			if (processTokenHandle.Get() == nullptr)
			{
				AVDebugWriteLine("Failed to get process access status for process id: " << targetProcessId);
				return processAccessStatus;
			}

			//Check process uiaccess access
			DWORD tokenLength = 0;
			DWORD tokenUiAccess = 0;
			GetTokenInformation(processTokenHandle.Get(), TokenUIAccess, &tokenUiAccess, sizeof(tokenUiAccess), &tokenLength);

			//Check process elevation access
			DWORD tokenElevation = 0;
			GetTokenInformation(processTokenHandle.Get(), TokenElevation, &tokenElevation, sizeof(tokenElevation), &tokenLength);

			//Check process elevation type
			TOKEN_ELEVATION_TYPE tokenElevationType = TokenElevationTypeDefault;
			GetTokenInformation(processTokenHandle.Get(), TokenElevationType, &tokenElevationType, sizeof(TOKEN_ELEVATION_TYPE), &tokenLength);

			//Create process access
			processAccessStatus.UiAccess = (bool)tokenUiAccess;
			processAccessStatus.Elevation = (bool)tokenElevation;
			processAccessStatus.ElevationType = tokenElevationType;

			//Check process admin access
			processAccessStatus.AdminAccess = processAccessStatus.Elevation || processAccessStatus.ElevationType == TokenElevationTypeFull;

			//AVDebugWriteLine("Process token uiaccess access: " << processAccessStatus.UiAccess);
			//AVDebugWriteLine("Process token administrator access: " << processAccessStatus.AdminAccess);
			//AVDebugWriteLine("Process token elevation access: " << processAccessStatus.Elevation);
			//AVDebugWriteLine("Process token elevation type: " << processAccessStatus.ElevationType);
			return processAccessStatus;
		}
		catch (...)
		{
			AVDebugWriteLine("Failed to get process access status");
			return processAccessStatus;
		}
	}
}