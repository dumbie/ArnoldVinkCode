#pragma once
#include <windows.h>
#include "AVDebug.h"
#include "AVFinally.h"

namespace ArnoldVinkCode
{
	enum class HKEY_ENUM : ULONG_PTR
	{
		CLASSES_ROOT = reinterpret_cast<ULONG_PTR>(HKEY_CLASSES_ROOT),
		CURRENT_USER = reinterpret_cast<ULONG_PTR>(HKEY_CURRENT_USER),
		LOCAL_MACHINE = reinterpret_cast<ULONG_PTR>(HKEY_LOCAL_MACHINE),
		USERS = reinterpret_cast<ULONG_PTR>(HKEY_USERS),
		PERFORMANCE_DATA = reinterpret_cast<ULONG_PTR>(HKEY_PERFORMANCE_DATA),
		PERFORMANCE_TEXT = reinterpret_cast<ULONG_PTR>(HKEY_PERFORMANCE_TEXT),
		PERFORMANCE_NLSTEXT = reinterpret_cast<ULONG_PTR>(HKEY_PERFORMANCE_NLSTEXT),
		CURRENT_CONFIG = reinterpret_cast<ULONG_PTR>(HKEY_CURRENT_CONFIG),
		DYN_DATA = reinterpret_cast<ULONG_PTR>(HKEY_DYN_DATA),
		CURRENT_USER_LOCAL_SETTINGS = reinterpret_cast<ULONG_PTR>(HKEY_CURRENT_USER_LOCAL_SETTINGS),
	};

	enum class REGTYPE_ENUM : ULONG
	{
		NONE = REG_NONE,
		SZ = REG_SZ,
		EXPAND_SZ = REG_EXPAND_SZ,
		BINARY = REG_BINARY,
		DWORD = REG_DWORD,
		DWORD_LITTLE_ENDIAN = REG_DWORD_LITTLE_ENDIAN,
		DWORD_BIG_ENDIAN = REG_DWORD_BIG_ENDIAN,
		LINK = REG_LINK,
		MULTI_SZ = REG_MULTI_SZ,
		RESOURCE_LIST = REG_RESOURCE_LIST,
		FULL_RESOURCE_DESCRIPTOR = REG_FULL_RESOURCE_DESCRIPTOR,
		RESOURCE_REQUIREMENTS_LIST = REG_RESOURCE_REQUIREMENTS_LIST,
		QWORD = REG_QWORD,
		QWORD_LITTLE_ENDIAN = REG_QWORD_LITTLE_ENDIAN
	};

	//Check registry key exists
	inline bool RegistryCheck(HKEY_ENUM hKey, std::wstring subKey)
	{
		try
		{
			//Open registry
			auto hOpenKey = AVFin<HKEY>(AVFinMethod::RegCloseKey);
			LSTATUS lRes = RegOpenKeyExW((HKEY)hKey, subKey.c_str(), NULL, KEY_READ, &hOpenKey.Get());
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
	inline bool RegistryCheck(HKEY_ENUM hKey, std::wstring subKey, std::wstring valueName)
	{
		try
		{
			//Open registry
			auto hOpenKey = AVFin<HKEY>(AVFinMethod::RegCloseKey);
			LSTATUS lRes = RegOpenKeyExW((HKEY)hKey, subKey.c_str(), NULL, KEY_READ, &hOpenKey.Get());
			if (lRes != ERROR_SUCCESS)
			{
				//Return result
				AVDebugWriteLine("Failed to open registry sub key: " << lRes << " / " << subKey);
				return false;
			}

			//Get value from registry
			lRes = RegQueryValueExW(hOpenKey.Get(), valueName.c_str(), NULL, NULL, NULL, NULL);
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

	//Get registry value type
	inline REGTYPE_ENUM RegistryType(HKEY_ENUM hKey, std::wstring subKey, std::wstring valueName)
	{
		try
		{
			//Open registry
			auto hOpenKey = AVFin<HKEY>(AVFinMethod::RegCloseKey);
			LSTATUS lRes = RegOpenKeyExW((HKEY)hKey, subKey.c_str(), NULL, KEY_READ, &hOpenKey.Get());
			if (lRes != ERROR_SUCCESS)
			{
				//Return result
				AVDebugWriteLine("Failed to open registry sub key: " << lRes << " / " << subKey);
				return REGTYPE_ENUM::NONE;
			}

			//Query registry
			ULONG keyType = REG_NONE;
			lRes = RegQueryValueExW(hOpenKey.Get(), valueName.c_str(), NULL, &keyType, NULL, NULL);
			if (lRes != ERROR_SUCCESS)
			{
				//Return result
				AVDebugWriteLine("Failed to get registry value type: " << lRes << " / " << valueName);
				return REGTYPE_ENUM::NONE;
			}
			else
			{
				//Return result
				AVDebugWriteLine("Registry value type: " << keyType << " / " << valueName);
				return (REGTYPE_ENUM)keyType;
			}
		}
		catch (...) {}
		return REGTYPE_ENUM::NONE;
	}

	//Create registry sub key
	inline bool RegistryCreate(HKEY_ENUM hKey, std::wstring subKey)
	{
		try
		{
			//Create registry sub key
			auto hOpenKey = AVFin<HKEY>(AVFinMethod::RegCloseKey);
			LSTATUS lRes = RegCreateKeyW((HKEY)hKey, subKey.c_str(), &hOpenKey.Get());
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
	inline bool RegistryDelete(HKEY_ENUM hKey, std::wstring subKey)
	{
		try
		{
			//Delete registry sub key
			LSTATUS lRes = RegDeleteTreeW((HKEY)hKey, subKey.c_str());
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
	inline bool RegistryDelete(HKEY_ENUM hKey, std::wstring subKey, std::wstring valueName)
	{
		try
		{
			//Open registry
			auto hOpenKey = AVFin<HKEY>(AVFinMethod::RegCloseKey);
			LSTATUS lRes = RegOpenKeyExW((HKEY)hKey, subKey.c_str(), NULL, KEY_WRITE, &hOpenKey.Get());
			if (lRes != ERROR_SUCCESS)
			{
				AVDebugWriteLine("Failed to open registry sub key: " << lRes << " / " << subKey);
				return false;
			}

			//Delete registry value
			lRes = RegDeleteValueW(hOpenKey.Get(), valueName.c_str());
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
	inline bool RegistrySet(HKEY_ENUM hKey, std::wstring subKey, std::wstring valueName, std::wstring valueSet)
	{
		try
		{
			//Create registry sub key
			RegistryCreate(hKey, subKey);

			//Open registry
			auto hOpenKey = AVFin<HKEY>(AVFinMethod::RegCloseKey);
			LSTATUS lRes = RegOpenKeyExW((HKEY)hKey, subKey.c_str(), NULL, KEY_WRITE, &hOpenKey.Get());
			if (lRes != ERROR_SUCCESS)
			{
				AVDebugWriteLine("Failed to open registry sub key: " << lRes << " / " << subKey);
				return false;
			}

			//Set value to registry
			DWORD valueSize = valueSet.size() * sizeof(WCHAR);
			lRes = RegSetValueExW(hOpenKey.Get(), valueName.c_str(), NULL, REG_SZ, (BYTE*)valueSet.c_str(), valueSize);
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
	inline bool RegistrySet(HKEY_ENUM hKey, std::wstring subKey, std::wstring valueName, DWORD valueSet)
	{
		try
		{
			//Create registry sub key
			RegistryCreate(hKey, subKey);

			//Open registry
			auto hOpenKey = AVFin<HKEY>(AVFinMethod::RegCloseKey);
			LSTATUS lRes = RegOpenKeyExW((HKEY)hKey, subKey.c_str(), NULL, KEY_WRITE, &hOpenKey.Get());
			if (lRes != ERROR_SUCCESS)
			{
				AVDebugWriteLine("Failed to open registry sub key: " << lRes << " / " << subKey);
				return false;
			}

			//Set value to registry
			DWORD valueSize = sizeof(valueSet);
			lRes = RegSetValueExW(hOpenKey.Get(), valueName.c_str(), NULL, REG_DWORD, (BYTE*)&valueSet, valueSize);
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
	inline bool RegistrySet(HKEY_ENUM hKey, std::wstring subKey, std::wstring valueName, std::vector<BYTE> valueSet)
	{
		try
		{
			//Create registry sub key
			RegistryCreate(hKey, subKey);

			//Open registry
			auto hOpenKey = AVFin<HKEY>(AVFinMethod::RegCloseKey);
			LSTATUS lRes = RegOpenKeyExW((HKEY)hKey, subKey.c_str(), NULL, KEY_WRITE, &hOpenKey.Get());
			if (lRes != ERROR_SUCCESS)
			{
				AVDebugWriteLine("Failed to open registry sub key: " << lRes << " / " << subKey);
				return false;
			}

			//Set value to registry
			DWORD valueSize = valueSet.size();
			lRes = RegSetValueExW(hOpenKey.Get(), valueName.c_str(), NULL, REG_BINARY, valueSet.data(), valueSize);
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
	inline std::wstring RegistryGetString(HKEY_ENUM hKey, std::wstring subKey, std::wstring valueName)
	{
		try
		{
			//Open registry
			auto hOpenKey = AVFin<HKEY>(AVFinMethod::RegCloseKey);
			LSTATUS lRes = RegOpenKeyExW((HKEY)hKey, subKey.c_str(), NULL, KEY_READ, &hOpenKey.Get());
			if (lRes != ERROR_SUCCESS)
			{
				AVDebugWriteLine("Failed to open registry sub key: " << lRes << " / " << subKey);
				return L"";
			}

			//Get value from registry
			std::vector<BYTE> buffer(1024);
			DWORD bufferSize = buffer.size();
			lRes = RegQueryValueExW(hOpenKey.Get(), valueName.c_str(), NULL, NULL, buffer.data(), &bufferSize);
			if (lRes != ERROR_SUCCESS)
			{
				AVDebugWriteLine("Failed to get value from registry: " << valueName);
				return L"";
			}

			//Resize buffer vector
			buffer.resize(bufferSize);

			//Return result
			return std::wstring((WCHAR*)buffer.data(), bufferSize / sizeof(WCHAR));
		}
		catch (...) {}
		return L"";
	}

	//Get dword registry value
	inline std::optional<DWORD> RegistryGetDword(HKEY_ENUM hKey, std::wstring subKey, std::wstring valueName)
	{
		try
		{
			//Open registry
			auto hOpenKey = AVFin<HKEY>(AVFinMethod::RegCloseKey);
			LSTATUS lRes = RegOpenKeyExW((HKEY)hKey, subKey.c_str(), NULL, KEY_READ, &hOpenKey.Get());
			if (lRes != ERROR_SUCCESS)
			{
				AVDebugWriteLine("Failed to open registry sub key: " << lRes << " / " << subKey);
				return std::nullopt;
			}

			//Get value from registry
			DWORD buffer = 0;
			DWORD bufferSize = sizeof(buffer);
			lRes = RegQueryValueExW(hOpenKey.Get(), valueName.c_str(), NULL, NULL, (BYTE*)&buffer, &bufferSize);
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
	inline std::vector<BYTE> RegistryGetBinary(HKEY_ENUM hKey, std::wstring subKey, std::wstring valueName)
	{
		try
		{
			//Open registry
			auto hOpenKey = AVFin<HKEY>(AVFinMethod::RegCloseKey);
			LSTATUS lRes = RegOpenKeyExW((HKEY)hKey, subKey.c_str(), NULL, KEY_READ, &hOpenKey.Get());
			if (lRes != ERROR_SUCCESS)
			{
				AVDebugWriteLine("Failed to open registry sub key: " << lRes << " / " << subKey);
				return std::vector<BYTE>();
			}

			//Get value from registry
			std::vector<BYTE> buffer(1024);
			DWORD bufferSize = buffer.size();
			lRes = RegQueryValueExW(hOpenKey.Get(), valueName.c_str(), NULL, NULL, buffer.data(), &bufferSize);
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