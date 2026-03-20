#pragma once
#include <windows.h>
#pragma comment(lib, "ntdll.lib")
#include <winternl.h>
#include <ntstatus.h>
#include <processthreadsapi.h>
#include <string>
#include <vector>

namespace ArnoldVinkCode::AVProcesses
{
	//Remove executable path from commandline
	inline std::wstring Remove_ExePathFromCommandLine(std::wstring targetCommandLine)
	{
		try
		{
			//Check command line
			if (targetCommandLine.empty())
			{
				return targetCommandLine;
			}

			//Remove executable path
			int endIndex = 0;
			bool inQuotes = false;
			for (wchar_t commandChar : targetCommandLine)
			{
				if (commandChar == '"')
				{
					inQuotes = !inQuotes;
				}
				else if (!inQuotes && commandChar == ' ')
				{
					break;
				}
				endIndex++;
			}
			targetCommandLine = targetCommandLine.substr(endIndex);
		}
		catch (...) {}
		return targetCommandLine;
	}

	//Get process parent id by process handle
	inline int Detail_ProcessParentIdByProcessHandle(HANDLE targetProcessHandle)
	{
		try
		{
			__PROCESS_BASIC_INFORMATION32 basicInformation{};
			NTSTATUS queryResult = NtQueryInformationProcess(targetProcessHandle, ProcessBasicInformation, &basicInformation, sizeof(basicInformation), NULL);
			if (!NT_SUCCESS(queryResult))
			{
				AVDebugWriteLine("Failed to get parent process id: " << targetProcessHandle << "/Query failed.");
				return 0;
			}
			else
			{
				return (int)basicInformation.InheritedFromUniqueProcessId;
			}
		}
		catch (...)
		{
			//AVDebugWriteLine("Failed to get parent processid: " << targetProcessHandle);
			return 0;
		}
	}

	/// <summary>
	/// Get process parameter by process handle
	/// </summary>
	/// <summary>Process handle with VM_READ access is required.</summary>
	inline std::wstring Detail_ParameterByProcessHandle(HANDLE targetProcessHandle, ProcessParameterOptions pOption)
	{
		std::wstring parameterString = L"";
		try
		{
			//Check target process handle
			if (targetProcessHandle == NULL)
			{
				AVDebugWriteLine("GetApplicationParameter invalid process handle.");
				return parameterString;
			}

			//Check application architecture
			BOOL targetIsWow64 = FALSE;
			BOOL currentIsWow64 = FALSE;
			IsWow64Process(targetProcessHandle, &targetIsWow64);
			IsWow64Process(GetCurrentProcess(), &currentIsWow64);

			//Read application parameter
			if (currentIsWow64 && targetIsWow64)
			{
				//AVDebugWriteLine("GetApplicationParameter (32) target: " << targetIsWow64 << " current: " << currentIsWow64);
				parameterString = GetApplicationParameter32(targetProcessHandle, pOption);
			}
			else if (currentIsWow64 && !targetIsWow64)
			{
				//AVDebugWriteLine("GetApplicationParameter (WOW64) target: " << targetIsWow64 << " current: " << currentIsWow64);
				parameterString = GetApplicationParameterWOW64(targetProcessHandle, pOption);
			}
			else if (!currentIsWow64 && targetIsWow64)
			{
				//AVDebugWriteLine("GetApplicationParameter (64) target: " << targetIsWow64 << " current: " << currentIsWow64);
				parameterString = GetApplicationParameter64(targetProcessHandle, pOption);
			}
			else if (!currentIsWow64 && !targetIsWow64)
			{
				//AVDebugWriteLine("GetApplicationParameter (32) target: " << targetIsWow64 << " current: " << currentIsWow64);
				parameterString = GetApplicationParameter32(targetProcessHandle, pOption);
			}
			else
			{
				AVDebugWriteLine("GetApplicationParameter unknown architecture.");
				return parameterString;
			}

			//Remove executable path from commandline
			if (pOption == ProcessParameterOptions::CommandLine)
			{
				parameterString = Remove_ExePathFromCommandLine(parameterString);
			}

			//Trim and return string
			return wstring_trim(parameterString);
		}
		catch (...)
		{
			AVDebugWriteLine("Failed to get GetApplicationParameter.");
			return parameterString;
		}
	}
}