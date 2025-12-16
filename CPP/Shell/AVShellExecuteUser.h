#pragma once
#pragma comment(lib, "Ole32")
#include "..\AVVariant.h"

namespace ArnoldVinkCode
{
	//Functions
	inline bool ShellExecuteUser(SHELLEXECUTEINFOW shellExecuteInfo)
	{
		IShellWindows* shellWindows;
		IDispatch* dispFindWindowSW;
		IServiceProvider* serviceProvider;
		IShellBrowser* shellBrowser;
		IShellView* desktopView;
		IDispatch* dispBackground;
		IShellFolderViewDual* shellFolderViewDual;
		IDispatch* dispApplication{};
		IShellDispatch2* shellDispatch2;
		AVFinallySafe(
			{
				shellWindows->Release();
				dispFindWindowSW->Release();
				serviceProvider->Release();
				shellBrowser->Release();
				desktopView->Release();
				dispBackground->Release();
				shellFolderViewDual->Release();
				dispApplication->Release();
				shellDispatch2->Release();
			});
		try
		{
			//Initialize COM library
			HRESULT hResult = CoInitialize(NULL);
			if (!SUCCEEDED(hResult))
			{
				AVDebugWriteLine(L"Failed to initialize COM library.");
				return false;
			}

			//Create shell windows instance
			const CLSID CLSID_ShellWindows = { 0x9BA05972, 0xF6A8, 0x11CF, {0xA4, 0x42, 0x00, 0xA0, 0xC9, 0x0A, 0x8F, 0x39} };
			hResult = CoCreateInstance(CLSID_ShellWindows, NULL, CLSCTX_ALL, IID_PPV_ARGS(&shellWindows));
			if (!SUCCEEDED(hResult))
			{
				AVDebugWriteLine(L"Failed to create shell windows instance.");
				return false;
			}

			//Find shell desktop window
			long pHwnd;
			VARIANT varLoc{};
			VARIANT varLocRoot{};
			hResult = shellWindows->FindWindowSW(&varLoc, &varLocRoot, SWC_DESKTOP, &pHwnd, SWFO_NEEDDISPATCH, &dispFindWindowSW);
			if (!SUCCEEDED(hResult))
			{
				AVDebugWriteLine(L"Failed to find shell desktop window.");
				return false;
			}
			hResult = dispFindWindowSW->QueryInterface(IID_PPV_ARGS(&serviceProvider));
			if (!SUCCEEDED(hResult))
			{
				AVDebugWriteLine(L"Failed to find shell desktop window.");
				return false;
			}

			//Query top level shell browser
			hResult = serviceProvider->QueryService(SID_STopLevelBrowser, IID_PPV_ARGS(&shellBrowser));
			if (!SUCCEEDED(hResult))
			{
				AVDebugWriteLine(L"Failed to query top level shell browser.");
				return false;
			}

			//Query active shell view
			hResult = shellBrowser->QueryActiveShellView(&desktopView);
			if (!SUCCEEDED(hResult))
			{
				AVDebugWriteLine(L"Failed to query active shell view.");
				return false;
			}

			//Get shell folder view dual
			hResult = desktopView->GetItemObject(SVGIO_BACKGROUND, IID_PPV_ARGS(&dispBackground));
			if (!SUCCEEDED(hResult))
			{
				AVDebugWriteLine(L"Failed to get shell folder view dual.");
				return false;
			}
			hResult = dispBackground->QueryInterface(IID_PPV_ARGS(&shellFolderViewDual));
			if (!SUCCEEDED(hResult))
			{
				AVDebugWriteLine(L"Failed to get shell folder view dual.");
				return false;
			}

			//Get application dispatch
			hResult = shellFolderViewDual->get_Application((IDispatch**)&dispApplication);
			if (!SUCCEEDED(hResult))
			{
				AVDebugWriteLine(L"Failed to get application dispatch.");
				return false;
			}
			hResult = dispApplication->QueryInterface(IID_PPV_ARGS(&shellDispatch2));
			if (!SUCCEEDED(hResult))
			{
				AVDebugWriteLine(L"Failed to get application dispatch.");
				return false;
			}

			//Shell execute
			BSTR lpFile = wchar_to_bstring(shellExecuteInfo.lpFile);
			VARIANT lpParameters = wchar_to_variant(shellExecuteInfo.lpParameters);
			VARIANT lpDirectory = wchar_to_variant(shellExecuteInfo.lpDirectory);
			VARIANT lpVerb = wchar_to_variant(shellExecuteInfo.lpVerb);
			VARIANT nShow = number_to_variant(shellExecuteInfo.nShow);
			hResult = shellDispatch2->ShellExecute(lpFile, lpParameters, lpDirectory, lpVerb, nShow);

			//Return result
			return SUCCEEDED(hResult);
		}
		catch (...)
		{
			return false;
		}
	}
}