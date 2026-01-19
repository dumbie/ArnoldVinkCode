#pragma once
#include <windows.h>
#include <appmodel.h>
#include <propsys.h>
#include <propkey.h>

namespace ArnoldVinkCode::AVProcesses
{
	//Get class name by window handle
	inline std::string Detail_ClassNameByWindowHandle(HWND targetWindowHandle)
	{
		try
		{
			CHAR buffer[1024];
			DWORD bufferSize = sizeof(buffer);
			GetClassNameA(targetWindowHandle, buffer, bufferSize);
			return std::string(buffer);
		}
		catch (...) {}
		return "";
	}

	//Get full exe path by process handle
	inline std::string Detail_ExecutablePathByProcessHandle(HANDLE targetProcessHandle)
	{
		try
		{
			CHAR buffer[1024];
			DWORD bufferSize = sizeof(buffer);
			if (QueryFullProcessImageNameA(targetProcessHandle, 0, buffer, &bufferSize))
			{
				return std::string(buffer);
			}
		}
		catch (...) {}
		return "";
	}

	//Get AppUserModelId by process handle
	inline std::string Detail_AppUserModelIdByProcessHandle(HANDLE targetProcessHandle)
	{
		try
		{
			UINT32 stringLength = 1024;
			std::vector<WCHAR> stringBuilder(1024);
			if (GetApplicationUserModelId(targetProcessHandle, &stringLength, stringBuilder.data()) == ERROR_SUCCESS)
			{
				std::wstring appUserModelIdW = stringBuilder.data();
				//Fix if (Check_PathUwpApplication(appUserModelIdW))
				//{
				return wstring_to_string(appUserModelIdW);
				//}
			}
		}
		catch (...) {}
		return "";
	}

	//Get AppUserModelId by window handle
	inline std::string Detail_AppUserModelIdByWindowHandle(HWND targetWindowHandle)
	{
		IPropertyStore* propertyStore;
		PROPVARIANT propertyVariant;
		AVFinally(
			{
				propertyStore->Release();
				PropVariantClear(&propertyVariant);
			});
		try
		{
			SHGetPropertyStoreForWindow(targetWindowHandle, IID_PPV_ARGS(&propertyStore));

			PropVariantInit(&propertyVariant);
			propertyStore->GetValue(PKEY_AppUserModel_ID, &propertyVariant);
			if (propertyVariant.vt == VT_LPWSTR)
			{
				std::wstring appUserModelIdW = propertyVariant.pwszVal;
				//Fix if (Check_PathUwpApplication(appUserModelIdW))
				//{
				return wstring_to_string(appUserModelIdW);
				//}
			}
		}
		catch (...) {}
		return "";
	}
}