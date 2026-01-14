#pragma once
#include <vector>
#include <string>
#include "..\..\AVDebug.h"

namespace ArnoldVinkCode::GitHub
{
	inline AVUri GetPathLatestReleases(std::string userName, std::string repoName)
	{
		AVUri avUri;
		try
		{
			avUri.targetHost = "https://api.github.com";
			avUri.targetPath = "repos/" + userName + "/" + repoName + "/releases/latest";
		}
		catch (...)
		{
			AVDebugWriteLine("Failed to get GitHub latest releases path.");
		}
		return avUri;
	}

	inline AVUri GetPathLatestDownload(std::string userName, std::string repoName, std::string fileName)
	{
		AVUri avUri;
		try
		{
			avUri.targetHost = "https://api.github.com";
			avUri.targetPath = userName + "/" + repoName + "/releases/latest/download/" + fileName;
		}
		catch (...)
		{
			AVDebugWriteLine("Failed to get GitHub latest download path.");
		}
		return avUri;
	}
}