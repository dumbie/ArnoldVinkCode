#pragma once
#include <windows.h>
#include <winternl.h>
#include <ntstatus.h>

//Imports
#pragma comment(lib,"ntdll.lib")
extern "C"
{
	NTSTATUS WINAPI NtQueryInformationProcess(IN HANDLE ProcessHandle, IN PROCESSINFOCLASS ProcessInformationClass, OUT PVOID ProcessInformation, IN ULONG ProcessInformationLength, OUT OPTIONAL PULONG ReturnLength);
	NTSTATUS WINAPI NtReadVirtualMemory(IN HANDLE ProcessHandle, IN PVOID BaseAddress, OUT PVOID Buffer, IN ULONG NumberOfBytesToRead, OUT OPTIONAL PULONG NumberOfBytesRead);
}

namespace ArnoldVinkCode::AVProcesses
{
	//Structures
	struct __PROCESS_BASIC_INFORMATION32
	{
		NTSTATUS ExitStatus;
		PVOID PebBaseAddress;
		PVOID AffinityMask;
		LONG BasePriority;
		PVOID UniqueProcessId;
		PVOID InheritedFromUniqueProcessId;
	};

	struct __PEB32
	{
		PVOID Reserved0;
		PVOID Reserved1;
		PVOID Reserved2;
		PVOID Reserved3;
		PVOID RtlUserProcessParameters;
	};

	struct __UNICODE_STRING32
	{
		USHORT Length;
		USHORT MaximumLength;
		PVOID Buffer;
	};

	struct __RTL_DRIVE_LETTER_CURDIR32
	{
		USHORT Flags;
		USHORT Length;
		ULONG TimeStamp;
		__UNICODE_STRING32 DosPath;
	};

	struct __RTL_USER_PROCESS_PARAMETERS32
	{
		ULONG MaximumLength;
		ULONG Length;
		ULONG Flags;
		ULONG DebugFlags;
		PVOID ConsoleHandle;
		ULONG ConsoleFlags;
		PVOID StandardInput;
		PVOID StandardOutput;
		PVOID StandardError;
		__UNICODE_STRING32 CurrentDirectory;
		PVOID CurrentDirectoryHandle;
		__UNICODE_STRING32 DllPath;
		__UNICODE_STRING32 ImagePathName;
		__UNICODE_STRING32 CommandLine;
		PVOID Environment;
		ULONG StartingX;
		ULONG StartingY;
		ULONG CountX;
		ULONG CountY;
		ULONG CountCharsX;
		ULONG CountCharsY;
		ULONG FillAttribute;
		ULONG WindowFlags;
		ULONG ShowWindowFlags;
		__UNICODE_STRING32 WindowTitle;
		__UNICODE_STRING32 DesktopInfo;
		__UNICODE_STRING32 ShellInfo;
		__UNICODE_STRING32 RuntimeData;
		__RTL_DRIVE_LETTER_CURDIR32 CurrentDirectores[32];
		ULONG EnvironmentSize;
	};

	//Methods
	inline std::wstring GetApplicationParameter32(HANDLE processHandle, ProcessParameterOptions pOption)
	{
		try
		{
			//AVDebugWriteLine("GetApplicationParameter architecture 32");

			__PROCESS_BASIC_INFORMATION32 basicInformation{};
			NTSTATUS readResult = NtQueryInformationProcess(processHandle, ProcessBasicInformation, &basicInformation, sizeof(basicInformation), NULL);
			if (!NT_SUCCESS(readResult))
			{
				//AVDebugWriteLine("Failed to get ProcessBasicInformation for: " << processHandle << "/Query failed.");
				return  L"";
			}

			__PEB32 pebCopy{};
			readResult = NtReadVirtualMemory(processHandle, basicInformation.PebBaseAddress, &pebCopy, sizeof(pebCopy), NULL);
			if (!NT_SUCCESS(readResult))
			{
				//AVDebugWriteLine("Failed to get PebBaseAddress for: " << processHandle);
				return  L"";
			}

			__RTL_USER_PROCESS_PARAMETERS32 paramsCopy{};
			readResult = NtReadVirtualMemory(processHandle, pebCopy.RtlUserProcessParameters, &paramsCopy, sizeof(paramsCopy), NULL);
			if (!NT_SUCCESS(readResult))
			{
				//AVDebugWriteLine("Failed to get ProcessParameters for: " << processHandle);
				return  L"";
			}

			ULONG stringLength = NULL;
			PVOID stringBuffer = nullptr;
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
			readResult = NtReadVirtualMemory(processHandle, stringBuffer, getString.data(), stringLength, NULL);
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