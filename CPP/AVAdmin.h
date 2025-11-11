#pragma once
#include "AVFinally.h"

//Check if thread has admin access
static bool IsThreadAdmin()
{
	PSID pSid = NULL;
	AVFinallySafe(
		{
			FreeSid(pSid);
		});
	try
	{
		//Allocate and init sid
		SID_IDENTIFIER_AUTHORITY identifierAuthority = SECURITY_NT_AUTHORITY;
		AllocateAndInitializeSid(&identifierAuthority, 2, SECURITY_BUILTIN_DOMAIN_RID, DOMAIN_ALIAS_RID_ADMINS, 0, 0, 0, 0, 0, 0, &pSid);

		//Check token membership
		BOOL isMember;
		CheckTokenMembership(NULL, pSid, &isMember);

		//Return result
		return isMember;
	}
	catch (...)
	{
		//Return result
		return false;
	}
}

//Check if process has admin access
static bool IsProcessAdmin()
{
	HANDLE hToken = NULL;
	AVFinallySafe(
		{
			CloseHandle(hToken);
		});
	try
	{
		//Open process token
		OpenProcessToken(GetCurrentProcess(), TOKEN_QUERY, &hToken);

		//Get token information
		DWORD dwSize;
		TOKEN_ELEVATION tokenElevation;
		GetTokenInformation(hToken, TokenElevation, &tokenElevation, sizeof(tokenElevation), &dwSize);

		//Return result
		return (bool)tokenElevation.TokenIsElevated;
	}
	catch (...)
	{
		//Return result
		return false;
	}
}