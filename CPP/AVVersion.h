#pragma once

static std::string GetVersionFromResource(HINSTANCE hInstance)
{
	//Set default version string
	std::string stringVersion = "V?.?.?.?";

	//Check empty hinstance
	if (hInstance == nullptr)
	{
		hInstance = GetModuleHandleW(nullptr);
	}

	//Find file version resource
	HRSRC hResourceFind = FindResourceW(hInstance, MAKEINTRESOURCE(VS_VERSION_INFO), RT_VERSION);
	if (hResourceFind)
	{
		HGLOBAL hResourceInfo = LoadResource(hInstance, hResourceFind);
		if (hResourceInfo)
		{
			LPVOID hResourceLock = LockResource(hResourceInfo);
			if (hResourceLock)
			{
				UINT resourceLength = 0;
				VS_FIXEDFILEINFO* resourceBuffer = nullptr;
				if (VerQueryValueW(hResourceLock, L"\\", (LPVOID*)&resourceBuffer, &resourceLength))
				{
					if (resourceBuffer)
					{
						DWORD dwFileVersionMS = resourceBuffer->dwFileVersionMS;
						DWORD dwFileVersionLS = resourceBuffer->dwFileVersionLS;

						DWORD dwMajor = HIWORD(dwFileVersionMS);
						DWORD dwMinor = LOWORD(dwFileVersionMS);
						DWORD dwPatch = HIWORD(dwFileVersionLS);
						DWORD dwRevision = LOWORD(dwFileVersionLS);

						//Format version string
						stringVersion = "V" + number_to_string(dwMajor) + "." + number_to_string(dwMinor) + "." + number_to_string(dwPatch) + "." + number_to_string(dwRevision);
					}
				}
			}
		}
	}

	//Return version string
	return stringVersion;
}