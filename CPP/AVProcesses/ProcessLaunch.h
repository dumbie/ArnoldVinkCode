#pragma once
#include <windows.h>
#include <filesystem>
#include "ProcessCheck.h"

namespace ArnoldVinkCode::AVProcesses
{
	/// <summary>
	/// Launch desktop application using ShellExecute
	/// </summary>
	inline bool Launch_ApplicationDesktop(std::wstring exePath, std::wstring workPath, std::wstring arguments, bool asAdmin, bool waitForExit)
	{
		try
		{
			//Check execute path
			if (exePath.empty())
			{
				AVDebugWriteLine("Shell execute failed: execute path is empty.");
				return false;
			}

			//Check file executable extension
			if (asAdmin)
			{
				std::vector<std::wstring> fileExecutables{ L".exe", L".bat", L".cmd", L".com", L".pif", L".bin" };
				std::wstring fileExtension = wstring_to_lower(std::filesystem::path(exePath).extension().wstring());
				if (!array_contains(fileExecutables, fileExtension))
				{
					AVDebugWriteLine("No executable detected, running as normal user.");
					asAdmin = false;
				}
			}

			//Set shell execute info
			SHELLEXECUTEINFOW shellExecuteInfo{};
			shellExecuteInfo.cbSize = sizeof(shellExecuteInfo);
			shellExecuteInfo.nShow = SW_SHOW;
			shellExecuteInfo.fMask = SEE_MASK_NOCLOSEPROCESS;
			shellExecuteInfo.lpVerb = asAdmin ? L"runas" : L"open";
			shellExecuteInfo.lpFile = exePath.c_str();

			//Check for url protocol
			if (!Check_PathUrlProtocol(exePath))
			{
				if (!arguments.empty())
				{
					shellExecuteInfo.lpParameters = arguments.c_str();
				}
				if (!workPath.empty() && FolderExists(workPath))
				{
					shellExecuteInfo.lpDirectory = workPath.c_str();
				}
				else
				{
					std::wstring fileFolderPath = std::filesystem::path(exePath).parent_path().wstring();
					shellExecuteInfo.lpDirectory = fileFolderPath.c_str();
					AVDebugWriteLine(L"Workpath is empty or missing, using exepath: " << fileFolderPath);
				}
			}

			//Shell execute process
			bool shellExecuteResult = false;
			if (asAdmin)
			{
				//Shell execute inherit user
				AVDebugWriteLine(L"Shell executing with inherited access: " << exePath);
				shellExecuteResult = ShellExecuteExW(&shellExecuteInfo);
			}
			else
			{
				//Shell execute normal user
				AVDebugWriteLine(L"Shell executing with user access: " << exePath);
				shellExecuteResult = ShellExecuteUser(shellExecuteInfo);
			}

			//Wait for process exit
			if (waitForExit && shellExecuteInfo.hProcess != NULL)
			{
				AVDebugWriteLine("Waiting for process to exit.");
				WaitForSingleObject(shellExecuteInfo.hProcess, INFINITE);
			}

			//Check execute result
			if (!shellExecuteResult)
			{
				AVDebugWriteLine(L"Shell execute failed: " << exePath);
				return false;
			}
			else
			{
				AVDebugWriteLine(L"Shell execute succeeded: " << exePath);
				return true;
			}
		}
		catch (...)
		{
			AVDebugWriteLine(L"Shell execute failed: " << exePath);
			return false;
		}
	}

	/// <summary>
	/// Launch UWP or Win32Store application
	/// </summary>
	inline bool Launch_ApplicationUwp(std::wstring appUserModelId, std::wstring arguments)
	{
		try
		{
			//Show launching message
			AVDebugWriteLine(L"Launching UWP or Win32Store application: " << appUserModelId << L"/" << arguments);

			//Initialize COM library
			HRESULT hResult = CoInitialize(NULL);
			if (!SUCCEEDED(hResult))
			{
				AVDebugWriteLine(L"Failed to initialize COM library.");
				return false;
			}

			//Create activation manager
			auto iActivationManager = AVFin<IApplicationActivationManager*>(AVFinMethod::ReleaseInterface);
			hResult = CoCreateInstance(CLSID_ApplicationActivationManager, NULL, CLSCTX_ALL, IID_IApplicationActivationManager, (void**)&iActivationManager.Get());
			if (!SUCCEEDED(hResult))
			{
				AVDebugWriteLine(L"Failed to create activation manager instance.");
				return false;
			}

			//Launch UWP application
			DWORD processId = 0;
			hResult = iActivationManager.Get()->ActivateApplication(appUserModelId.c_str(), arguments.c_str(), AO_NONE, &processId);

			//Return process id
			AVDebugWriteLine(L"Launched UWP or Win32Store process identifier: " << processId);
			return processId > 0;
		}
		catch (...)
		{
			AVDebugWriteLine(L"Failed launching UWP or Win32Store: " << appUserModelId);
			return false;
		}
	}
}