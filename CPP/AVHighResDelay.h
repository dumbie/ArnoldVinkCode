#pragma once
#include <windows.h>
#include "AVDebug.h"
#include "AVFinally.h"

namespace ArnoldVinkCode
{
	//Usage example: AVHighResDelay(1000.0F);
	inline void AVHighResDelay(float milliSecondsDelay)
	{
		HANDLE createEvent = NULL;
		AVFinallySafe(
			{
				CloseHandle(createEvent);
			});
		try
		{
			if (milliSecondsDelay <= 0)
			{
				AVDebugWriteLine("Invalid delay time.");
				return;
			}

			LARGE_INTEGER largeInteger{};
			largeInteger.QuadPart = (LONGLONG)(-1.0F * milliSecondsDelay * 10000.0F);

			createEvent = CreateWaitableTimerExW(NULL, NULL, CREATE_WAITABLE_TIMER_MANUAL_RESET | CREATE_WAITABLE_TIMER_HIGH_RESOLUTION, TIMER_ALL_ACCESS);
			if (createEvent)
			{
				if (SetWaitableTimerEx(createEvent, &largeInteger, 0, NULL, NULL, NULL, 0))
				{
					WaitForSingleObject(createEvent, INFINITE);
				}
			}
		}
		catch (...) {}
	}
}