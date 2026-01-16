#pragma once
#include <windows.h>
#pragma comment(lib, "ntdll.lib")
#include <winternl.h>
#include <ntstatus.h>
#include <processthreadsapi.h>
#include <string>
#include <vector>
#include "AVString.h"
#include "AVFinally.h"

namespace ArnoldVinkCode::AVProcesses
{
	//Enumerators
	enum ProcessMultiActions
	{
		Launch,
		Close,
		CloseAll,
		Restart,
		RestartAll,
		Select,
		NoAction,
		Cancel
	};

	//Classes
	class ProcessMultiAction
	{
	public:
		ProcessMultiActions Action = ProcessMultiActions::NoAction;
		//ProcessMulti ProcessMulti = NULL;
	};

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

		HWND WindowHandleMain(bool checkVisibility = true)
		{
			HWND windowHandleMain = NULL;
			try
			{
				//Check process name
				if (!Check_MainWindowProcessNameIsValid(ExeName()))
				{
					return windowHandleMain;
				}

				//Get window handle
				windowHandleMain = Get_WindowHandleMainByProcessId(Identifier(), checkVisibility);
			}
			catch (...) {}
			return windowHandleMain;
		};

		bool Validate()
		{
			try
			{
				return Identifier() > 0 && !ExePath().empty();
			}
			catch (...) {}
			return false;
		};

		void Debug()
		{
			try
			{
				AVDebugWriteLine("Identifier: " << Identifier());
				AVDebugWriteLine("IdentifierParent: " << IdentifierParent());
				AVDebugWriteLine("Handle: " << Handle());
				//AVDebugWriteLine("Type: " << Type);
				//AVDebugWriteLine("AdminAccess: " << AccessStatus.AdminAccess);
				//AVDebugWriteLine("Responding: " << Responding);
				//AVDebugWriteLine("Priority: " << Priority);
				//AVDebugWriteLine("AppUserModelId: " << AppUserModelId);
				AVDebugWriteLine("ExeName: " << ExeName().c_str());
				//AVDebugWriteLine("ExeNameNoExt: " << ExeNameNoExt);
				AVDebugWriteLine("ExePath: " << ExePath().c_str());
				//AVDebugWriteLine("WorkPath: " << WorkPath);
				//AVDebugWriteLine("Argument: " << Argument);
				AVDebugWriteLine("WindowHandleMain: " << WindowHandleMain());
				//AVDebugWriteLine("WindowHandles: " << WindowHandles.Count);
				//AVDebugWriteLine("WindowClassNameMain: " << WindowClassNameMain);
				//AVDebugWriteLine("WindowTitleMain: " << WindowTitleMain);
				//AVDebugWriteLine("StartTime: " << StartTime);
				//AVDebugWriteLine("RunTime: " << RunTime);
				//AVDebugWriteLine("Suspended: " << Suspended);
				//AVDebugWriteLine("Threads: " << Threads.Count);
				//AVDebugWriteLine("AppPackageName: " << AppPackage.DisplayName);
				//AVDebugWriteLine("AppxDetailsName: " << AppxDetails.DisplayName);
			}
			catch (...) {}
		};

		void Cache()
		{
			try
			{
				auto c1 = Identifier();
				auto c2 = IdentifierParent();
				auto c3 = Handle();
				//auto c4 = Type;
				//auto c5 = AccessStatus;
				//auto c6 = Responding;
				//auto c7 = Priority;
				//auto c8 = AppUserModelId;
				auto c9 = ExeName();
				//auto c10 = ExeNameNoExt;
				auto c11 = ExePath();
				//auto c12 = WorkPath;
				//auto c13 = Argument;
				auto c14 = WindowHandleMain();
				auto c15 = WindowHandles();
				//auto c16 = WindowClassNameMain;
				//auto c17 = WindowTitleMain;
				//auto c18 = StartTime;
				//auto c19 = RunTime;
				//auto c20 = Suspended;
				//auto c21 = Threads;
				//auto c22 = AppPackage;
				//auto c23 = AppxDetails;
			}
			catch (...) {}
		};

		~ProcessMulti() { Dispose(); }
		void Dispose()
		{
			try
			{
				if (CachedHandle != NULL)
				{
					CloseHandle(CachedHandle);
				}
			}
			catch (...) {}
		};
	};
}