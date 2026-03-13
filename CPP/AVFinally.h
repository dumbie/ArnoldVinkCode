#pragma once
#include <unknwn.h>
#include <wininet.h>
#define AVFinally(callback) AVFinallyFunction x([&]{ try { callback } catch (...) {} });

//Description: Runs code after going out of scope or loop.
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

//Description: Automatically releases object after going out of scope or loop.
//Note: Error C0000374 usually means you are using the wrong release method.
//Usage example: avFin = AVFinObj<VARIANT>(AVFinObjMethod::VariantClear);
//Usage example: avFin = AVFin(AVFinMethod::Custom, releaseObject);
//Usage example: avFin.SetReleaser([&](auto releaseObject)
//{
//	for (int i = 0; i < releaseItemCount; i++)
//	{
//		delete[](releaseObject[i].WCHAR);
//	}
//	delete[](releaseObject);
//});
enum class AVFinMethod
{
	DeleteSingle,
	DeleteArray,
	ReleaseInterface,
	CloseHandle,
	InternetCloseHandle,
	RegCloseKey,
	FreeStringBstr,
	ComFree,
	FreeSid,
	Free,
	Custom
};

enum class AVFinObjMethod
{
	VariantClear,
	PropVariantClear,
	Custom
};

template<typename T>
class AVFin
{
private:
	T ReleaseObject = nullptr;
	std::function<void(T& releaseObject)> ReleaseFunction = nullptr;
	AVFinMethod ReleaseMethod = AVFinMethod::Custom;

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
				else if (ReleaseMethod == AVFinMethod::ReleaseInterface)
				{
					((IUnknown*)ReleaseObject)->Release();
				}
				else if (ReleaseMethod == AVFinMethod::CloseHandle)
				{
					CloseHandle((HANDLE)ReleaseObject);
				}
				else if (ReleaseMethod == AVFinMethod::InternetCloseHandle)
				{
					InternetCloseHandle((HINTERNET)ReleaseObject);
				}
				else if (ReleaseMethod == AVFinMethod::RegCloseKey)
				{
					RegCloseKey((HKEY)ReleaseObject);
				}
				else if (ReleaseMethod == AVFinMethod::FreeStringBstr)
				{
					SysFreeString((BSTR)ReleaseObject);
				}
				else if (ReleaseMethod == AVFinMethod::ComFree)
				{
					CoTaskMemFree((LPVOID)ReleaseObject);
				}
				else if (ReleaseMethod == AVFinMethod::FreeSid)
				{
					FreeSid((PSID)ReleaseObject);
				}
				else if (ReleaseMethod == AVFinMethod::Free)
				{
					free((void*)ReleaseObject);
				}
				else if (ReleaseMethod == AVFinMethod::Custom)
				{
					if (ReleaseFunction != nullptr)
					{
						ReleaseFunction(ReleaseObject);
					}
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
		ReleaseObject = std::move(setObject);
	}

	AVFin(AVFinMethod setMethod, T& setObject)
	{
		ReleaseMethod = setMethod;
		ReleaseObject = std::move(setObject);
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

template<typename T>
class AVFinObj
{
private:
	T* ReleaseObject = nullptr;
	std::function<void(T& releaseObject)> ReleaseFunction = nullptr;
	AVFinObjMethod ReleaseMethod = AVFinObjMethod::Custom;

	bool Release()
	{
		try
		{
			if (ReleaseObject != nullptr)
			{
				if (ReleaseMethod == AVFinObjMethod::VariantClear)
				{
					VariantClear((LPVARIANT)ReleaseObject);
				}
				else if (ReleaseMethod == AVFinObjMethod::PropVariantClear)
				{
					PropVariantClear((LPPROPVARIANT)ReleaseObject);
				}
				else if (ReleaseMethod == AVFinObjMethod::Custom)
				{
					if (ReleaseFunction != nullptr)
					{
						ReleaseFunction(*ReleaseObject);
					}
				}
				ReleaseObject = nullptr;
				return true;
			}
		}
		catch (...) {}
		return false;
	}

public:
	AVFinObj(AVFinObjMethod setMethod)
	{
		ReleaseMethod = setMethod;
		static T defaultObject = T{};
		ReleaseObject = std::move(&defaultObject);
	}

	AVFinObj(AVFinObjMethod setMethod, T setObject)
	{
		ReleaseMethod = setMethod;
		ReleaseObject = std::move(&setObject);
	}

	AVFinObj(AVFinObjMethod setMethod, T* setObject)
	{
		ReleaseMethod = setMethod;
		ReleaseObject = std::move(setObject);
	}

	void SetReleaser(std::function<void(T& releaseObject)> setFunction)
	{
		ReleaseFunction = setFunction;
	}

	T& Get()
	{
		return *ReleaseObject;
	}

	~AVFinObj()
	{
		Release();
	}
};