#pragma once
#include <windows.h>
#include <winternl.h>
#include <ntstatus.h>

//Imports
const static HMODULE ntdll_hmodule = GetModuleHandleW(L"ntdll.dll");
inline NTSTATUS WINAPI NtWow64QueryInformationProcess64(IN HANDLE ProcessHandle, IN PROCESSINFOCLASS ProcessInformationClass, OUT PVOID ProcessInformation, IN ULONG ProcessInformationLength, OUT OPTIONAL PULONG ReturnLength)
{
	const static auto decl = decltype(&NtWow64QueryInformationProcess64)(GetProcAddress(ntdll_hmodule, "NtWow64QueryInformationProcess64"));
	return decl(ProcessHandle, ProcessInformationClass, ProcessInformation, ProcessInformationLength, ReturnLength);
}

inline NTSTATUS WINAPI NtWow64ReadVirtualMemory64(IN HANDLE ProcessHandle, IN PVOID64 BaseAddress, OUT PVOID Buffer, IN ULONG64 NumberOfBytesToRead, OUT OPTIONAL PULONG64 NumberOfBytesRead)
{
	const static auto decl = decltype(&NtWow64ReadVirtualMemory64)(GetProcAddress(ntdll_hmodule, "NtWow64ReadVirtualMemory64"));
	return decl(ProcessHandle, BaseAddress, Buffer, NumberOfBytesToRead, NumberOfBytesRead);
}

namespace ArnoldVinkCode::AVProcesses
{
	//Structures
	struct __PROCESS_BASIC_INFORMATIONWOW64
	{
		NTSTATUS ExitStatus;
		PVOID64 PebBaseAddress;
		PVOID64 AffinityMask;
		LONG BasePriority;
		PVOID64 UniqueProcessId;
		PVOID64 InheritedFromUniqueProcessId;
	};

	struct __PEBWOW64
	{
		PVOID64 Reserved0;
		PVOID64 Reserved1;
		PVOID64 Reserved2;
		PVOID64 Reserved3;
		PVOID64 RtlUserProcessParameters;
	};

	struct __UNICODE_STRINGWOW64
	{
		USHORT Length;
		USHORT MaximumLength;
		PVOID64 Buffer;
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
		PVOID64 ConsoleHandle;
		ULONG ConsoleFlags;
		PVOID64 StandardInput;
		PVOID64 StandardOutput;
		PVOID64 StandardError;
		__UNICODE_STRINGWOW64 CurrentDirectory;
		PVOID64 CurrentDirectoryHandle;
		__UNICODE_STRINGWOW64 DllPath;
		__UNICODE_STRINGWOW64 ImagePathName;
		__UNICODE_STRINGWOW64 CommandLine;
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

			__PROCESS_BASIC_INFORMATIONWOW64 basicInformation{};
			NTSTATUS readResult = NtWow64QueryInformationProcess64(processHandle, ProcessBasicInformation, &basicInformation, sizeof(basicInformation), NULL);
			if (!NT_SUCCESS(readResult))
			{
				//AVDebugWriteLine("Failed to get ProcessBasicInformation for: " << processHandle << "/Query failed.");
				return  L"";
			}

			__PEBWOW64 pebCopy{};
			readResult = NtWow64ReadVirtualMemory64(processHandle, basicInformation.PebBaseAddress, &pebCopy, sizeof(pebCopy), NULL);
			if (!NT_SUCCESS(readResult))
			{
				//AVDebugWriteLine("Failed to get PebBaseAddress for: " << processHandle);
				return  L"";
			}

			__RTL_USER_PROCESS_PARAMETERSWOW64 paramsCopy{};
			readResult = NtWow64ReadVirtualMemory64(processHandle, pebCopy.RtlUserProcessParameters, &paramsCopy, sizeof(paramsCopy), NULL);
			if (!NT_SUCCESS(readResult))
			{
				//AVDebugWriteLine("Failed to get ProcessParameters for: " << processHandle);
				return  L"";
			}

			ULONG stringLength = NULL;
			PVOID64 stringBuffer = nullptr;
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
			readResult = NtWow64ReadVirtualMemory64(processHandle, stringBuffer, getString.data(), stringLength, NULL);
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