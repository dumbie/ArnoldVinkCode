#pragma once
#include <windows.h>
#pragma comment(lib, "ntdll.lib")
#include <winternl.h>
#include <ntstatus.h>
#include <processthreadsapi.h>
#include <string>
#include <vector>
#include "..\..\AVTime.h"
#include "..\..\AVString.h"
#include "..\..\AVFinally.h"

namespace ArnoldVinkCode::AVProcesses
{
	class ProcessMulti
	{
	private:
		int CachedIdentifier = 0;
		int CachedIdentifierParent = 0;
		HANDLE CachedHandle = NULL;
		ProcessType CachedType = ProcessType::Unknown;
		std::string CachedAppUserModelId = "";
		std::string CachedExeName = "";
		std::string CachedExeNameNoExt = "";
		std::string CachedExePath = "";
		std::string CustomWindowTitleMain = "";
		SYSTEMTIME CachedStartTime{};

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

		ProcessType Type()
		{
			try
			{
				if (CachedType == ProcessType::Unknown)
				{
					if (!AppUserModelId().empty())
					{
						if (Check_WindowClassNameIsUwpApp(WindowClassNameMain()))
						{
							CachedType = ProcessType::UWP;
						}
						else
						{
							CachedType = ProcessType::Win32Store;
						}
					}
					else
					{
						CachedType = ProcessType::Win32;
					}
				}
			}
			catch (...) {}
			return CachedType;
		};

		bool Responding()
		{
			try
			{
				return Detail_ProcessRespondingByWindowHandle(WindowHandleMain());
			}
			catch (...) {}
			return true;
		}

		ProcessPriorityClasses Priority()
		{
			try
			{
				return (ProcessPriorityClasses)GetPriorityClass(Handle());
			}
			catch (...) {}
			return ProcessPriorityClasses::Unknown;
		}

		bool Priority(ProcessPriorityClasses priority)
		{
			bool prioritySet = false;
			try
			{
				prioritySet = SetPriorityClass(Handle(), (int)priority);
				AVDebugWriteLine("Set process priority class: " << (int)priority << "/" << prioritySet);
			}
			catch (...) {}
			return prioritySet;
		}

		std::string AppUserModelId()
		{
			try
			{
				if (CachedAppUserModelId.empty())
				{
					CachedAppUserModelId = Detail_AppUserModelIdByProcessHandle(Handle());
				}
			}
			catch (...) {}
			return CachedAppUserModelId;
		};

		std::string ExeName()
		{
			try
			{
				if (CachedExeName.empty())
				{
					std::filesystem::path filePath(ExePath());
					CachedExeName = filePath.filename().string();
				}
			}
			catch (...) {}
			return CachedExeName;
		};

		std::string ExeNameNoExt()
		{
			try
			{
				if (CachedExeNameNoExt.empty())
				{
					std::filesystem::path filePath(ExeName());
					CachedExeNameNoExt = filePath.filename().replace_extension().string();
				}
			}
			catch (...) {}
			return CachedExeNameNoExt;
		}

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

		HWND WindowHandleMain(bool checkVisibility = true)
		{
			HWND windowHandleMain = NULL;
			try
			{
				//Check process name
				if (!Check_WindowProcessNameIsValid(ExeName()))
				{
					return windowHandleMain;
				}

				//Get window handle
				if (!AppUserModelId().empty())
				{
					windowHandleMain = Get_WindowHandleMainByAppUserModelId(AppUserModelId(), checkVisibility);
				}
				if (windowHandleMain == NULL)
				{
					windowHandleMain = Get_WindowHandleMainByProcessId(Identifier(), checkVisibility);
				}
			}
			catch (...) {}
			return windowHandleMain;
		};

		std::vector<HWND> WindowHandles()
		{
			try
			{
				if (Type() == ProcessType::UWP)
				{
					return Get_WindowHandlesByAppUserModelId(AppUserModelId());
				}
				else
				{
					return Get_WindowHandlesByProcessId(Identifier());
				}
			}
			catch (...) {}
			return std::vector<HWND>();
		};

		std::string WindowClassNameMain()
		{
			try
			{
				return Detail_ClassNameByWindowHandle(WindowHandleMain());
			}
			catch (...) {}
			return "";
		}

		std::string WindowTitleMain()
		{
			try
			{
				if (CustomWindowTitleMain.empty())
				{
					std::string foundWindowTitle = Detail_WindowTitleByWindowHandle(WindowHandleMain());
					if (foundWindowTitle == "Unknown")
					{
						return ExeNameNoExt() + " window";
					}
					else
					{
						return foundWindowTitle;
					}
				}
				else
				{
					return CustomWindowTitleMain;
				}
			}
			catch (...) {}
			return "";
		}

		void WindowTitleMain(std::string customTitle)
		{
			try
			{
				CustomWindowTitleMain = customTitle;
			}
			catch (...) {}
		}

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
				AVDebugWriteLine("Type: " << (int)Type());
				//AVDebugWriteLine("AdminAccess: " << AccessStatus.AdminAccess);
				AVDebugWriteLine("Responding: " << Responding());
				AVDebugWriteLine("Priority: " << (int)Priority());
				AVDebugWriteLine("AppUserModelId: " << AppUserModelId().c_str());
				AVDebugWriteLine("ExeName: " << ExeName().c_str());
				AVDebugWriteLine("ExeNameNoExt: " << ExeNameNoExt().c_str());
				AVDebugWriteLine("ExePath: " << ExePath().c_str());
				//AVDebugWriteLine("WorkPath: " << WorkPath);
				//AVDebugWriteLine("Argument: " << Argument);
				AVDebugWriteLine("WindowHandleMain: " << WindowHandleMain());
				AVDebugWriteLine("WindowHandles: " << WindowHandles().size());
				AVDebugWriteLine("WindowClassNameMain: " << WindowClassNameMain().c_str());
				AVDebugWriteLine("WindowTitleMain: " << WindowTitleMain().c_str());
				AVDebugWriteLine("StartTime: " << StartTime().wMilliseconds);
				AVDebugWriteLine("RunTime: " << RunTime().wMilliseconds);
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
				auto c4 = Type();
				//auto c5 = AccessStatus;
				auto c6 = Responding();
				auto c7 = Priority();
				auto c8 = AppUserModelId();
				auto c9 = ExeName();
				auto c10 = ExeNameNoExt();
				auto c11 = ExePath();
				//auto c12 = WorkPath;
				//auto c13 = Argument;
				auto c14 = WindowHandleMain();
				auto c15 = WindowHandles();
				auto c16 = WindowClassNameMain();
				auto c17 = WindowTitleMain();
				auto c18 = StartTime();
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