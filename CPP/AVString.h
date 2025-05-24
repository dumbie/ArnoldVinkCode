#pragma once
#include <string>

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

static std::wstring string_to_wstring(std::string str)
{
	return std::wstring(str.begin(), str.end());
}

static int hstring_to_int(hstring str)
{
	return std::stoi(winrt::to_string(str));
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