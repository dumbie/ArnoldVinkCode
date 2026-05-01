#pragma once
#include <windows.h>
#include "AVDebug.h"
#include "AVFiles.h"
#include "AVHighResDelay.h"
#include "AVVersion.h"
#include "AVProcesses\ProcessClose.h"
#include "Api\ApiTokens.h"
#include "Api\GitHub\GitHub_Paths.h"
#include "Api\GitHub\GitHub_Version.h"

namespace ArnoldVinkCode
{
	//Update result class
	struct UpdateCheckResult
	{
		bool UpdateFound;
		std::string UpdateVersion;
	};

	//Clean application update files
	inline void UpdateCleanup()
	{
		try
		{
			AVDebugWriteLine(L"Cleaning application update.");

			//Close running application updater
			if (AVProcesses::Close_ProcessByName("Updater.exe", true))
			{
				AVHighResDelay(500);
			}

			//Move new updater executable file
			FileMove(L"Settings\\UpdaterReplace.exe", L"Updater.exe", true);
		}
		catch (...) {}
	}

	//Launch updater and restart application
	inline void UpdateRestart()
	{
		try
		{
			//Launch updater
			AVProcesses::Launch_ApplicationDesktop(L"Updater.exe", L"", L"-ProcessLaunch", true);

			//Exit application
			exit(0);
		}
		catch (...) {}
	}

	//Check for available application update
	inline UpdateCheckResult UpdateCheck(HINSTANCE hInstance, std::string gitUsername, std::string gitRepoName)
	{
		UpdateCheckResult updateCheckResult = UpdateCheckResult{};
		try
		{
			AVDebugWriteLine("Checking for application update: " << gitUsername.c_str() << " / " << gitRepoName.c_str());

			//Get online version
			std::string onlineVersion = string_to_lower(GitHub::GetLatestVersion(gitUsername, gitRepoName));

			//Get current version
			std::string currentVersion = "v" + GetVersionFromResource(hInstance);

			//Check if version matches
			if (!onlineVersion.empty() && currentVersion != onlineVersion)
			{
				AVDebugWriteLine("Application update found: " << onlineVersion.c_str() << " / " << currentVersion.c_str());
				updateCheckResult.UpdateFound = true;
				updateCheckResult.UpdateVersion = onlineVersion;
			}
			else
			{
				AVDebugWriteLine("No application update found.");
			}
		}
		catch (...)
		{
			AVDebugWriteLine("Failed checking application update (Exception)");
		}
		//Return result
		return updateCheckResult;
	}
}