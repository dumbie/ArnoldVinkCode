#pragma once
#include <unknwn.h>
#include "AVDebug.h"
#define AVFinally(callback) AVFinallyFunction x([&]{ callback });
#define AVFinallySafe(callback) AVFinallyFunction x([&]{ try { callback } catch (...) {} });

//Description: Runs code after going out of scope or loop
//Usage example: AVFinally(void(););
//Usage example: AVFinally({ void1(); void2(); });
//Note: Add in top of function before any other code is executed
template <typename T>
class AVFinallyFunction
{
private:
	T runFunction;
	bool runTriggered = false;

public:
	AVFinallyFunction(T f) : runFunction{ f } {}

	~AVFinallyFunction()
	{
		if (!runTriggered)
		{
			runTriggered = true;
			runFunction();
		}
	}
};

//Description: Automatically frees pointer after going out of scope or loop
//Usage example: AVFinPtr<int*>();
//Usage example: AVFinPtr(new int[1024]);
//Usage example: avFinPtr.SetReleaser(0, [](auto, auto releasePointer) { releasePointer->Release(); });
//Usage example: avFinPtr.SetReleaser(5, [](const int releaseItemCount, auto releasePointer)
//{
//	for (int i = 0; i < releaseItemCount; i++)
//	{
//		delete[](releasePointer[i].WCHAR);
//	}
//	delete[](releasePointer);
//});
//Note: Error C0000374 usually means you are using the wrong release method (new() = delete / new[] = delete[] / malloc = free / interface = Release)
enum class AVFinPtrMethod
{
	DeleteSingle,
	DeleteArray,
	Release,
	Free
};

template<typename T>
class AVFinPtr
{
private:
	T Pointer = nullptr;
	int ReleaseItemCount = 0;
	std::function<void(const int releaseItemCount, T& releasePointer)> ReleaseFunction = nullptr;
	AVFinPtrMethod ReleaseMethod = AVFinPtrMethod::Free;

	bool Release()
	{
		try
		{
			if (Pointer != nullptr)
			{
				if (ReleaseFunction != nullptr)
				{
					ReleaseFunction(ReleaseItemCount, Pointer);
				}
				else
				{
					if (ReleaseMethod == AVFinPtrMethod::DeleteSingle)
					{
						delete((void*)Pointer);
					}
					else if (ReleaseMethod == AVFinPtrMethod::DeleteArray)
					{
						delete[]((void*)Pointer);
					}
					else if (ReleaseMethod == AVFinPtrMethod::Release)
					{
						((IUnknown*)Pointer)->Release();
					}
					else
					{
						free((void*)Pointer);
					}
				}
				Pointer = nullptr;
				return true;
			}
		}
		catch (...) {}
		return false;
	}

public:
	AVFinPtr() {};
	AVFinPtr(T& setPointer)
	{
		Release();
		Pointer = setPointer;
	}

	void SetPointer(T& setPointer)
	{
		Release();
		Pointer = setPointer;
	}

	void SetReleaseMethod(AVFinPtrMethod setReleaseMethod)
	{
		ReleaseMethod = setReleaseMethod;
	}

	void SetReleaser(int setItemCount, std::function<void(const int releaseItemCount, T& releasePointer)> setFunction)
	{
		ReleaseItemCount = setItemCount;
		ReleaseFunction = setFunction;
	}

	T& Get()
	{
		return Pointer;
	}

	~AVFinPtr()
	{
		Release();
	}
};