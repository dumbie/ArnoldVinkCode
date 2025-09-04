#pragma once
#define AVFinally(callback) AVFinallyInternal x([&]{ callback });
#define AVFinallySafe(callback) AVFinallyInternal x([&]{ try { callback } catch (...) {} });

//Usage example: AVFinally(void(););
//Usage example: AVFinally({ void1(); void2(); });
//Description: Runs code after a function returns
//Note: Add in top of function before any other code is executed
template <typename T>
struct AVFinallyInternal
{
	T runFunction;
	BOOL runTriggered = false;

	AVFinallyInternal(T f) : runFunction{ f } {}

	~AVFinallyInternal()
	{
		if (!runTriggered)
		{
			runTriggered = true;
			runFunction();
		}
	}
};