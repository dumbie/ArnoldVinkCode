#pragma once
#include <windows.h>
#include <string>
#include <vector>

namespace ArnoldVinkCode::AVProcesses
{
	//ProcessGetAll.h
	std::vector<AVProcess> Get_ProcessAll(int targetProcessId = -1);
	std::optional<AVProcess> Get_ProcessByProcessId(int targetProcessId);
	std::optional<AVProcess> Get_ProcessCurrent();
	std::vector<AVProcess> Get_ProcessByAppUserModelId(std::string targetAppUserModelId);
	std::optional<AVProcess> Get_ProcessByWindowHandle(HWND targetWindowHandle);
	std::vector<AVProcess> Get_ProcessByName(std::string targetName, bool exactName);
	std::vector<AVProcess> Get_ProcessByExecutablePath(std::string targetExecutablePath);
	std::vector<AVProcess> Get_ProcessByWindowTitle(std::string targetWindowTitle, bool exactName);
}