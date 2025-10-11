#pragma once
//HKEY_CLASSES_ROOT, HKEY_CURRENT_USER, HKEY_LOCAL_MACHINE, HKEY_USERS, HKEY_CURRENT_CONFIG

//Check registry key exists
static bool RegistryCheck(HKEY hKey, std::wstring subKey)
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
			AVDebugWriteLine("Failed to open registry key: " << lRes << " / " << subKey);
			return false;
		}
		else
		{
			//Return result
			AVDebugWriteLine("Registry key exists: " << subKey);
			return true;
		}
	}
	catch (...) {}
	return false;
}

//Check registry value exists
static bool RegistryCheck(HKEY hKey, std::wstring subKey, std::wstring valueName)
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
			AVDebugWriteLine("Failed to open registry key: " << lRes << " / " << subKey);
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

//Set string registry value
static bool RegistrySet(HKEY hKey, std::wstring subKey, std::wstring valueName, std::wstring valueSet)
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
			AVDebugWriteLine("Failed to open registry key: " << lRes << " / " << subKey);
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
static bool RegistrySet(HKEY hKey, std::wstring subKey, std::wstring valueName, DWORD valueSet)
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
			AVDebugWriteLine("Failed to open registry key: " << lRes << " / " << subKey);
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
static bool RegistrySet(HKEY hKey, std::wstring subKey, std::wstring valueName, std::vector<BYTE> valueSet)
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
			AVDebugWriteLine("Failed to open registry key: " << lRes << " / " << subKey);
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
static std::wstring RegistryGetString(HKEY hKey, std::wstring subKey, std::wstring valueName)
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
			AVDebugWriteLine("Failed to open registry key: " << lRes << " / " << subKey);
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
static std::optional<DWORD> RegistryGetDword(HKEY hKey, std::wstring subKey, std::wstring valueName)
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
			AVDebugWriteLine("Failed to open registry key: " << lRes << " / " << subKey);
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
static std::vector<BYTE> RegistryGetBinary(HKEY hKey, std::wstring subKey, std::wstring valueName)
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
			AVDebugWriteLine("Failed to open registry key: " << lRes << " / " << subKey);
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