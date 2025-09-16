#pragma once
#include <string>
#include <vector>

static std::string char_to_string(const char* str)
{
	return std::string(str);
}

static std::wstring char_to_wstring(const char* str)
{
	std::string s(str);
	return std::wstring(s.begin(), s.end());
}

static std::string wchar_to_string(const wchar_t* str)
{
	std::wstring ws(str);
	return std::string(ws.begin(), ws.end());
}

static std::wstring wchar_to_wstring(const wchar_t* str)
{
	return std::wstring(str);
}

static std::string wstring_to_string(std::wstring str)
{
	return std::string(str.begin(), str.end());
}

static std::string hstring_to_string(hstring str)
{
	return std::string(str.begin(), str.end());
}

static hstring string_to_hstring(std::string str)
{
	return to_hstring(str);
}

static std::wstring string_to_wstring(std::string str)
{
	return std::wstring(str.begin(), str.end());
}

static int string_to_int(std::string str)
{
	return std::stoi(str);
}

static int wstring_to_int(std::wstring str)
{
	return std::stoi(str);
}

static int hstring_to_int(hstring str)
{
	std::string s(winrt::to_string(str));
	return std::stoi(s);
}

template<typename T>
static std::string number_to_string(T value)
{
	return std::to_string(value);
}

template<typename T>
static std::wstring number_to_wstring(T value)
{
	return std::to_wstring(value);
}

template<typename T>
static hstring number_to_hstring(T value)
{
	return winrt::to_hstring(value);
}

static std::string vector_to_string(std::vector<std::string> list, std::string split)
{
	std::string stringReturn;
	for (auto stringVector : list)
	{
		stringReturn += stringVector;
		if (!stringReturn.empty() && !split.empty())
		{
			stringReturn += split;
		}
	}
	return stringReturn;
}

static std::vector<std::string> string_to_vector(std::string str)
{
	std::string stringLine;
	std::stringstream stringLines(str);
	std::vector<std::string> stringVector;
	while (std::getline(stringLines, stringLine))
	{
		if (!stringLine.empty())
		{
			stringVector.push_back(stringLine);
		}
	}
	return stringVector;
}

static std::vector<std::string> string_split(std::string str, char split)
{
	std::string stringLine;
	std::stringstream stringLines(str);
	std::vector<std::string> stringVector;
	while (std::getline(stringLines, stringLine, split))
	{
		if (!stringLine.empty())
		{
			stringVector.push_back(stringLine);
		}
	}
	return stringVector;
}

static bool string_replace(std::string& string, std::string from, std::string to)
{
	size_t start_pos = string.find(from);
	if (start_pos == std::string::npos)
	{
		return false;
	}
	string.replace(start_pos, from.length(), to);
	return true;
}

static bool string_replace_all(std::string& string, std::string from, std::string to)
{
	size_t start_pos = 0;
	while ((start_pos = string.find(from, start_pos)) != std::string::npos)
	{
		string.replace(start_pos, from.length(), to);
		start_pos += to.length();
	}
	return true;
}