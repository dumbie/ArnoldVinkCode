#pragma once
#include <sstream>
#include <iostream>
#include <windows.h>
#define AVDebugWriteLine(message) (AVDebugWriteLineInternal() << message)

//Usage example: AVDebugWriteLine("Hello " << "World " << 1);
//Usage example: AVDebugWriteLineInternal() << "Hello " << "World " << 1;
class AVDebugWriteLineInternal
{
private:
	std::ostringstream stringstream;

public:
	template <class T>
	AVDebugWriteLineInternal& operator << (const T& value)
	{
		stringstream << value;
		return *this;
	}

	~AVDebugWriteLineInternal()
	{
		stringstream << "\n";
		std::cout << stringstream.str() << std::flush;
		OutputDebugStringA(stringstream.str().c_str());
	}
};