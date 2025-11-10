#pragma once
#include "AVDebug.h"

//Check startup shortcut
static bool StartupShortcutCheck(std::wstring appName)
{
	try
	{
		//Get startup path
		std::wstring startupFolderPath = PathGetFolderKnown(FOLDERID_Startup);
		std::wstring startupFilePath = PathMerge(startupFolderPath, appName + L".url");

		//Return result
		return FileExists(startupFilePath);
	}
	catch (...)
	{
		//Return result
		return false;
	}
}

//Create or remove startup shortcut
static bool StartupShortcutManage(std::wstring appName, bool useLauncher)
{
	try
	{
		//Get startup path
		std::wstring startupFolderPath = PathGetFolderKnown(FOLDERID_Startup);
		std::wstring startupFilePath = PathMerge(startupFolderPath, appName + L".url");


		//Set shortcut details

		std::wstring targetFilePath = PathGetExecutableFile();
		std::wstring targetExecutableFile = PathGetFileName(targetFilePath);
		
		AVDebugWriteLine(startupFilePath);
		AVDebugWriteLine(targetFilePath);
		AVDebugWriteLine(targetExecutableFile);

		//Check launcher executable
		if (useLauncher)
		{
		    std::wstring executableLauncher = L"";
		    if (FileExists(appName + L"-Launcher.exe"))
		    {
		        executableLauncher = appName + L"-Launcher.exe";
		    }
		    else if (FileExists(L"Launcher.exe"))
		    {
		        executableLauncher = L"Launcher.exe";
		    }

		    if (!executableLauncher.empty())
		    {
		        targetFilePath = string_replace(targetFilePath, targetExecutableFile, executableLauncher);
		    }
		}

		//Check if the shortcut already exists
		if (!FileExists(startupFilePath))
		{
			AVDebugWriteLine(L"Adding application to Windows startup: " + targetFilePath);

			//Convert strings
			std::string startupFilePathA = wstring_to_string(startupFilePath);
			std::string targetFilePathA = wstring_to_string(targetFilePath);
			std::string shortcutText = "[InternetShortcut]";
			shortcutText += "\nURL=" + targetFilePathA;
			shortcutText += "\nIconFile=" + targetFilePathA;
			shortcutText += "\nIconIndex=0";

			//Write shortcut file
			string_to_file(startupFilePathA, shortcutText);
		}
		else
		{
			AVDebugWriteLine(L"Removing application from Windows startup: " + targetFilePath);

			//Delete shortcut file
		    FileDelete(startupFilePath);
		}

		return true;
	}
	catch (...)
	{
		AVDebugWriteLine(L"Failed creating startup shortcut.");
		return false;
	}
}