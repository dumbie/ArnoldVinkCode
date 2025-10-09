#pragma once
#pragma comment(lib, "ntdll.lib")
#include <winternl.h>
#include <ntstatus.h>
#include <string>
#include <vector>

//Query system process information
static PSYSTEM_PROCESS_INFORMATION Query_SystemProcessInformation()
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
				std::vector<BYTE> spiBuffer(systemOffset);
				NTSTATUS status = NtQuerySystemInformation(SYSTEM_INFORMATION_CLASS::SystemProcessInformation, spiBuffer.data(), systemOffset, &systemLength);
				if (status == STATUS_INFO_LENGTH_MISMATCH)
				{
					systemOffset = std::max<ULONG>(systemOffset, systemLength);
				}
				else if (status == STATUS_SUCCESS)
				{
					//Cast SystemProcessInformation
					systemInfo = reinterpret_cast<PSYSTEM_PROCESS_INFORMATION>(spiBuffer.data());
					break;
				}
			}
			catch (...) {}
		}
	}
	catch (...) {}
	return systemInfo;
}

//Get process handle by process identifier
static HANDLE Get_ProcessHandleByProcessId(int targetProcessId)
{
	try
	{
		return OpenProcess(PROCESS_QUERY_INFORMATION, false, targetProcessId);
	}
	catch (...) {}
	return NULL;
}

//Get full exe path by process handle
static std::string Detail_ExecutablePathByProcessHandle(HANDLE targetProcessHandle)
{
	try
	{
		CHAR buffer[1024];
		DWORD bufferSize = sizeof(buffer);
		if (QueryFullProcessImageNameA(targetProcessHandle, 0, buffer, &bufferSize))
		{
			return std::string(buffer);
		}
	}
	catch (...) {}
	return "";
}

//Classes
class ProcessMulti
{
private:
	HANDLE CachedHandle = NULL;
	std::string CachedExePath = "";

public:
	int CachedIdentifier = 0;
	int CachedIdentifierParent = 0;

	ProcessMulti(int identifier, int identifierParent)
	{
		CachedIdentifier = identifier;
		CachedIdentifierParent = identifierParent;
	};

	HANDLE Handle()
	{
		try
		{
			if (CachedHandle == NULL)
			{
				CachedHandle = Get_ProcessHandleByProcessId(CachedIdentifier);
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
};

//List processes
static std::vector<ProcessMulti> ListProcesses()
{
	std::vector<ProcessMulti> listProcessMulti;
	try
	{
		//Query process information
		PSYSTEM_PROCESS_INFORMATION spi = Query_SystemProcessInformation();

		//Loop process information
		while (true)
		{
			//Add multi process to list
			ProcessMulti processMulti = ProcessMulti((int)spi->UniqueProcessId, (int)spi->Reserved2);
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
	}
	catch (...) {}
	return listProcessMulti;
};