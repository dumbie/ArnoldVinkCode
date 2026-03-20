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
	struct __PROCESS_BASIC_INFORMATION64
	{
		NTSTATUS ExitStatus;
		DWORD32 PebBaseAddress;
		DWORD32 AffinityMask;
		LONG BasePriority;
		DWORD32 UniqueProcessId;
		DWORD32 InheritedFromUniqueProcessId;
	};

	struct __PEB64
	{
		DWORD32 Reserved0;
		DWORD32 Reserved1;
		DWORD32 Reserved2;
		DWORD32 Reserved3;
		DWORD32 RtlUserProcessParameters;
	};

	struct __UNICODE_STRING64
	{
		USHORT Length;
		USHORT MaximumLength;
		DWORD32 Buffer;
	};

	struct __RTL_DRIVE_LETTER_CURDIR64
	{
		USHORT Flags;
		USHORT Length;
		ULONG TimeStamp;
		__UNICODE_STRING64 DosPath;
	};

	struct __RTL_USER_PROCESS_PARAMETERS64
	{
		ULONG MaximumLength;
		ULONG Length;
		ULONG Flags;
		ULONG DebugFlags;
		DWORD32 ConsoleHandle;
		ULONG ConsoleFlags;
		DWORD32 StandardInput;
		DWORD32 StandardOutput;
		DWORD32 StandardError;
		__UNICODE_STRING64 CurrentDirectory;
		DWORD32 CurrentDirectoryHandle;
		__UNICODE_STRING64 DllPath;
		__UNICODE_STRING64 ImagePathName;
		__UNICODE_STRING64 CommandLine;
		DWORD32 Environment;
		ULONG StartingX;
		ULONG StartingY;
		ULONG CountX;
		ULONG CountY;
		ULONG CountCharsX;
		ULONG CountCharsY;
		ULONG FillAttribute;
		ULONG WindowFlags;
		ULONG ShowWindowFlags;
		__UNICODE_STRING64 WindowTitle;
		__UNICODE_STRING64 DesktopInfo;
		__UNICODE_STRING64 ShellInfo;
		__UNICODE_STRING64 RuntimeData;
		__RTL_DRIVE_LETTER_CURDIR64 CurrentDirectores[32];
		ULONG EnvironmentSize;
	};

	//Methods
	inline std::wstring GetApplicationParameter64(HANDLE processHandle, ProcessParameterOptions pOption)
	{
		try
		{
			//AVDebugWriteLine("GetApplicationParameter architecture 64");

			DWORD64 pebBaseAddress = NULL;
			NTSTATUS readResult = NtQueryInformationProcess(processHandle, ProcessWow64Information, &pebBaseAddress, sizeof(pebBaseAddress), NULL);
			if (!NT_SUCCESS(readResult))
			{
				//AVDebugWriteLine("Failed to get ProcessWow64Information for: " << processHandle << "/Query failed.");
				return  L"";
			}

			__PEB64 pebCopy{};
			readResult = NtReadVirtualMemory(processHandle, (PVOID)pebBaseAddress, &pebCopy, sizeof(pebCopy), NULL);
			if (!NT_SUCCESS(readResult))
			{
				//AVDebugWriteLine("Failed to get PebBaseAddress for: " << processHandle);
				return  L"";
			}

			__RTL_USER_PROCESS_PARAMETERS64 paramsCopy{};
			readResult = NtReadVirtualMemory(processHandle, (PVOID)pebCopy.RtlUserProcessParameters, &paramsCopy, sizeof(paramsCopy), NULL);
			if (!NT_SUCCESS(readResult))
			{
				//AVDebugWriteLine("Failed to get ProcessParameters for: " << processHandle);
				return  L"";
			}

			ULONG stringLength = NULL;
			DWORD32 stringBuffer = NULL;
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
			readResult = NtReadVirtualMemory(processHandle, (PVOID)stringBuffer, getString.data(), stringLength, NULL);
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