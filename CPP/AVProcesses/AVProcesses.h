#pragma once
#include <windows.h>
#pragma comment(lib, "ntdll.lib")
#include <winternl.h>
#include <ntstatus.h>
#include <processthreadsapi.h>
#include <string>
#include <vector>
#include "AVString.h"

namespace ArnoldVinkCode::AVProcesses
{
	//Query system process information
	inline PSYSTEM_PROCESS_INFORMATION Query_SystemProcessInformation()
	{
		ULONG systemOffset = 0;
		PSYSTEM_PROCESS_INFORMATION systemInfo = NULL;
		try
		{
			while (true)
			{
				try
				{
					ULONG systemLength = 0;
					systemInfo = (PSYSTEM_PROCESS_INFORMATION)malloc(systemOffset);
					NTSTATUS queryResult = NtQuerySystemInformation(SYSTEM_INFORMATION_CLASS::SystemProcessInformation, systemInfo, systemOffset, &systemLength);
					if (queryResult == STATUS_INFO_LENGTH_MISMATCH)
					{
						systemOffset = std::max<ULONG>(systemOffset, systemLength);
						free(systemInfo);
					}
					else if (queryResult == STATUS_SUCCESS)
					{
						break;
					}
				}
				catch (...) {}
			}
		}
		catch (...) {}
		return systemInfo;
	}

	//Classes
	class ProcessMulti
	{
	private:
		int CachedIdentifier = 0;
		int CachedIdentifierParent = 0;
		HANDLE CachedHandle = NULL;
		std::string CachedExePath = "";
		std::string CachedExeName = "";

	public:
		ProcessMulti(int identifier, int identifierParent, std::string exeName)
		{
			CachedIdentifier = identifier;
			CachedIdentifierParent = identifierParent;
			CachedExeName = exeName;
		};

		int Identifier()
		{
			return CachedIdentifier;
		};

		int IdentifierParent()
		{
			return CachedIdentifierParent;
		};

		HANDLE Handle()
		{
			try
			{
				if (CachedHandle == NULL)
				{
					CachedHandle = Get_ProcessHandleByProcessId(Identifier());
				}
			}
			catch (...) {}
			return CachedHandle;
		};

		std::string ExePath()
		{
			try
			{
				if (CachedExePath.empty())
				{
					CachedExePath = Detail_ExecutablePathByProcessHandle(Handle());
				}
			}
			catch (...) {}
			return CachedExePath;
		};

		std::string ExeName()
		{
			return CachedExeName;
		};

		std::vector<HWND> WindowHandles()
		{
			try
			{
				return Get_WindowHandlesByProcessId(Identifier());
			}
			catch (...) {}
			return std::vector<HWND>();
		};

		HWND WindowHandleMain()
		{
			HWND windowHandleMain = NULL;
			try
			{
				windowHandleMain = Detail_WindowHandleMainByProcessId(Identifier());
			}
			catch (...) {}
			return windowHandleMain;
		};
	};

	//Get all running processes multi
	inline std::vector<ProcessMulti> Get_ProcessesMultiAll()
	{
		std::vector<ProcessMulti> listProcessMulti;
		try
		{
			//Query process information
			PSYSTEM_PROCESS_INFORMATION spi = Query_SystemProcessInformation();

			//Loop process information
			while (true)
			{
				try
				{
					//Get executable name
					std::wstring exeNameW = std::wstring(spi->ImageName.Buffer, spi->ImageName.Length / sizeof(WCHAR));
					std::string exeNameA = wstring_to_string(exeNameW);

					//Add multi process to list
					ProcessMulti processMulti = ProcessMulti((int)spi->UniqueProcessId, (int)spi->Reserved2, exeNameA);
					listProcessMulti.push_back(processMulti);

					//Move to next process
					if (spi->NextEntryOffset != 0)
					{
						spi = (PSYSTEM_PROCESS_INFORMATION)((BYTE*)spi + spi->NextEntryOffset);
					}
					else
					{
						break;
					}
				}
				catch (...) {}
			}
		}
		catch (...) {}
		return listProcessMulti;
	}

	//Get multi process by executable name
	inline std::vector<ProcessMulti> Get_ProcessesMultiByName(std::string executableName)
	{
		std::vector<ProcessMulti> listProcessMulti;
		try
		{
			//List all processes
			std::vector<ProcessMulti> processList = Get_ProcessesMultiAll();

			//Lowercase executable name
			std::string exeNameLowerSearch = string_to_lower(executableName);

			//Look for executable name
			for (ProcessMulti process : processList)
			{
				//Lowercase executable name
				std::string exeNameLowerProcess = string_to_lower(process.ExeName());

				//Add multi process to list
				if (exeNameLowerProcess == exeNameLowerSearch)
				{
					listProcessMulti.push_back(process);
				}
			}
		}
		catch (...) {}
		return listProcessMulti;
	}
}