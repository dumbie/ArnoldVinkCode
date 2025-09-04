#pragma once
#include <wininet.h>
#include "AVFinally.h"

static std::string DownloadString(std::string targetHost, std::string targetPath, std::string targetUserAgent, std::string targetHeader)
{
	try
	{

	}
	catch (...) {}
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

		//Internet Connect
		//Fix if url starts with https INTERNET_DEFAULT_HTTPS_PORT + INTERNET_FLAG_SECURE else INTERNET_DEFAULT_HTTP_PORT + NULL
		handleConnect = InternetConnectA(handleOpen, targetHost.c_str(), INTERNET_DEFAULT_HTTPS_PORT, NULL, NULL, INTERNET_SERVICE_HTTP, NULL, NULL);
		if (handleConnect == NULL)
		{
			AVDebugWriteLine("SendPostRequest connect handle empty.");
			return "";
		}

		//Internet Request Open
		PCSTR acceptTypes[] = { "*/*" };
		handleRequest = HttpOpenRequestA(handleConnect, "POST", targetPath.c_str(), NULL, NULL, acceptTypes, INTERNET_FLAG_SECURE, NULL);
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
			if (!InternetReadFile(handleRequest, &dataBufferRead[0], dataBufferRead.size(), &dataSize)) { break; }
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