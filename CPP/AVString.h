#pragma once
#include <string>
#include <vector>
#include <WTypes.h>
#include <winrt/Windows.System.h>
#include <winrt/Windows.UI.Xaml.h>
#include <winrt/Windows.UI.Xaml.Controls.h>

//Namespaces
namespace winrt
{
	using namespace Windows::UI::Xaml;
	using namespace Windows::UI::Xaml::Controls;
}

namespace ArnoldVinkCode
{
	inline bool string_empty_whitespace(std::string targetString)
	{
		return targetString.empty() || std::all_of(targetString.begin(), targetString.end(), [](char chr) { return std::isspace(chr); });
	}

	inline bool wstring_empty_whitespace(std::wstring targetString)
	{
		return targetString.empty() || std::all_of(targetString.begin(), targetString.end(), [](wchar_t chr) { return std::isspace(chr); });
	}

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

	inline std::wstring hstring_to_wstring(winrt::hstring str)
	{
		return std::wstring(str.begin(), str.end());
	}

	inline winrt::hstring wstring_to_hstring(std::wstring str)
	{
		return winrt::hstring(str.c_str());
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
		return winrt::to_hstring(str).c_str();
	}

	inline int string_to_int(std::string str)
	{
		return std::stoi(str);
	}

	inline int wstring_to_int(std::wstring str)
	{
		return std::stoi(str);
	}

	inline int hexstring_to_int(std::string str)
	{
		return std::stoi(str, nullptr, 16);
	}

	inline int whexstring_to_int(std::wstring str)
	{
		return std::stoi(str, nullptr, 16);
	}

	inline float wstring_to_float(std::wstring str)
	{
		return std::stof(str);
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

	template<typename T>
	inline std::string number_to_hexstring(T value, int digitCount, bool includeHeader)
	{
		std::stringstream stream;
		stream << std::uppercase << std::setfill('0') << std::hex << std::setw(digitCount) << value;
		if (includeHeader)
		{
			return "0x" + stream.str();
		}
		else
		{
			return stream.str();
		}
	}

	template<typename T>
	inline std::string number_to_hexstring_littleendian(T value, int digitCount, bool includeHeader)
	{
		digitCount /= 2;
		std::stringstream stream;
		stream << std::uppercase << std::setfill('0') << std::hex << std::setw(digitCount) << (value & 0xFF) << std::setw(digitCount) << ((value >> 8) & 0xFF);
		if (includeHeader)
		{
			return "0x" + stream.str();
		}
		else
		{
			return stream.str();
		}
	}

	template<typename T>
	inline std::wstring number_to_hexwstring(T value, int digitCount, bool includeHeader)
	{
		std::wstringstream stream;
		stream << std::uppercase << std::setfill(L'0') << std::hex << std::setw(digitCount) << value;
		if (includeHeader)
		{
			return L"0x" + stream.str();
		}
		else
		{
			return stream.str();
		}
	}

	template<typename T>
	inline std::wstring number_to_hexwstring_littleendian(T value, int digitCount, bool includeHeader)
	{
		digitCount /= 2;
		std::wstringstream stream;
		stream << std::uppercase << std::setfill(L'0') << std::hex << std::setw(digitCount) << (value & 0xFF) << std::setw(digitCount) << ((value >> 8) & 0xFF);
		if (includeHeader)
		{
			return L"0x" + stream.str();
		}
		else
		{
			return stream.str();
		}
	}

	template<typename T>
	inline std::string float_to_string(T value, int decimals)
	{
		std::ostringstream returnString;
		returnString << std::fixed << std::setprecision(decimals) << value;
		return returnString.str();
	}

	template<typename T>
	inline std::wstring float_to_wstring(T value, int decimals)
	{
		std::wstringstream returnString;
		returnString << std::fixed << std::setprecision(decimals) << value;
		return returnString.str();
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

	inline std::vector<std::wstring> wstring_split(std::wstring str, wchar_t split)
	{
		std::wstring stringLine;
		std::wstringstream stringLines(str);
		std::vector<std::wstring> stringVector;
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
		if (start_pos == std::wstring::npos) { return false; }
		string.replace(start_pos, from.length(), to);
		return true;
	}

	inline bool wstring_replace(std::wstring& string, std::wstring from, std::wstring to)
	{
		size_t start_pos = string.find(from);
		if (start_pos == std::wstring::npos) { return false; }
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

	inline bool wstring_replace_all(std::wstring& string, std::wstring from, std::wstring to)
	{
		size_t start_pos = 0;
		while ((start_pos = string.find(from, start_pos)) != std::wstring::npos)
		{
			string.replace(start_pos, from.length(), to);
			start_pos += to.length();
		}
		return true;
	}

	inline bool string_replace_first(std::string& targetString, std::string from, std::string to)
	{
		size_t replacePosition = targetString.find_first_of(from);
		if (replacePosition == std::string::npos) { return false; }
		targetString.replace(replacePosition, from.length(), to);
		return true;
	}

	inline bool wstring_replace_first(std::wstring& targetString, std::wstring from, std::wstring to)
	{
		size_t replacePosition = targetString.find_first_of(from);
		if (replacePosition == std::wstring::npos) { return false; }
		targetString.replace(replacePosition, from.length(), to);
		return true;
	}

	inline bool string_replace_last(std::string& targetString, std::string from, std::string to)
	{
		size_t replacePosition = targetString.find_last_of(from);
		if (replacePosition == std::string::npos) { return false; }
		targetString.replace(replacePosition, from.length(), to);
		return true;
	}

	inline bool wstring_replace_last(std::wstring& targetString, std::wstring from, std::wstring to)
	{
		size_t replacePosition = targetString.find_last_of(from);
		if (replacePosition == std::wstring::npos) { return false; }
		targetString.replace(replacePosition, from.length(), to);
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
		if (string_empty_whitespace(str))
		{
			return false;
		}
		std::string shortest;
		std::string longest;
		if (str.length() > contains.length())
		{
			longest = str;
			shortest = contains;
		}
		else
		{
			longest = contains;
			shortest = str;
		}
		return longest.find(shortest) != std::string::npos;
	}

	inline bool wstring_contains(std::wstring str, std::wstring contains)
	{
		if (wstring_empty_whitespace(str))
		{
			return false;
		}
		std::wstring shortest;
		std::wstring longest;
		if (str.length() > contains.length())
		{
			longest = str;
			shortest = contains;
		}
		else
		{
			longest = contains;
			shortest = str;
		}
		return longest.find(shortest) != std::wstring::npos;
	}

	inline bool string_starts_with(std::string str, std::string start)
	{
		return str.starts_with(start);
	}

	inline bool wstring_starts_with(std::wstring str, std::wstring start)
	{
		return str.starts_with(start);
	}

	inline std::string string_trim_left(std::string str)
	{
		const CHAR* trim = " \t\n\r\f\v\0";
		str.erase(0, str.find_first_not_of(trim));
		return str;
	}

	inline std::string string_trim_right(std::string str)
	{
		const CHAR* trim = " \t\n\r\f\v\0";
		str.erase(str.find_last_not_of(trim) + 1);
		return str;
	}

	inline std::string string_trim(std::string str)
	{
		return string_trim_left(string_trim_right(str));
	}

	inline std::wstring wstring_trim_left(std::wstring str)
	{
		const WCHAR* trim = L" \t\n\r\f\v\0";
		str.erase(0, str.find_first_not_of(trim));
		return str;
	}

	inline std::wstring wstring_trim_right(std::wstring str)
	{
		const WCHAR* trim = L" \t\n\r\f\v\0";
		str.erase(str.find_last_not_of(trim) + 1);
		return str;
	}

	inline std::wstring wstring_trim(std::wstring str)
	{
		return wstring_trim_left(wstring_trim_right(str));
	}

	inline std::wstring wstring_get_between(std::wstring str, std::wstring start, std::wstring end)
	{
		size_t pos1 = str.find(start);
		size_t pos2 = str.find(end, pos1 + 1);
		if (pos1 != std::wstring::npos && pos2 != std::wstring::npos)
		{
			return str.substr(pos1 + 1, pos2 - pos1 - 1);
		}
		else
		{
			return str;
		}
	}

	inline void wstring_to_tooltip(const winrt::DependencyObject& depObject, std::wstring str)
	{
		//Get current tooltip text
		auto tooltipStringCurrentBox = winrt::ToolTipService::GetToolTip(depObject);
		winrt::hstring tooltipStringCurrentHString = winrt::unbox_value<winrt::hstring>(tooltipStringCurrentBox);
		std::wstring tooltipStringCurrentString = hstring_to_wstring(tooltipStringCurrentHString);

		//Check and set tooltip text
		if (tooltipStringCurrentString != str)
		{
			winrt::ToolTipService::SetToolTip(depObject, winrt::box_value(str));
		}
	}

	inline void string_to_tooltip(const winrt::DependencyObject& depObject, std::string str)
	{
		//Get current tooltip text
		auto tooltipStringCurrentBox = winrt::ToolTipService::GetToolTip(depObject);
		winrt::hstring tooltipStringCurrentHString = winrt::unbox_value<winrt::hstring>(tooltipStringCurrentBox);
		std::string tooltipStringCurrentString = hstring_to_string(tooltipStringCurrentHString);

		//Check and set tooltip text
		if (tooltipStringCurrentString != str)
		{
			winrt::ToolTipService::SetToolTip(depObject, winrt::box_value(string_to_wstring(str)));
		}
	}
}