#pragma once
#include <windows.h>
#include <iostream>
#include <sstream>
#include <fstream>
#define AVDebugWriteLine(message) AVDebugWriteLineInternal() << message
inline bool AVDebugWriteLineLogFileEnabled = false;
inline std::wstring AVDebugWriteLineLogFilePath = L"Debug.log";

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

		//Convert stream to text
		std::wstring outputStringW = stringstream.str();
		LPCWSTR outputStringC = outputStringW.c_str();

		//Write text to console
		DWORD writeConsoleResult;
		WriteConsoleW(GetStdHandle(STD_OUTPUT_HANDLE), outputStringC, outputStringW.length(), &writeConsoleResult, NULL);

		//Write text to output
		OutputDebugStringW(outputStringC);

		//Write text to log file
		if (AVDebugWriteLineLogFileEnabled)
		{
			std::wofstream writeStream(AVDebugWriteLineLogFilePath, std::ios::app);
			writeStream << outputStringC;
			writeStream.close();
		}
	}
};