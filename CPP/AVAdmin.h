#pragma once
#include <windows.h>
#include "AVFinally.h"

namespace ArnoldVinkCode
{
	//Check if thread has admin access
	inline bool IsThreadAdmin()
	{
		try
		{
			//Allocate and init sid
			auto pSid = AVFin<PSID>(AVFinMethod::FreeSid);
			SID_IDENTIFIER_AUTHORITY identifierAuthority = SECURITY_NT_AUTHORITY;
			AllocateAndInitializeSid(&identifierAuthority, 2, SECURITY_BUILTIN_DOMAIN_RID, DOMAIN_ALIAS_RID_ADMINS, 0, 0, 0, 0, 0, 0, &pSid.Get());

			//Check token membership
			BOOL isMember;
			CheckTokenMembership(NULL, pSid.Get(), &isMember);

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
	inline bool IsProcessAdmin()
	{
		try
		{
			//Open process token
			auto hToken = AVFin<HANDLE>(AVFinMethod::CloseHandle);
			OpenProcessToken(GetCurrentProcess(), TOKEN_QUERY, &hToken.Get());

			//Get token information
			DWORD dwSize;
			TOKEN_ELEVATION tokenElevation;
			GetTokenInformation(hToken.Get(), TokenElevation, &tokenElevation, sizeof(tokenElevation), &dwSize);

			//Return result
			return (bool)tokenElevation.TokenIsElevated;
		}
		catch (...)
		{
			//Return result
			return false;
		}
	}
}