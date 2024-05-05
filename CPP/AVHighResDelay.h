#pragma once
#include <windows.h>

//Usage example: AVHighResDelay(1000.0F);
void AVHighResDelay(float milliSecondsDelay)
{
	if (milliSecondsDelay < 0.1F)
	{
		milliSecondsDelay = 0.1F;
	}

	LARGE_INTEGER largeInteger{};
	largeInteger.QuadPart = (LONGLONG)(-1.0F * milliSecondsDelay * 10000.0F);

	HANDLE createEvent = CreateWaitableTimerExW(NULL, NULL, CREATE_WAITABLE_TIMER_MANUAL_RESET | CREATE_WAITABLE_TIMER_HIGH_RESOLUTION, TIMER_ALL_ACCESS);
	if (createEvent)
	{
		if (SetWaitableTimerEx(createEvent, &largeInteger, 0, NULL, NULL, NULL, 0))
		{
			WaitForSingleObject(createEvent, INFINITE);
		}
		CloseHandle(createEvent);
	}
}