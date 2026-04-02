#pragma once
#pragma comment(lib, "Ole32")
#include "..\AVVariant.h"

namespace ArnoldVinkCode
{
	//Functions
	inline bool ShellExecuteUser(SHELLEXECUTEINFOW shellExecuteInfo)
	{
		try
		{
			//Initialize COM library
			HRESULT hResult = CoInitialize(nullptr);
			if (!SUCCEEDED(hResult))
			{
				AVDebugWriteLine(L"Failed to initialize COM library.");
				return false;
			}

			//Create shell windows instance
			const CLSID CLSID_ShellWindows = { 0x9BA05972, 0xF6A8, 0x11CF, {0xA4, 0x42, 0x00, 0xA0, 0xC9, 0x0A, 0x8F, 0x39} };

			auto shellWindows = AVFin<IShellWindows*>(AVFinMethod::ReleaseInterface);
			hResult = CoCreateInstance(CLSID_ShellWindows, nullptr, CLSCTX_ALL, IID_PPV_ARGS(&shellWindows.Get()));
			if (!SUCCEEDED(hResult))
			{
				AVDebugWriteLine(L"Failed to create shell windows instance.");
				return false;
			}

			//Find shell desktop window
			long pHwnd;
			auto varLoc = AVFinObj<VARIANT>(AVFinObjMethod::VariantClear);
			auto varLocRoot = AVFinObj<VARIANT>(AVFinObjMethod::VariantClear);
			auto dispFindWindowSW = AVFin<IDispatch*>(AVFinMethod::ReleaseInterface);
			hResult = shellWindows.Get()->FindWindowSW(&varLoc.Get(), &varLocRoot.Get(), SWC_DESKTOP, &pHwnd, SWFO_NEEDDISPATCH, &dispFindWindowSW.Get());
			if (!SUCCEEDED(hResult))
			{
				AVDebugWriteLine(L"Failed to find shell desktop window.");
				return false;
			}

			//Query service provider
			auto serviceProvider = AVFin<IServiceProvider*>(AVFinMethod::ReleaseInterface);
			hResult = dispFindWindowSW.Get()->QueryInterface(IID_PPV_ARGS(&serviceProvider.Get()));
			if (!SUCCEEDED(hResult))
			{
				AVDebugWriteLine(L"Failed to find shell desktop window.");
				return false;
			}

			//Query top level shell browser
			auto shellBrowser = AVFin<IShellBrowser*>(AVFinMethod::ReleaseInterface);
			hResult = serviceProvider.Get()->QueryService(SID_STopLevelBrowser, IID_PPV_ARGS(&shellBrowser.Get()));
			if (!SUCCEEDED(hResult))
			{
				AVDebugWriteLine(L"Failed to query top level shell browser.");
				return false;
			}

			//Query active shell view
			auto desktopView = AVFin<IShellView*>(AVFinMethod::ReleaseInterface);
			hResult = shellBrowser.Get()->QueryActiveShellView(&desktopView.Get());
			if (!SUCCEEDED(hResult))
			{
				AVDebugWriteLine(L"Failed to query active shell view.");
				return false;
			}

			//Get background of shell view
			auto dispBackground = AVFin<IDispatch*>(AVFinMethod::ReleaseInterface);
			hResult = desktopView.Get()->GetItemObject(SVGIO_BACKGROUND, IID_PPV_ARGS(&dispBackground.Get()));
			if (!SUCCEEDED(hResult))
			{
				AVDebugWriteLine(L"Failed to get shell folder view dual.");
				return false;
			}

			//Get shell folder view dual
			auto shellFolderViewDual = AVFin<IShellFolderViewDual*>(AVFinMethod::ReleaseInterface);
			hResult = dispBackground.Get()->QueryInterface(IID_PPV_ARGS(&shellFolderViewDual.Get()));
			if (!SUCCEEDED(hResult))
			{
				AVDebugWriteLine(L"Failed to get shell folder view dual.");
				return false;
			}

			//Get application dispatch
			auto appDispatch = AVFin<IDispatch*>(AVFinMethod::ReleaseInterface);
			hResult = shellFolderViewDual.Get()->get_Application(&appDispatch.Get());
			if (!SUCCEEDED(hResult))
			{
				AVDebugWriteLine(L"Failed to get application dispatch.");
				return false;
			}

			//Get shell dispatch 2
			auto shellDispatch = AVFin<IShellDispatch2*>(AVFinMethod::ReleaseInterface);
			hResult = appDispatch.Get()->QueryInterface(IID_PPV_ARGS(&shellDispatch.Get()));
			if (!SUCCEEDED(hResult))
			{
				AVDebugWriteLine(L"Failed to get application dispatch.");
				return false;
			}

			//Shell execute
			auto lpFile = AVFin(AVFinMethod::FreeStringBstr, wchar_to_bstring(shellExecuteInfo.lpFile));
			auto lpParameters = AVFinObj(AVFinObjMethod::VariantClear, wchar_to_variant(shellExecuteInfo.lpParameters));
			auto lpDirectory = AVFinObj(AVFinObjMethod::VariantClear, wchar_to_variant(shellExecuteInfo.lpDirectory));
			auto lpVerb = AVFinObj(AVFinObjMethod::VariantClear, wchar_to_variant(shellExecuteInfo.lpVerb));
			auto nShow = AVFinObj(AVFinObjMethod::VariantClear, number_to_variant(shellExecuteInfo.nShow));
			hResult = shellDispatch.Get()->ShellExecute(lpFile.Get(), lpParameters.Get(), lpDirectory.Get(), lpVerb.Get(), nShow.Get());

			//Return result
			return SUCCEEDED(hResult);
		}
		catch (...)
		{
			return false;
		}
	}
}