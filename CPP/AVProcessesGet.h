#pragma once
#pragma comment(lib, "ntdll.lib")
#include <winternl.h>
#include <ntstatus.h>
#include <string>
#include <vector>

namespace AVProcesses
{
	//Get process handle by process identifier
	static HANDLE Get_ProcessHandleByProcessId(int targetProcessId)
	{
		try
		{
			return OpenProcess(MAXIMUM_ALLOWED, false, targetProcessId);
		}
		catch (...) {}
		return NULL;
	}
}