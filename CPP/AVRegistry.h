#pragma once
//HKEY_CLASSES_ROOT, HKEY_CURRENT_USER, HKEY_LOCAL_MACHINE, HKEY_USERS, HKEY_CURRENT_CONFIG

//Check registry value exists
static bool RegistryCheck() {}

//Set string registry value
static bool RegistrySet(HKEY hKey, std::wstring subKey, std::wstring valueName, std::wstring valueSet)
{
	try
	{
		//Open registry
		HKEY pRes;
		LSTATUS lRes = RegOpenKeyExW(hKey, subKey.c_str(), NULL, KEY_WRITE, &pRes);
		if (lRes != ERROR_SUCCESS)
		{
			AVDebugWriteLine("Failed to open registry key: " << subKey);
			return false;
		}

		//Set value to registry
		DWORD valueSize = valueSet.size() * sizeof(WCHAR);
		lRes = RegSetValueExW(pRes, valueName.c_str(), NULL, REG_SZ, (BYTE*)valueSet.c_str(), valueSize);
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
	try
	{
		//Open registry
		HKEY pRes;
		LSTATUS lRes = RegOpenKeyExW(hKey, subKey.c_str(), NULL, KEY_WRITE, &pRes);
		if (lRes != ERROR_SUCCESS)
		{
			AVDebugWriteLine("Failed to open registry key: " << subKey);
			return false;
		}

		//Set value to registry
		DWORD valueSize = valueSet.size();
		lRes = RegSetValueExW(pRes, valueName.c_str(), NULL, REG_BINARY, valueSet.data(), valueSize);
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
	try
	{
		//Open registry
		HKEY pRes;
		LSTATUS lRes = RegOpenKeyExW(hKey, subKey.c_str(), NULL, KEY_READ, &pRes);
		if (lRes != ERROR_SUCCESS)
		{
			AVDebugWriteLine("Failed to open registry key: " << subKey);
			return L"";
		}

		//Get value from registry
		WCHAR buffer[1024];
		DWORD bufferSize = sizeof(buffer);
		lRes = RegQueryValueExW(pRes, valueName.c_str(), NULL, NULL, (BYTE*)buffer, &bufferSize);
		if (lRes != ERROR_SUCCESS)
		{
			AVDebugWriteLine("Failed to get value from registry: " << valueName);
			return L"";
		}

		//Close registry
		RegCloseKey(pRes);

		//Return result
		return std::wstring(buffer);
	}
	catch (...) {}
	return L"";
}

//Get binary registry value
static std::vector<BYTE> RegistryGetBinary(HKEY hKey, std::wstring subKey, std::wstring valueName)
{
	try
	{
		//Open registry
		HKEY pRes;
		LSTATUS lRes = RegOpenKeyExW(hKey, subKey.c_str(), NULL, KEY_READ, &pRes);
		if (lRes != ERROR_SUCCESS)
		{
			AVDebugWriteLine("Failed to open registry key: " << subKey);
			return std::vector<BYTE>();
		}

		//Get value from registry
		std::vector<BYTE> buffer = std::vector<BYTE>(1024);
		DWORD bufferSize = buffer.size();
		lRes = RegQueryValueExW(pRes, valueName.c_str(), NULL, NULL, buffer.data(), &bufferSize);
		if (lRes != ERROR_SUCCESS)
		{
			AVDebugWriteLine("Failed to get value from registry: " << valueName);
			return std::vector<BYTE>();
		}

		//Resize buffer vector
		buffer.resize(bufferSize);

		//Close registry
		RegCloseKey(pRes);

		//Return result
		return buffer;
	}
	catch (...) {}
	return std::vector<BYTE>();
}