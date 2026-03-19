#pragma once
#include <windows.h>

namespace ArnoldVinkCode::AVProcesses
{
	enum class ProcessParameterOptions
	{
		CurrentDirectoryPath,
		ImagePathName,
		CommandLine,
		DesktopInfo,
		Environment
	};

	enum class ProcessPriorityClasses
	{
		Unknown = 0x00,
		Normal = 0x20,
		Idle = 0x40,
		High = 0x80,
		Realtime = 0x100,
		BelowNormal = 0x4000,
		AboveNormal = 0x8000
	};

	enum class ProcessThreadState
	{
		Initialized = 0,
		Ready = 1,
		Running = 2,
		Standby = 3,
		Terminated = 4,
		Waiting = 5,
		Transition = 6,
		DeferredReady = 7,
		GateWait = 8,
		WaitingForProcessInSwap = 9,
		MaximumThreadState = 10,
		Unknown
	};

	enum class ProcessThreadWaitReason
	{
		Executive = 0,
		FreePage = 1,
		PageIn = 2,
		PoolAllocation = 3,
		DelayExecution = 4,
		Suspended = 5,
		UserRequest = 6,
		WrExecutive = 7,
		WrFreePage = 8,
		WrPageIn = 9,
		WrPoolAllocation = 10,
		WrDelayExecution = 11,
		WrSuspended = 12,
		WrUserRequest = 13,
		WrEventPair = 14,
		WrQueue = 15,
		WrLpcReceive = 16,
		WrLpcReply = 17,
		WrVirtualMemory = 18,
		WrPageOut = 19,
		WrRendezvous = 20,
		Spare2 = 21,
		Spare3 = 22,
		Spare4 = 23,
		Spare5 = 24,
		WrCalloutStack = 25,
		WrKernel = 26,
		WrResource = 27,
		WrPushLock = 28,
		WrMutex = 29,
		WrQuantumEnd = 30,
		WrDispatchInt = 31,
		WrPreempted = 32,
		WrYieldExecution = 33,
		WrFastMutex = 34,
		WrGuardedMutex = 35,
		WrRundown = 36,
		MaximumWaitReason = 37,
		Unknown
	};

	enum class ProcessType
	{
		Unknown = 0,
		Win32 = 1,
		Win32Store = 2,
		UWP = 3
	};
}