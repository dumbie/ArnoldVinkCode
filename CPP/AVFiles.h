#pragma once
#include <string>
#include <fstream>
#include <filesystem>

static std::string file_to_string(std::string filePath)
{
	std::ifstream file;
	file.open(filePath);
	auto fileSize = std::filesystem::file_size(filePath);
	std::string string(fileSize, '\0');
	file.read(string.data(), fileSize);
	file.close();
	return string;
}

static bool string_to_file(std::string filePath, std::string string)
{
	std::ofstream file;
	file.open(filePath);
	file.write(string.c_str(), string.size());
	file.close();
	return true;
}