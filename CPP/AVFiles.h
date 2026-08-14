#pragma once
#include <windows.h>
#include <string>
#include <fstream>
#include <filesystem>
#include <pathcch.h>

namespace ArnoldVinkCode
{
	inline std::wstring file_to_string(std::wstring filePath)
	{
		try
		{
			std::wifstream inputStream(filePath, std::ios::binary);
			std::wstringstream stringBuffer;
			stringBuffer << inputStream.rdbuf();
			inputStream.close();
			return stringBuffer.str();
		}
		catch (...)
		{
			return L"";
		}
	}

	inline bool string_to_file(std::wstring filePath, std::wstring string)
	{
		try
		{
			std::wofstream outputStream(filePath, std::ios::binary);
			outputStream << string;
			outputStream.close();
			return true;
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

	inline bool FolderDelete(std::wstring folderPath, bool recursive)
	{
		try
		{
			if (!recursive)
			{
				return std::filesystem::remove_all(folderPath);
			}
			else
			{
				int removeCount = 0;
				for (auto& path : std::filesystem::directory_iterator(folderPath))
				{
					if (std::filesystem::remove_all(path)) { removeCount++; }
				}
				return removeCount > 0;
			}
		}
		catch (...)
		{
			return false;
		}
	}

	inline bool FolderWritePermission(std::wstring folderPath)
	{
		try
		{
			//Set write path
			std::wstring writePath = PathMerge(folderPath, L"writepermission");

			//Open stream
			std::wofstream writeStream(writePath, std::ios::binary);

			//Check if stream is good
			if (!writeStream.good())
			{
				return false;
			}

			//Close stream
			writeStream.close();

			//Remove file
			std::filesystem::remove(writePath);
			return true;
		}
		catch (...)
		{
			return false;
		}
	}
}