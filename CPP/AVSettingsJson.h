#pragma once
#include <optional>
#include "AVDebug.h"
#include "AVFiles.h"
#include "nlohmann_json.hpp"

namespace ArnoldVinkCode
{
	class AVSettingsJson
	{
	private:
		//Variables
		std::wstring vSettingFilePath = L"";
		nlohmann::json vJToken = NULL;

		//Load settings from file
		bool FileLoad()
		{
			try
			{
				//Load json settings file
				std::wstring jsonText = file_to_string(vSettingFilePath);

				//Check json settings text
				if (wstring_empty_whitespace(jsonText))
				{
					AVDebugWriteLine("Failed reading settings file, falling back to empty.");
					jsonText = L"{}";
				}

				//Check corrupted settings file
				if (std::all_of(jsonText.begin(), jsonText.end(), [](wchar_t c) { return c == L'\0'; }))
				{
					AVDebugWriteLine("Settings file is corrupted, falling back to empty.");
					jsonText = L"{}";
				}

				//Parse json settings file
				vJToken = nlohmann::json::parse(jsonText);

				//Return result
				AVDebugWriteLine("Loaded settings file: " << vSettingFilePath.c_str());
				return true;
			}
			catch (...)
			{
				//Return result
				AVDebugWriteLine("Failed loading settings file: " << vSettingFilePath.c_str());
				return false;
			}
		}

		//Save settings to file
		bool FileSave()
		{
			try
			{
				//Convert json to string
				std::wstring jsonString = string_to_wstring(vJToken.dump());

				//Save settings file
				string_to_file(vSettingFilePath, jsonString);

				//Return result
				AVDebugWriteLine(L"Saved settings file: " << vSettingFilePath);
				return true;
			}
			catch (...)
			{
				//Return result
				AVDebugWriteLine(L"Failed saving settings file: " << vSettingFilePath);
				return false;
			}
		}

	public:
		//Initialize
		AVSettingsJson() {};
		AVSettingsJson(std::wstring settingFilePath)
		{
			try
			{
				vSettingFilePath = settingFilePath;
				FileLoad();
			}
			catch (...)
			{
				AVDebugWriteLine("Failed initializing settings.");
			}
		}

		//Check if setting exists
		bool Check(std::string settingName)
		{
			try
			{
				//Check setting
				return vJToken.contains(settingName);
			}
			catch (...)
			{
				//Return result
				AVDebugWriteLine("Failed checking setting: " << settingName.c_str());
				return false;
			}
		}

		//Remove setting
		bool Remove(std::string settingName)
		{
			try
			{
				//Remove setting
				vJToken.erase(settingName);
				AVDebugWriteLine("Removed setting: " << settingName.c_str());

				//Save setting file
				FileSave();

				//Return result
				return true;
			}
			catch (...)
			{
				//Return result
				AVDebugWriteLine("Failed removing setting: " << settingName.c_str());
				return false;
			}
		}

		//Load setting value
		/// <summary>
		/// std::optional<Type> var = set.Load<Type>("SettingName");
		/// if (var.has_value()) { var.value(); }
		/// </summary>
		template<typename T>
		std::optional<T> Load(std::string settingName)
		{
			try
			{
				//Return result
				return vJToken[settingName];
			}
			catch (...)
			{
				//Return result
				AVDebugWriteLine("Failed loading setting: " << settingName.c_str());
				return std::nullopt;
			}
		}

		//Set setting value
		bool Set(std::string settingName, auto settingValue)
		{
			try
			{
				//Set setting value
				vJToken[settingName] = settingValue;
				AVDebugWriteLine("Setted value: " << settingName.c_str());

				//Save setting file
				FileSave();

				//Return result
				return true;
			}
			catch (...)
			{
				//Return result
				AVDebugWriteLine("Failed setting value: " << settingName.c_str());
				return false;
			}
		}
	};
}