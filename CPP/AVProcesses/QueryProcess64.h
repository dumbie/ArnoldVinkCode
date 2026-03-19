#pragma once
#include <windows.h>
#include <winternl.h>
#include <ntstatus.h>

#pragma comment(lib,"ntdll.lib")
extern "C" NTSTATUS WINAPI NtReadVirtualMemory(HANDLE ProcessHandle, PVOID BaseAddress, PVOID Buffer, ULONG NumberOfBytesToRead, PULONG NumberOfBytesRead);

namespace ArnoldVinkCode::AVProcesses
{
	//Structures
	struct __PROCESS_BASIC_INFORMATION64
	{
		NTSTATUS ExitStatus;
		PVOID64 PebBaseAddress;
		PVOID64 AffinityMask;
		LONG BasePriority;
		PVOID64 UniqueProcessId;
		PVOID64 InheritedFromUniqueProcessId;
	};

	struct __PEB64
	{
		PVOID64 Reserved0;
		PVOID64 Reserved1;
		PVOID64 Reserved2;
		PVOID64 Reserved3;
		PVOID64 RtlUserProcessParameters;
	};

	struct __UNICODE_STRING64
	{
		USHORT Length;
		USHORT MaximumLength;
		PVOID64 Buffer;
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
		PVOID64 ConsoleHandle;
		ULONG ConsoleFlags;
		PVOID64 StandardInput;
		PVOID64 StandardOutput;
		PVOID64 StandardError;
		__UNICODE_STRING64 CurrentDirectory;
		PVOID64 CurrentDirectoryHandle;
		__UNICODE_STRING64 DllPath;
		__UNICODE_STRING64 ImagePathName;
		__UNICODE_STRING64 CommandLine;
		PVOID64 Environment;
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

			__PROCESS_BASIC_INFORMATION64 basicInformation{};
			NTSTATUS readResult = NtQueryInformationProcess(processHandle, ProcessBasicInformation, &basicInformation, sizeof(basicInformation), NULL);
			if (!NT_SUCCESS(readResult))
			{
				//AVDebugWriteLine("Failed to get ProcessBasicInformation for: " << processHandle << "/Query failed.");
				return  L"";
			}

			__PEB64 pebCopy{};
			readResult = NtReadVirtualMemory(processHandle, basicInformation.PebBaseAddress, &pebCopy, sizeof(pebCopy), NULL);
			if (!NT_SUCCESS(readResult))
			{
				//AVDebugWriteLine("Failed to get PebBaseAddress for: " << processHandle);
				return  L"";
			}

			__RTL_USER_PROCESS_PARAMETERS64 paramsCopy{};
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