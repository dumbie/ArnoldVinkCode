#pragma once
#include <windows.h>

namespace ArnoldVinkCode::AVProcesses
{
	//Get process handle by process identifier
	inline HANDLE Get_ProcessHandleByProcessId(int targetProcessId)
	{
		try
		{
			return OpenProcess(MAXIMUM_ALLOWED, false, targetProcessId);
		}
		catch (...) {}
		return NULL;
	}
}