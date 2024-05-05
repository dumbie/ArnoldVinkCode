#pragma once
#include <sstream>

class AVDebugWriteLine
{
private:
	std::ostringstream stringstream;

public:
	template <class T>
	AVDebugWriteLine& operator << (const T& value)
	{
		stringstream << value;
		return *this;
	}

	~AVDebugWriteLine()
	{
		stringstream << "\n";
		std::cout << stringstream.str() << std::flush;
		OutputDebugStringA(stringstream.str().c_str());
	}
};