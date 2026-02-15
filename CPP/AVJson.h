#pragma once
#include <string>
#include <vector>

NLOHMANN_JSON_NAMESPACE_BEGIN
template<typename T>
struct adl_serializer<std::optional<T>>
{
	static void to_json(json& jsonData, const std::optional<T>& opt)
	{
		if (opt.has_value())
		{
			jsonData = opt.value();
		}
		else
		{
			jsonData = nullptr;
		}
	}

	static void from_json(const json& jsonData, std::optional<T>& opt)
	{
		if (jsonData.is_null())
		{
			opt = std::nullopt;
		}
		else
		{
			opt = jsonData.get<T>();
		}
	}
};
NLOHMANN_JSON_NAMESPACE_END

namespace ArnoldVinkCode
{
	//Note: make sure your struct or class uses NLOHMANN_DEFINE_TYPE_INTRUSIVE_WITH_DEFAULT() or NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE_WITH_DEFAULT()
	//Unfortunately this only allows 64 variables in the struct or class

	inline void json_remove_null_values(nlohmann::json& jsonData)
	{
		try
		{
			if (!jsonData.is_object() && !jsonData.is_array())
			{
				return;
			}

			std::vector<std::string> erasa_keys;
			for (auto& item : jsonData.items())
			{
				if (item.value().is_null())
				{
					erasa_keys.push_back(item.key());
				}
				else
				{
					json_remove_null_values(item.value());
				}
			}

			for (auto& key : erasa_keys)
			{
				jsonData.erase(key);
			}
		}
		catch (...) {}
	}

	inline std::string json_to_jsonstring(nlohmann::json jsonData, bool removeNull)
	{
		std::string jsonString = "";
		try
		{
			//Remove null values from json data
			if (removeNull)
			{
				json_remove_null_values(jsonData);
			}

			//Convert json to string
			jsonString = jsonData.dump();
		}
		catch (...) {}
		return jsonString;
	}

	inline nlohmann::json jsonstring_to_json(std::string jsonString)
	{
		nlohmann::json jsonData{};
		try
		{
			//Parse json data
			jsonData = nlohmann::json::parse(jsonString);
		}
		catch (...) {}
		return jsonData;
	}

	template<typename T>
	inline nlohmann::json struct_to_json(T structVar)
	{
		nlohmann::json jsonData{};
		try
		{
			//Parse json data
			jsonData = structVar;
		}
		catch (...) {}
		return jsonData;
	}

	template<typename T>
	inline std::string struct_to_jsonstring(T structVar, bool removeNull)
	{
		std::string jsonString = "";
		try
		{
			//Parse json data
			nlohmann::json jsonData = struct_to_json(structVar);

			//Convert json to string
			jsonString = json_to_jsonstring(jsonData, removeNull);
		}
		catch (...) {}
		return jsonString;
	}

	template<typename T>
	inline T json_to_struct(nlohmann::json jsonData)
	{
		T returnStruct{};
		try
		{
			//Convert json to struct
			returnStruct = jsonData.get<T>();
		}
		catch (...) {}
		return returnStruct;
	}

	template<typename T>
	inline T jsonstring_to_struct(std::string jsonString)
	{
		T returnStruct{};
		try
		{
			//Parse json data
			nlohmann::json jsonData = jsonstring_to_json(jsonString);

			//Convert json to struct
			returnStruct = json_to_struct<T>(jsonData);
		}
		catch (...) {}
		return returnStruct;
	}
}