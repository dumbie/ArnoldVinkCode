#pragma once
#include <string>
#include <vector>
#include <winrt/Windows.System.h>

namespace ArnoldVinkCode
{
	inline std::string char_to_string(const char* str)
	{
		return std::string(str);
	}

	inline std::wstring char_to_wstring(const char* str)
	{
		std::string s(str);
		return std::wstring(s.begin(), s.end());
	}

	inline std::string wchar_to_string(const wchar_t* str)
	{
		std::wstring ws(str);
		return std::string(ws.begin(), ws.end());
	}

	inline std::wstring wchar_to_wstring(const wchar_t* str)
	{
		return std::wstring(str);
	}

	inline std::string wstring_to_string(std::wstring str)
	{
		return std::string(str.begin(), str.end());
	}

	inline BSTR wchar_to_bstring(const wchar_t* str)
	{
		BSTR bString = SysAllocString(str);
		//SysFreeString(bString);
		return bString;
	}

	inline std::string hstring_to_string(winrt::hstring str)
	{
		return std::string(str.begin(), str.end());
	}

	inline winrt::hstring string_to_hstring(std::string str)
	{
		return winrt::to_hstring(str);
	}

	inline std::wstring string_to_wstring(std::string str)
	{
		return std::wstring(str.begin(), str.end());
	}

	inline int string_to_int(std::string str)
	{
		return std::stoi(str);
	}

	inline int wstring_to_int(std::wstring str)
	{
		return std::stoi(str);
	}

	inline int hstring_to_int(winrt::hstring str)
	{
		std::string s(winrt::to_string(str));
		return std::stoi(s);
	}

	template<typename T>
	inline std::string number_to_string(T value)
	{
		return std::to_string(value);
	}

	template<typename T>
	inline std::wstring number_to_wstring(T value)
	{
		return std::to_wstring(value);
	}

	template<typename T>
	inline winrt::hstring number_to_hstring(T value)
	{
		return winrt::to_hstring(value);
	}

	inline std::string vector_to_string(std::vector<std::string> list, std::string split)
	{
		std::string stringReturn;
		for (std::string stringVector : list)
		{
			stringReturn += stringVector;
			if (!stringReturn.empty() && !split.empty())
			{
				stringReturn += split;
			}
		}
		return stringReturn;
	}

	inline std::vector<std::string> string_to_vector(std::string str)
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

	inline std::vector<std::string> string_split(std::string str, char split)
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

	inline bool string_replace(std::string& string, std::string from, std::string to)
	{
		size_t start_pos = string.find(from);
		if (start_pos == std::string::npos)
		{
			return false;
		}
		string.replace(start_pos, from.length(), to);
		return true;
	}

	inline bool string_replace(std::wstring& string, std::wstring from, std::wstring to)
	{
		size_t start_pos = string.find(from);
		if (start_pos == std::wstring::npos)
		{
			return false;
		}
		string.replace(start_pos, from.length(), to);
		return true;
	}

	inline bool string_replace_all(std::string& string, std::string from, std::string to)
	{
		size_t start_pos = 0;
		while ((start_pos = string.find(from, start_pos)) != std::string::npos)
		{
			string.replace(start_pos, from.length(), to);
			start_pos += to.length();
		}
		return true;
	}

	inline std::string string_to_lower(std::string str)
	{
		std::string strTemp = str;
		std::transform(strTemp.begin(), strTemp.end(), strTemp.begin(), tolower);
		return strTemp;
	}

	inline std::wstring wstring_to_lower(std::wstring str)
	{
		std::wstring strTemp = str;
		std::transform(strTemp.begin(), strTemp.end(), strTemp.begin(), tolower);
		return strTemp;
	}

	inline std::string string_to_upper(std::string str)
	{
		std::string strTemp = str;
		std::transform(strTemp.begin(), strTemp.end(), strTemp.begin(), toupper);
		return strTemp;
	}

	inline std::wstring wstring_to_upper(std::wstring str)
	{
		std::wstring strTemp = str;
		std::transform(strTemp.begin(), strTemp.end(), strTemp.begin(), toupper);
		return strTemp;
	}

	inline bool string_contains(std::string str, std::string contains)
	{
		return str.find(contains) != std::string::npos;
	}

	inline bool wstring_contains(std::wstring str, std::wstring contains)
	{
		return str.find(contains) != std::wstring::npos;
	}
}