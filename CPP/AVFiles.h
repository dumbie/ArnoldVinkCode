#pragma once
#include <windows.h>
#include <string>
#include <fstream>
#include <filesystem>
#include <pathcch.h>

namespace ArnoldVinkCode
{
	inline std::string file_to_string(std::string filePath)
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

	inline bool string_to_file(std::string filePath, std::string string)
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

	inline bool FileDelete(std::wstring filePath)
	{
		try
		{
			return std::filesystem::remove(filePath);
		}
		catch (...)
		{
			return false;
		}
	}

	inline bool FileExists(std::wstring filePath)
	{
		try
		{
			return std::filesystem::exists(filePath);
		}
		catch (...)
		{
			return false;
		}
	}

	inline bool FileMove(std::wstring oldFilePath, std::wstring newFilePath, bool overWrite)
	{
		try
		{
			if (oldFilePath == newFilePath)
			{
				AVDebugWriteLine(L"Failed moving file: targeting the same path.");
				return false;
			}

			if (FileExists(oldFilePath))
			{
				AVDebugWriteLine(L"Moving: " + oldFilePath + L" to " + newFilePath);
				if (overWrite) { FileDelete(newFilePath); }
				std::filesystem::rename(oldFilePath, newFilePath);
				return true;
			}
			else
			{
				AVDebugWriteLine(L"Failed moving file: " + oldFilePath + L" does not exist");
				return false;
			}
		}
		catch (...)
		{
			return false;
		}
	}

	inline std::vector<std::filesystem::directory_entry> FileList(std::wstring folderPath, bool listRecursive)
	{
		std::vector<std::filesystem::directory_entry> fileList;
		try
		{
			if (listRecursive)
			{
				auto iterator = std::filesystem::recursive_directory_iterator(folderPath);
				for (auto entry : iterator)
				{
					fileList.push_back(entry);
				}
			}
			else
			{
				auto iterator = std::filesystem::directory_iterator(folderPath);
				for (auto entry : iterator)
				{
					fileList.push_back(entry);
				}
			}
		}
		catch (...) {}
		return fileList;
	}

	inline bool FolderCreate(std::wstring folderPath)
	{
		try
		{
			return std::filesystem::create_directory(folderPath);
		}
		catch (...)
		{
			return false;
		}
	}

	inline bool FolderExists(std::wstring folderPath)
	{
		try
		{
			return std::filesystem::exists(folderPath);
		}
		catch (...)
		{
			return false;
		}
	}
}