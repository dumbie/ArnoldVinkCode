#pragma once
#include <windows.h>
#pragma comment(lib, "ntdll.lib")
#include <string>
#include <vector>

namespace ArnoldVinkCode::AVProcesses
{
	//ProcessCheck.h
	bool Check_PathUwpApplication(std::wstring targetPath);
	bool Check_PathUrlProtocol(std::wstring targetPath);
	bool Check_RunningProcessByProcessId(int targetProcessId);
	bool Check_RunningProcessByWindowHandle(HWND targetWindowHandle);
	bool Check_RunningProcessByName(std::string targetProcessName, bool exactName);
	bool Check_RunningProcessByAppUserModelId(std::string targetAppUserModelId);
	bool Check_ProcessIsForegroundUwpApp(int targetProcessId);
	bool Check_WindowClassNameIsUwpApp(std::string targetClassName);
	bool Check_WindowClassNameIsValid(std::string targetClassName);
	bool Check_WindowProcessNameIsValid(std::string targetProcessName);
	bool Check_WindowHandleValid(HWND targetWindowHandle, bool checkMainWindow, bool checkVisibility);

	//ProcessClose.h
	bool Close_ProcessByProcessId(int targetProcessId);
	bool Close_ProcessTreeByProcessId(int targetProcessId);
	bool Close_ProcessByName(std::string targetProcessName, bool exactName);
	bool Close_ProcessByExecutablePath(std::string targetExecutablePath);
	bool Close_ProcessByAppUserModelId(std::string targetAppUserModelId);
	bool Close_ProcessByWindowMessage(HWND targetWindowHandle);

	//ProcessDetail.h
	HWND Detail_WindowHandleMainByProcessId(int targetProcessId, bool checkVisibility);
	HWND Detail_WindowHandleMainByThreadId(int targetThreadId, bool checkVisibility);
	HWND Detail_WindowHandleMainByAppUserModelId(std::string targetAppUserModelId, bool checkVisibility);
	HANDLE Detail_ProcessHandleByProcessId(int targetProcessId);
	std::tm Detail_ProcessStartTimeByProcessHandle(HANDLE processHandle);
	std::string Detail_WindowTitleByWindowHandle(HWND targetWindowHandle);
	int Detail_WindowZOrderByWindowHandle(HWND windowHandle);
	std::string Detail_ClassNameByWindowHandle(HWND targetWindowHandle);
	int Detail_ProcessIdByWindowHandle(HWND targetWindowHandle);
	std::string Detail_ExecutablePathByProcessHandle(HANDLE targetProcessHandle);
	std::string Detail_PackageFullNameByProcessHandle(HANDLE targetProcessHandle);
	std::string Detail_AppUserModelIdByProcessHandle(HANDLE targetProcessHandle);
	std::string Detail_AppUserModelIdByWindowHandle(HWND targetWindowHandle);

	//ProcessGetThreads.h
	std::vector<ProcessThreadInfo> Detail_ProcessThreadsByProcessId(int targetProcessId, bool firstThreadOnly);

	//ProcessHide.h
	bool Hide_ProcessByWindowHandle(HWND windowHandle);
	bool Hide_ProcessByProcessId(int processId);

	//ProcessLaunch.h
	bool Launch_ApplicationDesktop(std::wstring exePath, std::wstring workPath, std::wstring arguments, bool asAdmin, bool waitForExit = false);
	bool Launch_ApplicationUwp(std::wstring appUserModelId, std::wstring arguments);

	//ProcessRestart.h
	bool Restart_ProcessByProcessId(int processId, std::wstring newArgs, bool withoutArgs);

	//ProcessShow.h
	void Close_WindowsPrompts();
	bool Show_ProcessByWindowHandle(HWND windowHandle);
	bool Show_ProcessByProcessId(int processId);

	//ProcessStatus.h
	bool Detail_ProcessRespondingByWindowHandle(HWND targetWindowHandle);
	ProcessAccessStatus Detail_ProcessAccessStatusByProcessId(int targetProcessId, bool currentProcess);

	//ProcessWindow.h
	std::vector<HWND> Detail_WindowHandlesByProcessId(int targetProcessId);
	std::vector<HWND> Detail_WindowHandlesByThreadId(int targetThreadId);
	std::vector<HWND> Detail_WindowHandlesByAppUserModelId(std::string targetAppUserModelId);

	//QueryProcess.h
	std::wstring Remove_ExePathFromCommandLine(std::wstring targetCommandLine);
	int Detail_ProcessParentIdByProcessHandle(HANDLE targetProcessHandle);
	std::wstring Detail_ParameterByProcessHandle(HANDLE targetProcessHandle, ProcessParameterOptions pOption);
}