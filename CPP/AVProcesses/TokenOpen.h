#pragma once
#include <windows.h>
#pragma comment(lib, "ntdll.lib")
#include <winternl.h>
#include <ntstatus.h>
#include <processthreadsapi.h>
#include <string>
#include <vector>
#include "ProcessDetail.h"

namespace ArnoldVinkCode::AVProcesses
{
	//Get current process token
	inline HANDLE Token_Open_Current()
	{
		try
		{
			//Open current process token
			HANDLE hToken = NULL;
			if (!OpenProcessToken(GetCurrentProcess(), TOKEN_ALL_ACCESS, &hToken))
			{
				AVDebugWriteLine("Failed getting current process token: " << GetLastError());
				return NULL;
			}

			//AVDebugWriteLine("Got current process token: " << hToken);
			return hToken;
		}
		catch (...)
		{
			AVDebugWriteLine("Failed getting current process token.");
			return NULL;
		}
	}

	//Get other process token
	inline HANDLE Token_Open_Process(int processId, DWORD processAccess, DWORD tokenAccess)
	{
		try
		{
			//Open other process
			auto hProcess = AVFin(AVFinMethod::CloseHandle, OpenProcess(processAccess, false, processId));
			if (hProcess.Get() == NULL)
			{
				AVDebugWriteLine("Failed getting other process: " << processId << "/" << GetLastError());
				return NULL;
			}

			//Open other process token
			HANDLE hToken = NULL;
			if (!OpenProcessToken(hProcess.Get(), tokenAccess, &hToken))
			{
				AVDebugWriteLine("Failed getting other process token: " << processId << "/" << GetLastError());
				return NULL;
			}

			//AVDebug.WriteLine("Got other process token: " << processId << "/" << hToken);
			return hToken;
		}
		catch (...)
		{
			AVDebugWriteLine("Failed getting other process token: " << processId);
			return NULL;
		}
	}

	//Get unelevated process token
	inline HANDLE Token_Open_Unelevated()
	{
		try
		{
			//Get unelevated process
			HWND shellWindow = GetShellWindow();
			int unelevatedProcessId = Detail_ProcessIdByWindowHandle(shellWindow);
			if (unelevatedProcessId <= 0)
			{
				AVDebugWriteLine("Invalid unelevated process: " << unelevatedProcessId);
				return NULL;
			}

			//Open unelevated process
			auto hProcess = AVFin(AVFinMethod::CloseHandle, OpenProcess(PROCESS_QUERY_INFORMATION, false, unelevatedProcessId));
			if (hProcess.Get() == NULL)
			{
				AVDebugWriteLine("Failed getting unelevated process: " << GetLastError());
				return NULL;
			}

			//Open unelevated process token
			HANDLE hToken = NULL;
			if (!OpenProcessToken(hProcess.Get(), TOKEN_ALL_ACCESS, &hToken))
			{
				AVDebugWriteLine("Failed getting unelevated process token: " << GetLastError());
				return NULL;
			}

			AVDebugWriteLine("Got unelevated process token: " << hToken);
			return hToken;
		}
		catch (...)
		{
			AVDebugWriteLine("Failed getting unelevated process token.");
			return NULL;
		}
	}
}