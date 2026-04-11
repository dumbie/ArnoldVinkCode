#pragma once
#include <windows.h>
#include <string>
#include <vector>
#include <shobjidl.h>

namespace ArnoldVinkCode
{
	/// <summary>
	/// Title: Select executable file...
	/// Filter: { { L"Executable files", L"*.exe" } }
	/// </summary>
	inline std::wstring filepicker_open(std::wstring title, std::vector<std::pair<std::wstring, std::wstring>> filefilters)
	{
		std::wstring importPath;
		try
		{
			auto pFileDialog = AVFin<IFileOpenDialog*>(AVFinMethod::ReleaseInterface);
			HRESULT hResult = CoCreateInstance(CLSID_FileOpenDialog, NULL, CLSCTX_ALL, IID_IFileOpenDialog, (void**)&pFileDialog.Get());
			if (SUCCEEDED(hResult))
			{
				//Set file filters
				int filterCount = filefilters.size();
				std::vector<COMDLG_FILTERSPEC> filterSpec(filterCount);
				for (int i = 0; i < filterCount; i++)
				{
					filterSpec[i].pszName = filefilters[i].first.c_str();
					filterSpec[i].pszSpec = filefilters[i].second.c_str();
				}

				//Set file dialog
				pFileDialog.Get()->SetTitle(title.c_str());
				pFileDialog.Get()->SetFileTypes(filterSpec.size(), filterSpec.data());
				pFileDialog.Get()->SetOptions(FOS_PATHMUSTEXIST | FOS_FILEMUSTEXIST);

				//Show file dialog
				hResult = pFileDialog.Get()->Show(NULL);

				//Get file dialog result
				if (SUCCEEDED(hResult))
				{
					auto pShellItem = AVFin<IShellItem*>(AVFinMethod::ReleaseInterface);
					hResult = pFileDialog.Get()->GetResult(&pShellItem.Get());
					if (SUCCEEDED(hResult))
					{
						PWSTR pszFilePath;
						hResult = pShellItem.Get()->GetDisplayName(SIGDN_FILESYSPATH, &pszFilePath);
						if (SUCCEEDED(hResult))
						{
							importPath = pszFilePath;
						}
					}
				}
			}

			//Return result
			return importPath;
		}
		catch (...)
		{
			//Return result
			return importPath;
		}
	}

	/// <summary>
	/// Title: Select executable files...
	/// Filter: { { L"Executable files", L"*.exe" } }
	/// </summary>
	inline std::vector<std::wstring> filepicker_open_multi(std::wstring title, std::vector<std::pair<std::wstring, std::wstring>> filefilters)
	{
		std::vector<std::wstring> importPaths;
		try
		{
			auto pFileDialog = AVFin<IFileOpenDialog*>(AVFinMethod::ReleaseInterface);
			HRESULT hResult = CoCreateInstance(CLSID_FileOpenDialog, NULL, CLSCTX_ALL, IID_IFileOpenDialog, (void**)&pFileDialog.Get());
			if (SUCCEEDED(hResult))
			{
				//Set file filters
				int filterCount = filefilters.size();
				std::vector<COMDLG_FILTERSPEC> filterSpec(filterCount);
				for (int i = 0; i < filterCount; i++)
				{
					filterSpec[i].pszName = filefilters[i].first.c_str();
					filterSpec[i].pszSpec = filefilters[i].second.c_str();
				}

				//Set file dialog
				pFileDialog.Get()->SetTitle(title.c_str());
				pFileDialog.Get()->SetFileTypes(filterSpec.size(), filterSpec.data());
				pFileDialog.Get()->SetOptions(FOS_PATHMUSTEXIST | FOS_FILEMUSTEXIST | FOS_ALLOWMULTISELECT);

				//Show file dialog
				hResult = pFileDialog.Get()->Show(NULL);

				//Get file dialog result
				if (SUCCEEDED(hResult))
				{
					auto pShellItemArray = AVFin<IShellItemArray*>(AVFinMethod::ReleaseInterface);
					hResult = pFileDialog.Get()->GetResults(&pShellItemArray.Get());
					if (SUCCEEDED(hResult))
					{
						DWORD itemCount;
						hResult = pShellItemArray.Get()->GetCount(&itemCount);
						for (DWORD i = 0; i < itemCount; i++)
						{
							auto pShellItem = AVFin<IShellItem*>(AVFinMethod::ReleaseInterface);
							hResult = pShellItemArray.Get()->GetItemAt(i, &pShellItem.Get());
							if (SUCCEEDED(hResult))
							{
								PWSTR pszFilePath;
								hResult = pShellItem.Get()->GetDisplayName(SIGDN_FILESYSPATH, &pszFilePath);
								if (SUCCEEDED(hResult))
								{
									importPaths.push_back(pszFilePath);
								}
							}
						}
					}
				}
			}

			//Return result
			return importPaths;
		}
		catch (...)
		{
			//Return result
			return importPaths;
		}
	}

	/// <summary>
	/// Title: Export file...
	/// Filter: { { L"Text file", L"*.txt" }, { L"Document file", L"*.doc" } }
	/// </summary>
	inline std::wstring filepicker_save(std::wstring title, std::vector<std::pair<std::wstring, std::wstring>> filefilters)
	{
		std::wstring exportPath;
		try
		{
			auto pFileDialog = AVFin<IFileOpenDialog*>(AVFinMethod::ReleaseInterface);
			HRESULT hResult = CoCreateInstance(CLSID_FileSaveDialog, NULL, CLSCTX_ALL, IID_IFileSaveDialog, (void**)&pFileDialog.Get());
			if (SUCCEEDED(hResult))
			{
				//Set file filters
				int filterCount = filefilters.size();
				std::vector<COMDLG_FILTERSPEC> filterSpec(filterCount);
				for (int i = 0; i < filterCount; i++)
				{
					filterSpec[i].pszName = filefilters[i].first.c_str();
					filterSpec[i].pszSpec = filefilters[i].second.c_str();
				}

				//Set file dialog
				pFileDialog.Get()->SetTitle(title.c_str());
				pFileDialog.Get()->SetFileTypes(filterSpec.size(), filterSpec.data());
				pFileDialog.Get()->SetDefaultExtension(L"");
				pFileDialog.Get()->SetOptions(FOS_OVERWRITEPROMPT);

				//Show file dialog
				hResult = pFileDialog.Get()->Show(NULL);

				//Get file dialog result
				if (SUCCEEDED(hResult))
				{
					auto pShellItem = AVFin<IShellItem*>(AVFinMethod::ReleaseInterface);
					hResult = pFileDialog.Get()->GetResult(&pShellItem.Get());
					if (SUCCEEDED(hResult))
					{
						PWSTR pszFilePath;
						hResult = pShellItem.Get()->GetDisplayName(SIGDN_FILESYSPATH, &pszFilePath);
						if (SUCCEEDED(hResult))
						{
							exportPath = pszFilePath;
						}
					}
				}
			}

			//Return result
			return exportPath;
		}
		catch (...)
		{
			//Return result
			return exportPath;
		}
	}
}