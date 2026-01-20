#pragma once
#include <windows.h>

namespace ArnoldVinkCode::AVProcesses
{
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
		Unknown = 10
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
		Unknown = 7
	};

	enum class ProcessType
	{
		Unknown = 0,
		Win32 = 1,
		Win32Store = 2,
		UWP = 3
	};
}