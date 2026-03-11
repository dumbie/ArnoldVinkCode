#pragma once
#include <unknwn.h>
#define AVFinally(callback) AVFinallyFunction x([&]{ callback });
#define AVFinallySafe(callback) AVFinallyFunction x([&]{ try { callback } catch (...) {} });

//Description: Runs code after going out of scope or loop
//Usage example: AVFinally({ void1(); void2(); });
//Usage example: releaseObject = new int[1024]; AVFinally({ delete[] releaseObject; });
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

//Description: Automatically releases object after going out of scope or loop
//Usage example: AVFin(AVFinMethod::Free, releaseObject);
//Usage example: avFin.SetReleaser([&](auto releaseObject)
//{
//	for (int i = 0; i < releaseItemCount; i++)
//	{
//		delete[](releaseObject[i].WCHAR);
//	}
//	delete[](releaseObject);
//});
//Note: Error C0000374 usually means you are using the wrong release method (new() = DeleteSingle / new[] = DeleteArray / malloc = Free / Interface = Release / HANDLE = CloseHandle / CoTaskMemAlloc = ComFree)
enum class AVFinMethod
{
	DeleteSingle,
	DeleteArray,
	Release,
	CloseHandle,
	Custom,
	ComFree,
	Free
};

template<typename T>
class AVFin
{
private:
	T ReleaseObject = nullptr;
	std::function<void(T& releaseObject)> ReleaseFunction = nullptr;
	AVFinMethod ReleaseMethod = AVFinMethod::Free;

	bool Release()
	{
		try
		{
			if (ReleaseObject != nullptr)
			{
				if (ReleaseMethod == AVFinMethod::DeleteSingle)
				{
					delete((void*)ReleaseObject);
				}
				else if (ReleaseMethod == AVFinMethod::DeleteArray)
				{
					delete[]((void*)ReleaseObject);
				}
				else if (ReleaseMethod == AVFinMethod::Release)
				{
					((IUnknown*)ReleaseObject)->Release();
				}
				else if (ReleaseMethod == AVFinMethod::CloseHandle)
				{
					CloseHandle(ReleaseObject);
				}
				else if (ReleaseMethod == AVFinMethod::Custom)
				{
					if (ReleaseFunction != nullptr)
					{
						ReleaseFunction(ReleaseObject);
					}
				}
				else if (ReleaseMethod == AVFinMethod::ComFree)
				{
					CoTaskMemFree(ReleaseObject);
				}
				else
				{
					free((void*)ReleaseObject);
				}
				ReleaseObject = nullptr;
				return true;
			}
		}
		catch (...) {}
		return false;
	}

public:
	AVFin(AVFinMethod setMethod)
	{
		ReleaseMethod = setMethod;
	}

	AVFin(AVFinMethod setMethod, T setObject)
	{
		ReleaseMethod = setMethod;
		ReleaseObject = setObject;
		setObject = nullptr;
	}

	AVFin(AVFinMethod setMethod, T& setObject)
	{
		ReleaseMethod = setMethod;
		ReleaseObject = setObject;
		setObject = nullptr;
	}

	void SetReleaser(std::function<void(T& releaseObject)> setFunction)
	{
		ReleaseFunction = setFunction;
	}

	T& Get()
	{
		return ReleaseObject;
	}

	~AVFin()
	{
		Release();
	}
};