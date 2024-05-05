#pragma once
#include <sstream>

class AVDebugWriteLine
{
private:
	std::ostringstream stringstream;

public:
	template <class T>
	void operator << (const T& value)
	{
		stringstream << value;
	}

	~AVDebugWriteLine()
	{
		stringstream << "\n";
		std::cout << stringstream.str() << std::flush;
		OutputDebugStringA(stringstream.str().c_str());
	}
};