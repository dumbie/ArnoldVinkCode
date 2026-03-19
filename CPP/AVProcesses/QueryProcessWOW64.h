#pragma once
#include <windows.h>
#include <winternl.h>
#include <ntstatus.h>

#pragma comment(lib,"ntdll.lib")
extern "C" NTSTATUS WINAPI NtReadVirtualMemory(HANDLE ProcessHandle, PVOID BaseAddress, PVOID Buffer, ULONG NumberOfBytesToRead, PULONG NumberOfBytesRead);

namespace ArnoldVinkCode::AVProcesses
{
	//Structures
	struct __PEBWOW64
	{
		DWORD Reserved0;
		DWORD Reserved1;
		DWORD Reserved2;
		DWORD Reserved3;
		DWORD RtlUserProcessParameters;
	};

	struct __UNICODE_STRINGWOW64
	{
		USHORT Length;
		USHORT MaximumLength;
		DWORD Buffer;
	};

	struct __RTL_DRIVE_LETTER_CURDIRWOW64
	{
		USHORT Flags;
		USHORT Length;
		ULONG TimeStamp;
		__UNICODE_STRINGWOW64 DosPath;
	};

	struct __RTL_USER_PROCESS_PARAMETERSWOW64
	{
		ULONG MaximumLength;
		ULONG Length;
		ULONG Flags;
		ULONG DebugFlags;
		DWORD ConsoleHandle;
		ULONG ConsoleFlags;
		DWORD StandardInput;
		DWORD StandardOutput;
		DWORD StandardError;
		__UNICODE_STRINGWOW64 CurrentDirectory;
		DWORD CurrentDirectoryHandle;
		__UNICODE_STRINGWOW64 DllPath;
		__UNICODE_STRINGWOW64 ImagePathName;
		__UNICODE_STRINGWOW64 CommandLine;
		DWORD Environment;
		ULONG StartingX;
		ULONG StartingY;
		ULONG CountX;
		ULONG CountY;
		ULONG CountCharsX;
		ULONG CountCharsY;
		ULONG FillAttribute;
		ULONG WindowFlags;
		ULONG ShowWindowFlags;
		__UNICODE_STRINGWOW64 WindowTitle;
		__UNICODE_STRINGWOW64 DesktopInfo;
		__UNICODE_STRINGWOW64 ShellInfo;
		__UNICODE_STRINGWOW64 RuntimeData;
		__RTL_DRIVE_LETTER_CURDIRWOW64 CurrentDirectores[32];
		ULONG EnvironmentSize;
	};

	//Methods
	inline std::wstring GetApplicationParameterWOW64(HANDLE processHandle, ProcessParameterOptions pOption)
	{
		try
		{
			//AVDebugWriteLine("GetApplicationParameter architecture WOW64");

			DWORD64 pebBaseAddress = NULL;
			NTSTATUS readResult = NtQueryInformationProcess(processHandle, ProcessWow64Information, &pebBaseAddress, sizeof(pebBaseAddress), NULL);
			if (!NT_SUCCESS(readResult))
			{
				//AVDebugWriteLine("Failed to get ProcessBasicInformation for: " << processHandle << "/Query failed.");
				return  L"";
			}

			__PEBWOW64 pebCopy{};
			readResult = NtReadVirtualMemory(processHandle, (PVOID64)pebBaseAddress, &pebCopy, sizeof(pebCopy), NULL);
			if (!NT_SUCCESS(readResult))
			{
				//AVDebugWriteLine("Failed to get PebBaseAddress for: " << processHandle);
				return  L"";
			}

			__RTL_USER_PROCESS_PARAMETERSWOW64 paramsCopy{};
			readResult = NtReadVirtualMemory(processHandle, (PVOID64)pebCopy.RtlUserProcessParameters, &paramsCopy, sizeof(paramsCopy), NULL);
			if (!NT_SUCCESS(readResult))
			{
				//AVDebugWriteLine("Failed to get ProcessParameters for: " << processHandle);
				return  L"";
			}

			ULONG stringLength = NULL;
			DWORD64 stringBuffer = NULL;
			if (pOption == ProcessParameterOptions::CurrentDirectoryPath)
			{
				stringLength = paramsCopy.CurrentDirectory.Length;
				stringBuffer = paramsCopy.CurrentDirectory.Buffer;
			}
			else if (pOption == ProcessParameterOptions::ImagePathName)
			{
				stringLength = paramsCopy.ImagePathName.Length;
				stringBuffer = paramsCopy.ImagePathName.Buffer;
			}
			else if (pOption == ProcessParameterOptions::DesktopInfo)
			{
				stringLength = paramsCopy.DesktopInfo.Length;
				stringBuffer = paramsCopy.DesktopInfo.Buffer;
			}
			else if (pOption == ProcessParameterOptions::Environment)
			{
				stringLength = paramsCopy.EnvironmentSize;
				stringBuffer = paramsCopy.Environment;
			}
			else
			{
				stringLength = paramsCopy.CommandLine.Length;
				stringBuffer = paramsCopy.CommandLine.Buffer;
			}

			if (stringLength <= 0)
			{
				//AVDebugWriteLine("Failed to get ParameterString length for: " << processHandle);
				return  L"";
			}

			std::wstring getString;
			getString.insert(getString.begin(), stringLength, ' ');
			readResult = NtReadVirtualMemory(processHandle, (PVOID64)stringBuffer, getString.data(), stringLength, NULL);
			if (!NT_SUCCESS(readResult))
			{
				AVDebugWriteLine("Failed to get ParameterString for: " << processHandle);
				return  L"";
			}
			else
			{
				//AVDebugWriteLine("Got ParameterString: " << processHandle << "/" << getString);
				return getString;
			}
		}
		catch (...)
		{
			AVDebugWriteLine("Failed to get GetApplicationParameter.");
			return  L"";
		}
	}
}