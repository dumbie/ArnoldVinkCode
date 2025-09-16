#pragma once
#include <string>
#include <exception>
#include "AVString.h"

static std::string AVExceptionString()
{
	std::exception_ptr exception = std::current_exception();
	try
	{
		std::rethrow_exception(exception);
	}
	catch (const std::exception ex)
	{
		//Get exception name
		std::string exName = typeid(ex).name();
		string_replace(exName, "class std::", "");

		//Get exception message
		std::string exMessage = ex.what();

		//Return exception
		return "Exception: " + exName + " / " + exMessage;
	}
	catch (const std::string ex)
	{
		return "Exception: " + ex;
	}
	catch (const std::wstring ex)
	{
		return "Exception: " + wstring_to_string(ex);
	}
	catch (const char* ex)
	{
		return "Exception: " + char_to_string(ex);
	}
	catch (const wchar_t* ex)
	{
		return "Exception: " + wchar_to_string(ex);
	}
	catch (const int ex)
	{
		return "Exception: " + number_to_string(ex);
	}
	catch (...)
	{
		return "Exception: Unknown";
	}
}