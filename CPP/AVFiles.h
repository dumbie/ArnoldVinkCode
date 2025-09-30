#pragma once
#include <string>
#include <fstream>
#include <filesystem>
#include <pathcch.h>

static std::string file_to_string(std::string filePath)
{
	try
	{
		std::ifstream file;
		file.open(filePath);
		auto fileSize = std::filesystem::file_size(filePath);
		std::string string(fileSize, '\0');
		file.read(string.data(), fileSize);
		file.close();
		return string;
	}
	catch (...)
	{
		return std::string("");
	}
}

static bool string_to_file(std::string filePath, std::string string)
{
	try
	{
		std::ofstream file;
		file.open(filePath);
		file.write(string.c_str(), string.size());
		file.close();
		return true;
	}
	catch (...)
	{
		return false;
	}
}

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