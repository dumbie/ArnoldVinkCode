#pragma once
#include <string>
#include "..\..\AVDebug.h"

namespace ArnoldVinkCode::GitHub
{
	inline std::string GetLatestVersion(std::string userName, std::string repoName)
	{
		try
		{
			//Set request headers
			std::vector<std::string> requestHeaders{};
			requestHeaders.push_back("Accept: application/json");
			requestHeaders.push_back("Authorization: token " + ApiTokens::GitHub);

			//Download releases from Github
			std::string releasesJson = DownloadString(GetPathLatestReleases(userName, repoName), repoName, requestHeaders);

			//Parse json data
			nlohmann::json parsedJson = nlohmann::json::parse(releasesJson);

			//Get available version
			return parsedJson["name"];
		}
		catch (...)
		{
			AVDebugWriteLine("Failed to get latest GitHub version.");
			return "";
		}
	}
}