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
}