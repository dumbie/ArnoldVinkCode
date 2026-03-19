#pragma once
#include <windows.h>

namespace ArnoldVinkCode::AVProcesses
{
	class ProcessAccessStatus
	{
	public:
		bool UiAccess = false;
		bool AdminAccess = false;
		bool Elevation = false;
		TOKEN_ELEVATION_TYPE ElevationType = TokenElevationTypeDefault;
	};

	class ProcessThreadInfo
	{
	public:
		int Identifier = 0;
		ProcessThreadState ThreadState = ProcessThreadState::Unknown;
		ProcessThreadWaitReason ThreadWaitReason = ProcessThreadWaitReason::Unknown;

		bool Suspended()
		{
			return ThreadState == ProcessThreadState::Waiting && ThreadWaitReason == ProcessThreadWaitReason::Suspended;
		}
	};
}