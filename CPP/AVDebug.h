#pragma once
#include <windows.h>
#include <iostream>
#define AVDebugWriteLine(message) AVDebugWriteLineInternal() << message

//Usage example: AVDebugWriteLine("Hello " << "World " << 1);
//Usage example: AVDebugWriteLineInternal() << "Hello " << "World " << 1;
//Description: Show string in both console and debug output
struct AVDebugWriteLineInternal
{
	std::wostringstream stringstream;

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