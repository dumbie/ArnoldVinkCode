#pragma once
#include <windows.h>
#include "AVDebug.h"
#include "AVFinally.h"
//HKEY_CLASSES_ROOT, HKEY_CURRENT_USER, HKEY_LOCAL_MACHINE, HKEY_USERS, HKEY_CURRENT_CONFIG

namespace ArnoldVinkCode
{
	//Check registry key exists
	inline bool RegistryCheck(HKEY hKey, std::wstring subKey)
	{
		HKEY hOpenKey;
		AVFinallySafe(
			{
				RegCloseKey(hOpenKey);
			});
		try
		{
			//Open registry
			LSTATUS lRes = RegOpenKeyExW(hKey, subKey.c_str(), NULL, KEY_READ, &hOpenKey);
			if (lRes != ERROR_SUCCESS)
			{
				//Return result
				AVDebugWriteLine("Failed to open registry sub key: " << lRes << " / " << subKey);
				return false;
			}
			else
			{
				//Return result
				AVDebugWriteLine("Registry sub key exists: " << subKey);
				return true;
			}
		}
		catch (...) {}
		return false;
	}

	//Check registry value exists
	inline bool RegistryCheck(HKEY hKey, std::wstring subKey, std::wstring valueName)
	{
		HKEY hOpenKey;
		AVFinallySafe(
			{
				RegCloseKey(hOpenKey);
			});
		try
		{
			//Open registry
			LSTATUS lRes = RegOpenKeyExW(hKey, subKey.c_str(), NULL, KEY_READ, &hOpenKey);
			if (lRes != ERROR_SUCCESS)
			{
				//Return result
				AVDebugWriteLine("Failed to open registry sub key: " << lRes << " / " << subKey);
				return false;
			}

			//Get value from registry
			lRes = RegQueryValueExW(hOpenKey, valueName.c_str(), NULL, NULL, NULL, NULL);
			if (lRes != ERROR_SUCCESS)
			{
				//Return result
				AVDebugWriteLine("Failed to get value from registry: " << valueName);
				return false;
			}
			else
			{
				//Return result
				AVDebugWriteLine("Registry value exists: " << valueName);
				return true;
			}
		}
		catch (...) {}
		return false;
	}

	//Create registry sub key
	inline bool RegistryCreate(HKEY hKey, std::wstring subKey)
	{
		HKEY hOpenKey;
		AVFinallySafe(
			{
				RegCloseKey(hOpenKey);
			});
		try
		{
			//Create registry sub key
			LSTATUS lRes = RegCreateKeyW(hKey, subKey.c_str(), &hOpenKey);
			if (lRes != ERROR_SUCCESS)
			{
				AVDebugWriteLine("Failed to create registry sub key: " << lRes << " / " << subKey);
				return false;
			}

			//Return result
			AVDebugWriteLine("Created registry sub key: " << subKey);
			return true;
		}
		catch (...) {}
		return false;
	}

	//Delete registry sub key
	inline bool RegistryDelete(HKEY hKey, std::wstring subKey)
	{
		try
		{
			//Delete registry sub key
			LSTATUS lRes = RegDeleteTreeW(hKey, subKey.c_str());
			if (lRes != ERROR_SUCCESS)
			{
				AVDebugWriteLine("Failed to delete registry sub key: " << lRes << " / " << subKey);
				return false;
			}

			//Return result
			AVDebugWriteLine("Deleted registry sub key: " << subKey);
			return true;
		}
		catch (...) {}
		return false;
	}

	//Delete registry value
	inline bool RegistryDelete(HKEY hKey, std::wstring subKey, std::wstring valueName)
	{
		HKEY hOpenKey;
		AVFinallySafe(
			{
				RegCloseKey(hOpenKey);
			});
		try
		{
			//Open registry
			LSTATUS lRes = RegOpenKeyExW(hKey, subKey.c_str(), NULL, KEY_WRITE, &hOpenKey);
			if (lRes != ERROR_SUCCESS)
			{
				AVDebugWriteLine("Failed to open registry sub key: " << lRes << " / " << subKey);
				return false;
			}

			//Delete registry value
			lRes = RegDeleteValueW(hOpenKey, valueName.c_str());
			if (lRes != ERROR_SUCCESS)
			{
				AVDebugWriteLine("Failed to delete registry value: " << lRes << " / " << valueName);
				return false;
			}

			//Return result
			AVDebugWriteLine("Deleted registry value: " << valueName);
			return true;
		}
		catch (...) {}
		return false;
	}

	//Set string registry value
	inline bool RegistrySet(HKEY hKey, std::wstring subKey, std::wstring valueName, std::wstring valueSet)
	{
		HKEY hOpenKey;
		AVFinallySafe(
			{
				RegCloseKey(hOpenKey);
			});
		try
		{
			//Create registry sub key
			RegistryCreate(hKey, subKey);

			//Open registry
			LSTATUS lRes = RegOpenKeyExW(hKey, subKey.c_str(), NULL, KEY_WRITE, &hOpenKey);
			if (lRes != ERROR_SUCCESS)
			{
				AVDebugWriteLine("Failed to open registry sub key: " << lRes << " / " << subKey);
				return false;
			}

			//Set value to registry
			DWORD valueSize = valueSet.size() * sizeof(WCHAR);
			lRes = RegSetValueExW(hOpenKey, valueName.c_str(), NULL, REG_SZ, (BYTE*)valueSet.c_str(), valueSize);
			if (lRes != ERROR_SUCCESS)
			{
				AVDebugWriteLine("Failed to set value to registry: " << valueName << " / " << valueSet);
				return false;
			}

			//Return result
			AVDebugWriteLine("Set registry value: " << valueName << " / " << valueSet);
			return true;
		}
		catch (...) {}
		return false;
	}

	//Set dword registry value
	inline bool RegistrySet(HKEY hKey, std::wstring subKey, std::wstring valueName, DWORD valueSet)
	{
		HKEY hOpenKey;
		AVFinallySafe(
			{
				RegCloseKey(hOpenKey);
			});
		try
		{
			//Create registry sub key
			RegistryCreate(hKey, subKey);

			//Open registry
			LSTATUS lRes = RegOpenKeyExW(hKey, subKey.c_str(), NULL, KEY_WRITE, &hOpenKey);
			if (lRes != ERROR_SUCCESS)
			{
				AVDebugWriteLine("Failed to open registry sub key: " << lRes << " / " << subKey);
				return false;
			}

			//Set value to registry
			DWORD valueSize = sizeof(valueSet);
			lRes = RegSetValueExW(hOpenKey, valueName.c_str(), NULL, REG_DWORD, (BYTE*)&valueSet, valueSize);
			if (lRes != ERROR_SUCCESS)
			{
				AVDebugWriteLine("Failed to set value to registry: " << valueName << " / " << valueSet);
				return false;
			}

			//Return result
			AVDebugWriteLine("Set registry value: " << valueName << " / " << valueSet);
			return true;
		}
		catch (...) {}
		return false;
	}

	//Set binary registry value
	inline bool RegistrySet(HKEY hKey, std::wstring subKey, std::wstring valueName, std::vector<BYTE> valueSet)
	{
		HKEY hOpenKey;
		AVFinallySafe(
			{
				RegCloseKey(hOpenKey);
			});
		try
		{
			//Create registry sub key
			RegistryCreate(hKey, subKey);

			//Open registry
			LSTATUS lRes = RegOpenKeyExW(hKey, subKey.c_str(), NULL, KEY_WRITE, &hOpenKey);
			if (lRes != ERROR_SUCCESS)
			{
				AVDebugWriteLine("Failed to open registry sub key: " << lRes << " / " << subKey);
				return false;
			}

			//Set value to registry
			DWORD valueSize = valueSet.size();
			lRes = RegSetValueExW(hOpenKey, valueName.c_str(), NULL, REG_BINARY, valueSet.data(), valueSize);
			if (lRes != ERROR_SUCCESS)
			{
				AVDebugWriteLine("Failed to set value to registry: " << valueName);
				return false;
			}

			//Return result
			AVDebugWriteLine("Set registry value: " << valueName);
			return true;
		}
		catch (...) {}
		return false;
	}

	//Get string registry value
	inline std::wstring RegistryGetString(HKEY hKey, std::wstring subKey, std::wstring valueName)
	{
		HKEY hOpenKey;
		AVFinallySafe(
			{
				RegCloseKey(hOpenKey);
			});
		try
		{
			//Open registry
			LSTATUS lRes = RegOpenKeyExW(hKey, subKey.c_str(), NULL, KEY_READ, &hOpenKey);
			if (lRes != ERROR_SUCCESS)
			{
				AVDebugWriteLine("Failed to open registry sub key: " << lRes << " / " << subKey);
				return L"";
			}

			//Get value from registry
			WCHAR buffer[1024];
			DWORD bufferSize = sizeof(buffer);
			lRes = RegQueryValueExW(hOpenKey, valueName.c_str(), NULL, NULL, (BYTE*)buffer, &bufferSize);
			if (lRes != ERROR_SUCCESS)
			{
				AVDebugWriteLine("Failed to get value from registry: " << valueName);
				return L"";
			}

			//Return result
			return std::wstring(buffer);
		}
		catch (...) {}
		return L"";
	}

	//Get dword registry value
	inline std::optional<DWORD> RegistryGetDword(HKEY hKey, std::wstring subKey, std::wstring valueName)
	{
		HKEY hOpenKey;
		AVFinallySafe(
			{
				RegCloseKey(hOpenKey);
			});
		try
		{
			//Open registry
			LSTATUS lRes = RegOpenKeyExW(hKey, subKey.c_str(), NULL, KEY_READ, &hOpenKey);
			if (lRes != ERROR_SUCCESS)
			{
				AVDebugWriteLine("Failed to open registry sub key: " << lRes << " / " << subKey);
				return std::nullopt;
			}

			//Get value from registry
			DWORD buffer = 0;
			DWORD bufferSize = sizeof(buffer);
			lRes = RegQueryValueExW(hOpenKey, valueName.c_str(), NULL, NULL, (BYTE*)&buffer, &bufferSize);
			if (lRes != ERROR_SUCCESS)
			{
				AVDebugWriteLine("Failed to get value from registry: " << valueName);
				return std::nullopt;
			}

			//Return result
			return buffer;
		}
		catch (...) {}
		return std::nullopt;
	}

	//Get binary registry value
	inline std::vector<BYTE> RegistryGetBinary(HKEY hKey, std::wstring subKey, std::wstring valueName)
	{
		HKEY hOpenKey;
		AVFinallySafe(
			{
				RegCloseKey(hOpenKey);
			});
		try
		{
			//Open registry
			LSTATUS lRes = RegOpenKeyExW(hKey, subKey.c_str(), NULL, KEY_READ, &hOpenKey);
			if (lRes != ERROR_SUCCESS)
			{
				AVDebugWriteLine("Failed to open registry sub key: " << lRes << " / " << subKey);
				return std::vector<BYTE>();
			}

			//Get value from registry
			std::vector<BYTE> buffer = std::vector<BYTE>(1024);
			DWORD bufferSize = buffer.size();
			lRes = RegQueryValueExW(hOpenKey, valueName.c_str(), NULL, NULL, buffer.data(), &bufferSize);
			if (lRes != ERROR_SUCCESS)
			{
				AVDebugWriteLine("Failed to get value from registry: " << valueName);
				return std::vector<BYTE>();
			}

			//Resize buffer vector
			buffer.resize(bufferSize);

			//Return result
			return buffer;
		}
		catch (...) {}
		return std::vector<BYTE>();
	}
}