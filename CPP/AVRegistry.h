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

	struct RegValue
	{
		std::optional<std::wstring> Name;
		std::optional<REGTYPE_ENUM> Type;
		std::optional<std::wstring> DataString; //REG_SZ, REG_EXPAND_SZ
		std::optional<uint32_t> DataDword; //REG_DWORD
		std::optional<uint64_t> DataQword; //REG_QWORD
		std::optional<std::vector<BYTE>> DataBinary; //REG_BINARY
		std::optional<std::vector<std::wstring>> DataMultiString; //REG_MULTI_SZ
	};

	//Open registry key
	inline HKEY RegistryOpenAuto(HKEY_ENUM hKey, std::wstring subKey, int keyFlag)
	{
		HKEY hOpenKey = nullptr;
		try
		{
			//Open registry 32bit
			LSTATUS lRes = RegOpenKeyExW((HKEY)hKey, subKey.c_str(), NULL, keyFlag | KEY_WOW64_32KEY, &hOpenKey);
			if (lRes == ERROR_SUCCESS)
			{
				//Return result
				//AVDebugWriteLine("Opened registry sub key 32bit: " << subKey);
				return hOpenKey;
			}

			//Open registry 64bit
			lRes = RegOpenKeyExW((HKEY)hKey, subKey.c_str(), NULL, keyFlag | KEY_WOW64_64KEY, &hOpenKey);
			if (lRes == ERROR_SUCCESS)
			{
				//Return result
				//AVDebugWriteLine("Opened registry sub key 64bit: " << subKey);
				return hOpenKey;
			}

			//Check result
			if (hOpenKey == nullptr)
			{
				//Return result
				AVDebugWriteLine("Failed auto open registry sub key: " << lRes << " / " << subKey);
			}
		}
		catch (...) {}
		return hOpenKey;
	}

	//Check registry key exists
	inline bool RegistryCheck(HKEY_ENUM hKey, std::wstring subKey)
	{
		try
		{
			//Open registry
			auto hOpenKey = AVFin<HKEY>(AVFinMethod::RegCloseKey, RegistryOpenAuto(hKey, subKey.c_str(), KEY_READ));
			if (hOpenKey.Get() == nullptr)
			{
				//Return result
				AVDebugWriteLine("Failed to open registry sub key: " << subKey);
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
			auto hOpenKey = AVFin<HKEY>(AVFinMethod::RegCloseKey, RegistryOpenAuto(hKey, subKey.c_str(), KEY_READ));
			if (hOpenKey.Get() == nullptr)
			{
				//Return result
				AVDebugWriteLine("Failed to open registry sub key: " << subKey);
				return false;
			}

			//Get value from registry
			LSTATUS lRes = RegQueryValueExW(hOpenKey.Get(), valueName.c_str(), NULL, NULL, NULL, NULL);
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
			auto hOpenKey = AVFin<HKEY>(AVFinMethod::RegCloseKey, RegistryOpenAuto(hKey, subKey.c_str(), KEY_READ));
			if (hOpenKey.Get() == nullptr)
			{
				//Return result
				AVDebugWriteLine("Failed to open registry sub key: " << subKey);
				return REGTYPE_ENUM::NONE;
			}

			//Query registry
			ULONG keyType = REG_NONE;
			LSTATUS lRes = RegQueryValueExW(hOpenKey.Get(), valueName.c_str(), NULL, &keyType, NULL, NULL);
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
			auto hOpenKey = AVFin<HKEY>(AVFinMethod::RegCloseKey, RegistryOpenAuto(hKey, subKey.c_str(), KEY_WRITE));
			if (hOpenKey.Get() == nullptr)
			{
				AVDebugWriteLine("Failed to open registry sub key: " << subKey);
				return false;
			}

			//Delete registry value
			LSTATUS lRes = RegDeleteValueW(hOpenKey.Get(), valueName.c_str());
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
			auto hOpenKey = AVFin<HKEY>(AVFinMethod::RegCloseKey, RegistryOpenAuto(hKey, subKey.c_str(), KEY_WRITE));
			if (hOpenKey.Get() == nullptr)
			{
				AVDebugWriteLine("Failed to open registry sub key: " << subKey);
				return false;
			}

			//Set value to registry
			DWORD valueSize = valueSet.size() * sizeof(WCHAR);
			LSTATUS lRes = RegSetValueExW(hOpenKey.Get(), valueName.c_str(), NULL, REG_SZ, (BYTE*)valueSet.c_str(), valueSize);
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
			auto hOpenKey = AVFin<HKEY>(AVFinMethod::RegCloseKey, RegistryOpenAuto(hKey, subKey.c_str(), KEY_WRITE));
			if (hOpenKey.Get() == nullptr)
			{
				AVDebugWriteLine("Failed to open registry sub key: " << subKey);
				return false;
			}

			//Set value to registry
			DWORD valueSize = sizeof(valueSet);
			LSTATUS lRes = RegSetValueExW(hOpenKey.Get(), valueName.c_str(), NULL, REG_DWORD, (BYTE*)&valueSet, valueSize);
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
			auto hOpenKey = AVFin<HKEY>(AVFinMethod::RegCloseKey, RegistryOpenAuto(hKey, subKey.c_str(), KEY_WRITE));
			if (hOpenKey.Get() == nullptr)
			{
				AVDebugWriteLine("Failed to open registry sub key: " << subKey);
				return false;
			}

			//Set value to registry
			DWORD valueSize = valueSet.size();
			LSTATUS lRes = RegSetValueExW(hOpenKey.Get(), valueName.c_str(), NULL, REG_BINARY, valueSet.data(), valueSize);
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
			auto hOpenKey = AVFin<HKEY>(AVFinMethod::RegCloseKey, RegistryOpenAuto(hKey, subKey.c_str(), KEY_READ));
			if (hOpenKey.Get() == nullptr)
			{
				AVDebugWriteLine("Failed to open registry sub key: " << subKey);
				return L"";
			}

			//Get value from registry
			std::vector<BYTE> buffer(1024);
			DWORD bufferSize = buffer.size();
			LSTATUS lRes = RegQueryValueExW(hOpenKey.Get(), valueName.c_str(), NULL, NULL, buffer.data(), &bufferSize);
			if (lRes != ERROR_SUCCESS)
			{
				AVDebugWriteLine("Failed to get value from registry: " << valueName);
				return L"";
			}

			//Resize buffer vector
			buffer.resize(bufferSize);

			//Return result
			WCHAR* stringData = reinterpret_cast<WCHAR*>(buffer.data());
			return std::wstring(stringData);
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
			auto hOpenKey = AVFin<HKEY>(AVFinMethod::RegCloseKey, RegistryOpenAuto(hKey, subKey.c_str(), KEY_READ));
			if (hOpenKey.Get() == nullptr)
			{
				AVDebugWriteLine("Failed to open registry sub key: " << subKey);
				return std::nullopt;
			}

			//Get value from registry
			DWORD buffer = 0;
			DWORD bufferSize = sizeof(buffer);
			LSTATUS lRes = RegQueryValueExW(hOpenKey.Get(), valueName.c_str(), NULL, NULL, (BYTE*)&buffer, &bufferSize);
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
			auto hOpenKey = AVFin<HKEY>(AVFinMethod::RegCloseKey, RegistryOpenAuto(hKey, subKey.c_str(), KEY_READ));
			if (hOpenKey.Get() == nullptr)
			{
				AVDebugWriteLine("Failed to open registry sub key: " << subKey);
				return std::vector<BYTE>();
			}

			//Get value from registry
			std::vector<BYTE> buffer(1024);
			DWORD bufferSize = buffer.size();
			LSTATUS lRes = RegQueryValueExW(hOpenKey.Get(), valueName.c_str(), NULL, NULL, buffer.data(), &bufferSize);
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

	//Get all registry values from subkey
	inline std::vector<RegValue> RegistryGetValuesAll(HKEY_ENUM hKey, std::wstring subKey)
	{
		std::vector<RegValue> regValues;
		try
		{
			//Open registry
			auto hOpenKey = AVFin<HKEY>(AVFinMethod::RegCloseKey, RegistryOpenAuto(hKey, subKey.c_str(), KEY_READ));
			if (hOpenKey.Get() == nullptr)
			{
				AVDebugWriteLine("Failed to open registry sub key: " << subKey);
				return regValues;
			}

			//Get key information from registry
			DWORD valueCount = 0;
			DWORD maxValueNameLength = 0;
			DWORD maxValueDataLength = 0;
			LSTATUS lRes = RegQueryInfoKeyW(hOpenKey.Get(), NULL, NULL, NULL, NULL, NULL, NULL, &valueCount, &maxValueNameLength, &maxValueDataLength, NULL, NULL);
			if (lRes != ERROR_SUCCESS)
			{
				AVDebugWriteLine("Failed to get registry key information: " << lRes << " / " << subKey);
				return regValues;
			}

			//Get values from registry
			for (DWORD i = 0; i < valueCount; i++)
			{
				//Set value name and data buffers
				DWORD valueType = 0;
				DWORD valueNameLength = maxValueNameLength + 1;
				DWORD valueDataLength = maxValueDataLength;
				std::vector<WCHAR> valueName(valueNameLength);
				std::vector<BYTE> valueData(valueDataLength);

				//Get value name and data from registry
				lRes = RegEnumValueW(hOpenKey.Get(), i, valueName.data(), &valueNameLength, NULL, &valueType, valueData.data(), &valueDataLength);
				if (lRes != ERROR_SUCCESS)
				{
					continue;
				}

				//Convert data to registry value
				RegValue regValue{};
				regValue.Name = std::wstring(valueName.data());
				regValue.Type = (REGTYPE_ENUM)valueType;
				if (valueType == REG_SZ || valueType == REG_EXPAND_SZ)
				{
					WCHAR* stringData = reinterpret_cast<WCHAR*>(valueData.data());
					regValue.DataString = std::wstring(stringData);
				}
				else if (valueType == REG_DWORD)
				{
					regValue.DataDword = *reinterpret_cast<uint32_t*>(valueData.data());
				}
				else if (valueType == REG_QWORD)
				{
					regValue.DataQword = *reinterpret_cast<uint64_t*>(valueData.data());
				}
				else if (valueType == REG_BINARY)
				{
					regValue.DataBinary = std::vector<BYTE>(valueData.begin(), valueData.begin() + valueDataLength);
				}
				else if (valueType == REG_MULTI_SZ)
				{
					std::vector<std::wstring> stringsMulti{};
					WCHAR* stringsData = reinterpret_cast<WCHAR*>(valueData.data());
					while (*stringsData)
					{
						//Convert to string
						std::wstring string = stringsData;

						//Add string to list
						stringsMulti.push_back(string);

						//Move to next string
						stringsData += string.size() + 1;
					}
					regValue.DataMultiString = stringsMulti;
				}

				//Add registry value to list
				regValues.push_back(regValue);
			}

			//AVDebugWriteLine("Registry value count: " << valueCount << " / " << subKey);
		}
		catch (...) {}
		return regValues;
	}
}