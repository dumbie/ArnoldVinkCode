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
	std::wostringstream stringstream;

public:
	template <typename T>
	AVDebugWriteLineInternal& operator << (const T& value)
	{
		stringstream << value;
		return *this;
	}

	~AVDebugWriteLineInternal()
	{
		stringstream << "\n";
		OutputDebugStringW(stringstream.str().c_str());
		std::wcout << stringstream.str().c_str() << std::flush;
	}
};