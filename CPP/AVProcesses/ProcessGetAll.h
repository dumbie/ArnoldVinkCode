#pragma once
#include <windows.h>
#include <string>
#include <vector>
#include "..\AVString.h"
#include "..\AVFinally.h"

namespace ArnoldVinkCode::AVProcesses
{
	//Get all running processes
	inline std::vector<AVProcess> Get_ProcessAll(int targetProcessId)
	{
		std::vector<AVProcess> listProcess{};
		try
		{
			//AVDebugWriteLine("Getting all processes.");

			//Query process information
			auto spiQueryBuffer = AVFin(AVFinMethod::FreeMarshal, Query_SystemProcessInformation());
			if (spiQueryBuffer.Get() == nullptr)
			{
				AVDebugWriteLine("Failed getting all processes: query failed.");
				return listProcess;
			}

			//Loop process information
			ULONG systemProcessOffset = 0;
			while (true)
			{
				try
				{
					//Get process information
					PSYSTEM_PROCESS_INFORMATION systemProcess = (PSYSTEM_PROCESS_INFORMATION)((BYTE*)spiQueryBuffer.Get() + systemProcessOffset);

					//Get executable name
					std::wstring exeNameW = std::wstring(systemProcess->ImageName.Buffer, systemProcess->ImageName.Length / sizeof(WCHAR));
					std::string exeNameA = wstring_to_string(exeNameW);

					//Add process to list
					AVProcess process = AVProcess((int)systemProcess->UniqueProcessId, (int)systemProcess->Reserved2, exeNameA);

					//Check target identifier
					if (targetProcessId >= 0)
					{
						if (targetProcessId == process.Identifier())
						{
							return { process };
						}
					}
					else
					{
						listProcess.push_back(process);
					}

					//Move to next process
					if (systemProcess->NextEntryOffset != 0)
					{
						systemProcessOffset += systemProcess->NextEntryOffset;
					}
					else
					{
						break;
					}
				}
				catch (...) {}
			}
		}
		catch (...)
		{
			AVDebugWriteLine("Failed getting all processes.");
		}
		return listProcess;
	}

	//Get process by process id
	inline std::optional<AVProcess> Get_ProcessByProcessId(int targetProcessId)
	{
		try
		{
			return AVProcess(targetProcessId, 0, "");
		}
		catch (...)
		{
			AVDebugWriteLine("Failed get process by process id: " << targetProcessId);
			return std::nullopt;
		}
	}

	//Get process for current process
	inline std::optional<AVProcess> Get_ProcessCurrent()
	{
		try
		{
			return Get_ProcessByProcessId(GetCurrentProcessId());
		}
		catch (...)
		{
			AVDebugWriteLine("Failed to get process for current process.");
			return std::nullopt;
		}
	}

	/// <summary>
	/// Get processes by AppUserModelId
	/// </summary>
	/// <param name="targetAppUserModelId">UWP or Win32Store AppUserModelId</param>
	inline std::vector<AVProcess> Get_ProcessByAppUserModelId(std::string targetAppUserModelId)
	{
		//AVDebugWriteLine("Getting processes by AppUserModelId: " << targetName);
		std::vector<AVProcess> foundProcesses{};
		try
		{
			std::string targetAppUserModelIdLower = string_to_lower(targetAppUserModelId);
			for (AVProcess checkProcess : Get_ProcessAll())
			{
				try
				{
					std::string foundAppUserModelIdLower = string_to_lower(checkProcess.AppUserModelId());
					if (foundAppUserModelIdLower == targetAppUserModelIdLower)
					{
						foundProcesses.push_back(checkProcess);
					}
				}
				catch (...) {}
			}
		}
		catch (...)
		{
			AVDebugWriteLine("Failed to get processes by AppUserModelId.");
		}
		return foundProcesses;
	}

	//Get process by window handle
	inline std::optional<AVProcess> Get_ProcessByWindowHandle(HWND targetWindowHandle)
	{
		try
		{
			//Check if window handle is UWP or Win32Store application
			std::string appUserModelId = Detail_AppUserModelIdByWindowHandle(targetWindowHandle);
			if (!appUserModelId.empty())
			{
				return Get_ProcessByAppUserModelId(appUserModelId)[0];
			}
			else
			{
				int processId = Detail_ProcessIdByWindowHandle(targetWindowHandle);
				return Get_ProcessByProcessId(processId);
			}
		}
		catch (...)
		{
			AVDebugWriteLine("Failed to get process by window handle.");
			return std::nullopt;
		}
	}

	/// <summary>
	/// Get processes by name
	/// </summary>
	/// <param name="targetName">Process name or executable name</param>
	/// <param name="exactName">Search for exact process name</param>
	inline std::vector<AVProcess> Get_ProcessByName(std::string targetName, bool exactName)
	{
		//AVDebugWriteLine("Getting processes by name: " << targetName.c_str());
		std::vector<AVProcess> foundProcesses{};
		try
		{
			//Lowercase executable name
			std::string targetNameLower = string_to_lower(targetName);

			//Look for executable name
			for (AVProcess checkProcess : Get_ProcessAll())
			{
				try
				{
					//Lowercase executable name
					std::string foundNameLower = string_to_lower(checkProcess.ExeName());
					std::string foundNameNoExtLower = string_to_lower(checkProcess.ExeNameNoExt());

					//Add process to list
					if (exactName)
					{
						if (foundNameLower == targetNameLower || foundNameNoExtLower == targetNameLower)
						{
							foundProcesses.push_back(checkProcess);
						}
					}
					else
					{
						if (string_contains(foundNameLower, targetNameLower) || string_contains(foundNameNoExtLower, targetNameLower))
						{
							foundProcesses.push_back(checkProcess);
						}
					}
				}
				catch (...) {}
			}
		}
		catch (...)
		{
			AVDebugWriteLine("Failed to get processes by name.");
		}
		return foundProcesses;
	}

	/// <summary>
	/// Get processes by executable path
	/// </summary>
	/// <param name="targetExecutablePath">Process executable path</param>
	inline std::vector<AVProcess> Get_ProcessByExecutablePath(std::string targetExecutablePath)
	{
		//AVDebugWriteLine("Getting processes by executable path: " << targetExecutablePath);
		std::vector<AVProcess> foundProcesses;
		try
		{
			std::string targetExecutablePathLower = string_to_lower(targetExecutablePath);
			for (AVProcess checkProcess : Get_ProcessAll())
			{
				try
				{
					std::string foundExecutablePathLower = string_to_lower(checkProcess.ExePath());
					if (foundExecutablePathLower == targetExecutablePathLower)
					{
						foundProcesses.push_back(checkProcess);
					}
				}
				catch (...) {}
			}
		}
		catch (...)
		{
			AVDebugWriteLine("Failed to get processes by executable path.");
		}
		return foundProcesses;
	}

	/// <summary>
	/// Get processes by window title
	/// </summary>
	/// <param name="targetWindowTitle">Search for window title</param>
	/// <param name="exactName">Search for exact window title</param>
	inline std::vector<AVProcess> Get_ProcessByWindowTitle(std::string targetWindowTitle, bool exactName)
	{
		//AVDebugWriteLine("Getting processes by window title: " << targetWindowTitle);
		std::vector<AVProcess> foundProcesses;
		try
		{
			std::string targetWindowTitleLower = string_to_lower(targetWindowTitle);
			for (AVProcess checkProcess : Get_ProcessAll())
			{
				try
				{
					std::string foundWindowTitleLower = string_to_lower(checkProcess.WindowTitleMain());
					if (exactName)
					{
						if (foundWindowTitleLower == targetWindowTitleLower)
						{
							foundProcesses.push_back(checkProcess);
						}
					}
					else
					{
						if (string_contains(foundWindowTitleLower, targetWindowTitleLower))
						{
							foundProcesses.push_back(checkProcess);
						}
					}
				}
				catch (...) {}
			}
		}
		catch (...)
		{
			AVDebugWriteLine("Failed to get processes by window title.");
		}
		return foundProcesses;
	}
}