#pragma once
#include <windows.h>
#include <filesystem>
#include "ProcessCheck.h"

namespace ArnoldVinkCode::AVProcesses
{
	/// <summary>
	/// Launch application using ShellExecute
	/// </summary>
	inline bool Launch_ShellExecute(std::wstring exePath, std::wstring workPath, std::wstring arguments, bool asAdmin)
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
				std::vector<std::wstring> fileExecutables{ L".exe", L".bat", L".cmd", L".com", L".pif" };
				std::wstring fileExtension = std::filesystem::path(exePath).extension().wstring();
				std::wstring fileExtensionLower = wstring_to_lower(fileExtension);
				if (!array_contains(fileExecutables, fileExtensionLower))
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
					AVDebugWriteLine(L"Workpath is empty or missing, using exepath: " + fileFolderPath);
				}
			}

			//Shell execute process
			bool shellExecuteResult = false;
			if (asAdmin)
			{
				//Shell execute inherit user
				AVDebugWriteLine(L"Shell executing with inherited access: " + exePath);
				shellExecuteResult = ShellExecuteExW(&shellExecuteInfo);
			}
			else
			{
				//Shell execute normal user
				AVDebugWriteLine(L"Shell executing with user access: " + exePath);
				shellExecuteResult = ShellExecuteUser(shellExecuteInfo);
			}

			//Check execute result
			if (!shellExecuteResult)
			{
				AVDebugWriteLine(L"Shell execute failed: " + exePath);
				return false;
			}
			else
			{
				AVDebugWriteLine(L"Shell execute succeeded: " + exePath);
				return true;
			}
		}
		catch (...)
		{
			AVDebugWriteLine(L"Shell execute failed: " + exePath);
			return false;
		}
	}

	///// <summary>
	///// Launch UWP or Win32Store application
	///// </summary>
	//public static bool Launch_UwpApplication(string appUserModelId, string arguments)
	//{
	//    try
	//    {
	//        //Show launching message
	//        AVDebug.WriteLine("Launching UWP or Win32Store application: " + appUserModelId + "/" + arguments);

	//        //Start the process
	//        UWPActivationManager UWPActivationManager = new UWPActivationManager();
	//        UWPActivationManager.ActivateApplication(appUserModelId, arguments, UWP_ACTIVATEOPTIONS.AO_NONE, out int processId);

	//        //Return process id
	//        AVDebug.WriteLine("Launched UWP or Win32Store process identifier: " + processId);
	//        return processId > 0;
	//    }
	//    catch (...)
	//    {
	//        AVDebug.WriteLine("Failed launching UWP or Win32Store: " + appUserModelId + "/" + ex.Message);
	//        return false;
	//    }
	//}
}