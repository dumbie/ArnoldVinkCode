#pragma once
#include <Shlobj_core.h>

static std::wstring PathMerge(std::wstring pathLeft, std::wstring pathRight)
{
	try
	{
		std::filesystem::path left_path(pathLeft);
		std::filesystem::path right_path(pathRight);
		return std::wstring(left_path / right_path);
	}
	catch (...)
	{
		return std::wstring(L"");
	}
}

static std::wstring PathGetFolderKnown(KNOWNFOLDERID knownFolderId)
{
	PWSTR ppszPath;
	HRESULT hResult = SHGetKnownFolderPath(knownFolderId, NULL, NULL, &ppszPath);
	if (SUCCEEDED(hResult))
	{
		return ppszPath;
	}
	else
	{
		return L"";
	}
}

static std::wstring PathGetFileName(std::wstring fullPath)
{
	try
	{
		std::filesystem::path path(fullPath);
		return path.filename();
	}
	catch (...)
	{
		return fullPath;
	}
}

static std::wstring PathGetFileNameWithoutExtension(std::wstring fullPath)
{
	try
	{
		std::filesystem::path path(fullPath);
		return path.stem();
	}
	catch (...)
	{
		return fullPath;
	}
}

static std::wstring PathGetExecutableFile()
{
	try
	{
		WCHAR exePath[MAX_PATH]{};
		GetModuleFileNameW(NULL, exePath, MAX_PATH);
		return std::wstring(exePath);
	}
	catch (...)
	{
		return std::wstring(L"");
	}
}

static std::wstring PathGetExecutableDirectory()
{
	try
	{
		WCHAR exePath[MAX_PATH]{};
		GetModuleFileNameW(NULL, exePath, MAX_PATH);
		PathCchRemoveFileSpec(exePath, MAX_PATH);
		return std::wstring(exePath);
	}
	catch (...)
	{
		return std::wstring(L"");
	}
}

static std::vector<std::wstring> PathListFiles(std::wstring pathList, bool recursive)
{
	std::vector<std::wstring> list;
	try
	{
		if (recursive)
		{
			for (auto file : std::filesystem::recursive_directory_iterator(pathList, std::filesystem::directory_options::skip_permission_denied))
			{
				list.push_back(file.path().wstring());
			}
		}
		else
		{
			for (auto file : std::filesystem::directory_iterator(pathList, std::filesystem::directory_options::skip_permission_denied))
			{
				list.push_back(file.path().wstring());
			}
		}
	}
	catch (...) {}
	return list;
}