#pragma once
#include <windows.h>
#pragma comment(lib, "wininet.lib")
#include <wininet.h>
#include "AVFinally.h"
#include "AVString.h"
#include "AVDebug.h"

namespace ArnoldVinkCode
{
	class AVUri
	{
	public:
		std::string targetHost;
		std::string targetPath;
	};

	inline std::string DownloadString(AVUri avUri, std::string targetUserAgent, std::vector<std::string> targetHeaders)
	{
		try
		{
			//Internet Open
			auto handleOpen = AVFin(AVFinMethod::Custom, InternetOpenA(targetUserAgent.c_str(), INTERNET_OPEN_TYPE_PRECONFIG, NULL, NULL, NULL));
			handleOpen.SetReleaser([&](auto releaseObject) { InternetCloseHandle((HINTERNET)releaseObject); });
			if (handleOpen.Get() == nullptr)
			{
				AVDebugWriteLine("DownloadString open handle empty.");
				return "";
			}

			//Check target host
			DWORD internetFlags = 0x00000000;
			INTERNET_PORT internetPort = INTERNET_INVALID_PORT_NUMBER;
			if (avUri.targetHost.starts_with("http://"))
			{
				string_replace_all(avUri.targetHost, "http://", "");
				internetPort = INTERNET_DEFAULT_HTTP_PORT;
			}
			else
			{
				string_replace_all(avUri.targetHost, "https://", "");
				internetPort = INTERNET_DEFAULT_HTTPS_PORT;
				internetFlags |= INTERNET_FLAG_SECURE;
			}

			//Internet Connect
			auto handleConnect = AVFin(AVFinMethod::Custom, InternetConnectA(handleOpen.Get(), avUri.targetHost.c_str(), internetPort, NULL, NULL, INTERNET_SERVICE_HTTP, NULL, NULL));
			handleConnect.SetReleaser([&](auto releaseObject) { InternetCloseHandle((HINTERNET)releaseObject); });
			if (handleConnect.Get() == nullptr)
			{
				AVDebugWriteLine("DownloadString connect handle empty.");
				return "";
			}

			//Internet Request Open
			LPCSTR acceptTypes[] = { "*/*", NULL };
			auto handleRequest = AVFin(AVFinMethod::Custom, HttpOpenRequestA(handleConnect.Get(), "GET", avUri.targetPath.c_str(), NULL, NULL, acceptTypes, internetFlags, NULL));
			handleRequest.SetReleaser([&](auto releaseObject) { InternetCloseHandle((HINTERNET)releaseObject); });
			if (handleRequest.Get() == nullptr)
			{
				AVDebugWriteLine("DownloadString request handle empty.");
				return "";
			}

			//Internet Request Headers
			for (std::string targetHeader : targetHeaders)
			{
				HttpAddRequestHeadersA(handleRequest.Get(), targetHeader.c_str(), (DWORD)targetHeader.size(), NULL);
			}

			//Internet Request Send
			if (!HttpSendRequestA(handleRequest.Get(), NULL, NULL, NULL, NULL))
			{
				AVDebugWriteLine("DownloadString send request failed.");
				return "";
			}

			//Internet Check Data
			DWORD dataSize;
			if (!InternetQueryDataAvailable(handleRequest.Get(), &dataSize, NULL, NULL))
			{
				AVDebugWriteLine("DownloadString query data available failed.");
				return "";
			}
			if (dataSize <= 0)
			{
				AVDebugWriteLine("DownloadString data is empty.");
				return "";
			}

			//Internet Read Data
			std::string dataBufferRead;
			dataBufferRead.resize(dataSize);
			std::string dataBufferTotal;
			while (true)
			{
				//Read buffer
				if (!InternetReadFile(handleRequest.Get(), dataBufferRead.data(), (DWORD)dataBufferRead.size(), &dataSize)) { break; }
				if (dataSize <= 0) { break; }
				dataBufferRead.resize(dataSize);

				//Append buffer
				dataBufferTotal.append(dataBufferRead);
			}

			//Return result
			AVDebugWriteLine("DownloadString succeeded: " << avUri.targetHost.c_str() << " | " << avUri.targetPath.c_str() << " | " << dataBufferTotal.size() << "bytes");
			return dataBufferTotal;
		}
		catch (...)
		{
			//Return result
			AVDebugWriteLine("DownloadString exception.");
			return "";
		}
	}

	inline std::string SendPostRequest(AVUri avUri, std::string targetUserAgent, std::vector<std::string> targetHeaders, std::string targetData)
	{
		try
		{
			//Internet Open
			auto handleOpen = AVFin(AVFinMethod::Custom, InternetOpenA(targetUserAgent.c_str(), INTERNET_OPEN_TYPE_PRECONFIG, NULL, NULL, NULL));
			handleOpen.SetReleaser([&](auto releaseObject) { InternetCloseHandle((HINTERNET)releaseObject); });
			if (handleOpen.Get() == nullptr)
			{
				AVDebugWriteLine("SendPostRequest open handle empty.");
				return "";
			}

			//Check target host
			DWORD internetFlags = 0x00000000;
			INTERNET_PORT internetPort = INTERNET_INVALID_PORT_NUMBER;
			if (avUri.targetHost.starts_with("http://"))
			{
				string_replace_all(avUri.targetHost, "http://", "");
				internetPort = INTERNET_DEFAULT_HTTP_PORT;
			}
			else
			{
				string_replace_all(avUri.targetHost, "https://", "");
				internetPort = INTERNET_DEFAULT_HTTPS_PORT;
				internetFlags |= INTERNET_FLAG_SECURE;
			}

			//Internet Connect
			auto handleConnect = AVFin(AVFinMethod::Custom, InternetConnectA(handleOpen.Get(), avUri.targetHost.c_str(), internetPort, NULL, NULL, INTERNET_SERVICE_HTTP, NULL, NULL));
			handleConnect.SetReleaser([&](auto releaseObject) { InternetCloseHandle((HINTERNET)releaseObject); });
			if (handleConnect.Get() == nullptr)
			{
				AVDebugWriteLine("SendPostRequest connect handle empty.");
				return "";
			}

			//Internet Request Open
			LPCSTR acceptTypes[] = { "*/*", NULL };
			auto handleRequest = AVFin(AVFinMethod::Custom, HttpOpenRequestA(handleConnect.Get(), "POST", avUri.targetPath.c_str(), NULL, NULL, acceptTypes, internetFlags, NULL));
			handleRequest.SetReleaser([&](auto releaseObject) { InternetCloseHandle((HINTERNET)releaseObject); });
			if (handleRequest.Get() == nullptr)
			{
				AVDebugWriteLine("SendPostRequest request handle empty.");
				return "";
			}

			//Internet Request Headers
			for (std::string targetHeader : targetHeaders)
			{
				HttpAddRequestHeadersA(handleRequest.Get(), targetHeader.c_str(), (DWORD)targetHeader.size(), NULL);
			}

			//Internet Request Send
			if (!HttpSendRequestA(handleRequest.Get(), NULL, NULL, (LPVOID)targetData.c_str(), (DWORD)targetData.size()))
			{
				AVDebugWriteLine("SendPostRequest send request failed.");
				return "";
			}

			//Internet Check Data
			DWORD dataSize;
			if (!InternetQueryDataAvailable(handleRequest.Get(), &dataSize, NULL, NULL))
			{
				AVDebugWriteLine("SendPostRequest query data available failed.");
				return "";
			}
			if (dataSize <= 0)
			{
				AVDebugWriteLine("SendPostRequest data is empty.");
				return "";
			}

			//Internet Read Data
			std::string dataBufferRead;
			dataBufferRead.resize(dataSize);
			std::string dataBufferTotal;
			while (true)
			{
				//Read buffer
				if (!InternetReadFile(handleRequest.Get(), dataBufferRead.data(), (DWORD)dataBufferRead.size(), &dataSize)) { break; }
				if (dataSize <= 0) { break; }
				dataBufferRead.resize(dataSize);

				//Append buffer
				dataBufferTotal.append(dataBufferRead);
			}

			//Return result
			AVDebugWriteLine("SendPostRequest succeeded: " << avUri.targetHost.c_str() << " | " << avUri.targetPath.c_str() << " | " << dataBufferTotal.size() << "bytes");
			return dataBufferTotal;
		}
		catch (...)
		{
			//Return result
			AVDebugWriteLine("SendPostRequest exception.");
			return "";
		}
	}
}