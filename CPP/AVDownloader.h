#pragma once
#pragma comment(lib, "wininet.lib")
#include <wininet.h>
#include "AVFinally.h"
#include "AVString.h"

static std::string DownloadString(std::string targetHost, std::string targetPath, std::string targetUserAgent, std::string targetHeader)
{
	HINTERNET handleOpen = NULL;
	HINTERNET handleConnect = NULL;
	HINTERNET handleRequest = NULL;
	AVFinallySafe(
		{
			InternetCloseHandle(handleOpen);
			InternetCloseHandle(handleConnect);
			InternetCloseHandle(handleRequest);
		});
	try
	{
		//Internet Open
		handleOpen = InternetOpenA(targetUserAgent.c_str(), INTERNET_OPEN_TYPE_PRECONFIG, NULL, NULL, NULL);
		if (handleOpen == NULL)
		{
			AVDebugWriteLine("DownloadString open handle empty.");
			return "";
		}

		//Check target host
		DWORD internetFlags = 0x00000000;
		INTERNET_PORT internetPort = INTERNET_INVALID_PORT_NUMBER;
		if (targetHost.starts_with("http://"))
		{
			string_replace_all(targetHost, "http://", "");
			internetPort = INTERNET_DEFAULT_HTTP_PORT;
		}
		else
		{
			string_replace_all(targetHost, "https://", "");
			internetPort = INTERNET_DEFAULT_HTTPS_PORT;
			internetFlags |= INTERNET_FLAG_SECURE;
		}

		//Internet Connect
		handleConnect = InternetConnectA(handleOpen, targetHost.c_str(), internetPort, NULL, NULL, INTERNET_SERVICE_HTTP, NULL, NULL);
		if (handleConnect == NULL)
		{
			AVDebugWriteLine("DownloadString connect handle empty.");
			return "";
		}

		//Internet Request Open
		PCSTR acceptTypes[] = { "*/*", NULL };
		handleRequest = HttpOpenRequestA(handleConnect, "GET", targetPath.c_str(), NULL, NULL, acceptTypes, internetFlags, NULL);
		if (handleRequest == NULL)
		{
			AVDebugWriteLine("DownloadString request handle empty.");
			return "";
		}

		//Internet Request Send
		if (!HttpSendRequestA(handleRequest, targetHeader.c_str(), targetHeader.size(), NULL, NULL))
		{
			AVDebugWriteLine("DownloadString send request failed.");
			return "";
		}

		//Internet Check Data
		DWORD dataSize;
		if (!InternetQueryDataAvailable(handleRequest, &dataSize, NULL, NULL))
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
			if (!InternetReadFile(handleRequest, dataBufferRead.data(), dataBufferRead.size(), &dataSize)) { break; }
			if (dataSize <= 0) { break; }
			dataBufferRead.resize(dataSize);

			//Append buffer
			dataBufferTotal.append(dataBufferRead);
		}

		//Return result
		AVDebugWriteLine("DownloadString succeeded: " << targetHost.c_str() << " | " << targetPath.c_str() << " | " << dataBufferTotal.size() << "bytes");
		return dataBufferTotal;
	}
	catch (...)
	{
		//Return result
		AVDebugWriteLine("DownloadString exception.");
		return "";
	}
}

static std::string SendPostRequest(std::string targetHost, std::string targetPath, std::string targetUserAgent, std::string targetHeader, std::string targetData)
{
	HINTERNET handleOpen = NULL;
	HINTERNET handleConnect = NULL;
	HINTERNET handleRequest = NULL;
	AVFinallySafe(
		{
			InternetCloseHandle(handleOpen);
			InternetCloseHandle(handleConnect);
			InternetCloseHandle(handleRequest);
		});
	try
	{
		//Internet Open
		handleOpen = InternetOpenA(targetUserAgent.c_str(), INTERNET_OPEN_TYPE_PRECONFIG, NULL, NULL, NULL);
		if (handleOpen == NULL)
		{
			AVDebugWriteLine("SendPostRequest open handle empty.");
			return "";
		}

		//Check target host
		DWORD internetFlags = 0x00000000;
		DWORD internetPort = INTERNET_INVALID_PORT_NUMBER;
		if (targetHost.starts_with("http://"))
		{
			string_replace_all(targetHost, "http://", "");
			internetPort = INTERNET_DEFAULT_HTTP_PORT;
		}
		else
		{
			string_replace_all(targetHost, "https://", "");
			internetPort = INTERNET_DEFAULT_HTTPS_PORT;
			internetFlags |= INTERNET_FLAG_SECURE;
		}

		//Internet Connect
		handleConnect = InternetConnectA(handleOpen, targetHost.c_str(), internetPort, NULL, NULL, INTERNET_SERVICE_HTTP, NULL, NULL);
		if (handleConnect == NULL)
		{
			AVDebugWriteLine("SendPostRequest connect handle empty.");
			return "";
		}

		//Internet Request Open
		PCSTR acceptTypes[] = { "*/*" };
		handleRequest = HttpOpenRequestA(handleConnect, "POST", targetPath.c_str(), NULL, NULL, acceptTypes, internetFlags, NULL);
		if (handleRequest == NULL)
		{
			AVDebugWriteLine("SendPostRequest request handle empty.");
			return "";
		}

		//Internet Request Send
		if (!HttpSendRequestA(handleRequest, targetHeader.c_str(), targetHeader.size(), (LPVOID)targetData.c_str(), targetData.size()))
		{
			AVDebugWriteLine("SendPostRequest send request failed.");
			return "";
		}

		//Internet Check Data
		DWORD dataSize;
		if (!InternetQueryDataAvailable(handleRequest, &dataSize, NULL, NULL))
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
			if (!InternetReadFile(handleRequest, dataBufferRead.data(), dataBufferRead.size(), &dataSize)) { break; }
			if (dataSize <= 0) { break; }
			dataBufferRead.resize(dataSize);

			//Append buffer
			dataBufferTotal.append(dataBufferRead);
		}

		//Return result
		AVDebugWriteLine("SendPostRequest succeeded: " << targetHost.c_str() << " | " << targetPath.c_str() << " | " << dataBufferTotal.size() << "bytes");
		return dataBufferTotal;
	}
	catch (...)
	{
		//Return result
		AVDebugWriteLine("SendPostRequest exception.");
		return "";
	}
}