#pragma once
#include <vector>
#include <algorithm>
#include <string>

namespace ArnoldVinkCode
{
	inline bool array_contains(std::vector<std::wstring> array, std::wstring value)
	{
		return std::find(array.begin(), array.end(), value) != array.end();
	}

	inline bool array_contains(std::vector<std::string> array, std::string value)
	{
		return std::find(array.begin(), array.end(), value) != array.end();
	}
}