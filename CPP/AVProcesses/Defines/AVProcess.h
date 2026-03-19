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
	class AVProcess
	{
	private:
		int CachedIdentifier = 0;
		int CachedIdentifierParent = 0;
		HANDLE CachedHandle = NULL;
		ProcessType CachedType = ProcessType::Unknown;
		std::optional<ProcessAccessStatus> CachedAccessStatus = std::nullopt;
		std::string CachedAppUserModelId = "";
		std::string CachedExeName = "";
		std::string CachedExeNameNoExt = "";
		std::string CachedExePath = "";
		std::string CachedWorkPath = "";
		std::string CachedArgument = "";
		std::string CustomWindowTitleMain = "";
		std::tm CachedStartTime = tm_empty();

	public:
		AVProcess(int identifier, int identifierParent, std::string exeName)
		{
			CachedIdentifier = identifier;
			CachedIdentifierParent = identifierParent;
			CachedExeName = exeName;
		}

		int Identifier()
		{
			return CachedIdentifier;
		}

		int IdentifierParent()
		{
			try
			{
				if (CachedIdentifierParent <= 0)
				{
					CachedIdentifierParent = Detail_ProcessParentIdByProcessHandle(Handle());
				}
			}
			catch (...) {}
			return CachedIdentifierParent;
		}

		HANDLE Handle()
		{
			try
			{
				if (CachedHandle == NULL)
				{
					CachedHandle = Detail_ProcessHandleByProcessId(Identifier());
				}
			}
			catch (...) {}
			return CachedHandle;
		}

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
		}

		ProcessAccessStatus AccessStatus()
		{
			try
			{
				if (!CachedAccessStatus.has_value())
				{
					CachedAccessStatus = Detail_ProcessAccessStatusByProcessId(Identifier(), false);
				}
			}
			catch (...) {}
			return CachedAccessStatus.value();
		}

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
		}

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
		}

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
		}

		std::string WorkPath()
		{
			try
			{
				if (CachedWorkPath.empty())
				{
					std::wstring parameterWString = Detail_ParameterByProcessHandle(Handle(), ProcessParameterOptions::CurrentDirectoryPath);
					CachedWorkPath = wstring_to_string(parameterWString);
				}
			}
			catch (...) {}
			return CachedWorkPath;
		}

		std::string Argument()
		{
			try
			{
				if (CachedArgument.empty())
				{
					std::wstring parameterWString = Detail_ParameterByProcessHandle(Handle(), ProcessParameterOptions::CommandLine);
					CachedArgument = wstring_to_string(parameterWString);
				}
			}
			catch (...) {}
			return CachedArgument;
		}

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
					windowHandleMain = Detail_WindowHandleMainByAppUserModelId(AppUserModelId(), checkVisibility);
				}
				if (windowHandleMain == NULL)
				{
					windowHandleMain = Detail_WindowHandleMainByProcessId(Identifier(), checkVisibility);
				}
			}
			catch (...) {}
			return windowHandleMain;
		}

		std::vector<HWND> WindowHandles()
		{
			try
			{
				if (Type() == ProcessType::UWP)
				{
					return Detail_WindowHandlesByAppUserModelId(AppUserModelId());
				}
				else
				{
					return Detail_WindowHandlesByProcessId(Identifier());
				}
			}
			catch (...) {}
			return std::vector<HWND>();
		}

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

		std::tm StartTime()
		{
			try
			{
				if (tm_is_empty(CachedStartTime))
				{
					CachedStartTime = Detail_ProcessStartTimeByProcessHandle(Handle());
				}
			}
			catch (...) {}
			return CachedStartTime;
		}

		ATL::CTimeSpan RunTime()
		{
			try
			{
				//Get current and start time
				std::tm current_time = tm_current();
				std::tm start_time = StartTime();

				//Get time difference
				return time_difference(current_time, start_time);
			}
			catch (...) {}
			return CTimeSpan(0);
		}

		bool Suspended()
		{
			try
			{
				return Detail_ProcessThreadsByProcessId(Identifier(), true)[0].Suspended();
			}
			catch (...) {}
			return false;
		}

		std::vector<ProcessThreadInfo> Threads()
		{
			try
			{
				return Detail_ProcessThreadsByProcessId(Identifier(), false);
			}
			catch (...) {}
			return std::vector<ProcessThreadInfo>{};
		}

		bool Validate()
		{
			try
			{
				return Identifier() > 0 && !ExePath().empty();
			}
			catch (...) {}
			return false;
		}

		void Debug()
		{
			try
			{
				AVDebugWriteLine("Identifier: " << Identifier());
				AVDebugWriteLine("IdentifierParent: " << IdentifierParent());
				AVDebugWriteLine("Handle: " << Handle());
				AVDebugWriteLine("Type: " << (int)Type());
				AVDebugWriteLine("AdminAccess: " << AccessStatus().AdminAccess);
				AVDebugWriteLine("Responding: " << Responding());
				AVDebugWriteLine("Priority: " << (int)Priority());
				AVDebugWriteLine("AppUserModelId: " << AppUserModelId().c_str());
				AVDebugWriteLine("ExeName: " << ExeName().c_str());
				AVDebugWriteLine("ExeNameNoExt: " << ExeNameNoExt().c_str());
				AVDebugWriteLine("ExePath: " << ExePath().c_str());
				AVDebugWriteLine("WorkPath: " << WorkPath().c_str());
				AVDebugWriteLine("Argument: " << Argument().c_str());
				AVDebugWriteLine("WindowHandleMain: " << WindowHandleMain());
				AVDebugWriteLine("WindowHandles: " << WindowHandles().size());
				AVDebugWriteLine("WindowClassNameMain: " << WindowClassNameMain().c_str());
				AVDebugWriteLine("WindowTitleMain: " << WindowTitleMain().c_str());
				AVDebugWriteLine("StartTime: " << tm_to_string(StartTime(), "%d.%m.%Y %H:%M:%S").c_str());
				AVDebugWriteLine("RunTime: " << RunTime().GetTotalSeconds() << "sec");
				AVDebugWriteLine("Suspended: " << Suspended());
				AVDebugWriteLine("Threads: " << Threads().size());
				//AVDebugWriteLine("AppPackageName: " << AppPackage.DisplayName);
				//AVDebugWriteLine("AppxDetailsName: " << AppxDetails.DisplayName);
			}
			catch (...) {}
		}

		void Cache()
		{
			try
			{
				auto c1 = Identifier();
				auto c2 = IdentifierParent();
				auto c3 = Handle();
				auto c4 = Type();
				auto c5 = AccessStatus();
				auto c6 = Responding();
				auto c7 = Priority();
				auto c8 = AppUserModelId();
				auto c9 = ExeName();
				auto c10 = ExeNameNoExt();
				auto c11 = ExePath();
				auto c12 = WorkPath();
				auto c13 = Argument();
				auto c14 = WindowHandleMain();
				auto c15 = WindowHandles();
				auto c16 = WindowClassNameMain();
				auto c17 = WindowTitleMain();
				auto c18 = StartTime();
				auto c19 = RunTime();
				auto c20 = Suspended();
				auto c21 = Threads();
				//auto c22 = AppPackage;
				//auto c23 = AppxDetails;
			}
			catch (...) {}
		}

		~AVProcess() { Dispose(); }
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
		}
	};
}