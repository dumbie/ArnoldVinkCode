#pragma once
#include <windows.h>

namespace ArnoldVinkCode::AVProcesses
{
	//Close process by identifier
	inline bool Close_ProcessByProcessId(int targetProcessId)
	{
		try
		{
			if (GetCurrentProcessId() == targetProcessId)
			{
				AVDebugWriteLine("Prevented closing process by id: " << targetProcessId << "/Process is current application.");
				return false;
			}

			auto closeProcess = AVFin(AVFinMethod::CloseHandle, OpenProcess(PROCESS_TERMINATE, false, targetProcessId));
			if (closeProcess.Get() == nullptr)
			{
				AVDebugWriteLine("Failed closing process by id: " << targetProcessId << "/Process not found.");
				return false;
			}
			else
			{
				bool processClosed = TerminateProcess(closeProcess.Get(), 0);
				AVDebugWriteLine("Closed process by id: " << targetProcessId << "/" << processClosed);
				return processClosed;
			}
		}
		catch (...)
		{
			AVDebugWriteLine("Failed closing process by id: " << targetProcessId);
			return false;
		}
	}

	//Close process tree by identifier
	inline bool Close_ProcessTreeByProcessId(int targetProcessId)
	{
		try
		{
			//Close child processes
			for (AVProcess childProcess : Get_ProcessAll())
			{
				try
				{
					if (childProcess.IdentifierParent() == targetProcessId)
					{
						Close_ProcessByProcessId(childProcess.Identifier());
					}
				}
				catch (...) {}
			}

			//Close parent process
			Close_ProcessByProcessId(targetProcessId);

			AVDebugWriteLine("Closed process tree by id: " << targetProcessId);
			return true;
		}
		catch (...)
		{
			AVDebugWriteLine("Failed closing process tree by id: " << targetProcessId);
			return false;
		}
	}

	//Close process by name
	inline bool Close_ProcessByName(std::string targetProcessName, bool exactName)
	{
		try
		{
			bool processClosed = false;
			for (AVProcess foundProcesses : Get_ProcessByName(targetProcessName, exactName))
			{
				try
				{
					if (Close_ProcessTreeByProcessId(foundProcesses.Identifier()))
					{
						processClosed = true;
					}
				}
				catch (...) {}
			}

			AVDebugWriteLine("Closed process by name: " << targetProcessName.c_str() << "/" << processClosed);
			return processClosed;
		}
		catch (...)
		{
			AVDebugWriteLine("Failed closing process by name: " << targetProcessName.c_str());
			return false;
		}
	}

	//Close process by executable path
	inline bool Close_ProcessByExecutablePath(std::string targetExecutablePath)
	{
		try
		{
			bool processClosed = false;
			for (AVProcess foundProcesses : Get_ProcessByExecutablePath(targetExecutablePath))
			{
				try
				{
					if (Close_ProcessTreeByProcessId(foundProcesses.Identifier()))
					{
						processClosed = true;
					}
				}
				catch (...) {}
			}

			AVDebugWriteLine("Closed process by executable path: " << targetExecutablePath.c_str() << "/" << processClosed);
			return processClosed;
		}
		catch (...)
		{
			AVDebugWriteLine("Failed closing process by executable path: " << targetExecutablePath.c_str());
			return false;
		}
	}

	//Close process by AppUserModelId
	inline bool Close_ProcessByAppUserModelId(std::string targetAppUserModelId)
	{
		try
		{
			bool processClosed = false;
			for (AVProcess foundProcesses : Get_ProcessByAppUserModelId(targetAppUserModelId))
			{
				try
				{
					if (Close_ProcessTreeByProcessId(foundProcesses.Identifier()))
					{
						processClosed = true;
					}
				}
				catch (...) {}
			}

			AVDebugWriteLine("Closed process by AppUserModelId: " << targetAppUserModelId.c_str() << "/" + processClosed);
			return processClosed;
		}
		catch (...)
		{
			AVDebugWriteLine("Failed closing process by AppUserModelId: " << targetAppUserModelId.c_str());
			return false;
		}
	}

	//Close process by window message
	inline bool Close_ProcessByWindowMessage(HWND targetWindowHandle)
	{
		try
		{
			PostMessage(targetWindowHandle, WM_CLOSE, 0, 0);
			PostMessage(targetWindowHandle, WM_QUIT, 0, 0);

			AVDebugWriteLine("Closed process by window message: " << targetWindowHandle);
			return true;
		}
		catch (...)
		{
			AVDebugWriteLine("Failed closing process by window message: " << targetWindowHandle);
			return false;
		}
	}
}