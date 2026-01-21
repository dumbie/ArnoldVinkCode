#pragma once
#include <windows.h>
#include <atltime.h>
#include <iostream>
#include <string>
#include <ctime>

namespace ArnoldVinkCode
{
	//TIME_T is seconds since epoch (January 1, 1970)
	//FILETIME is 100-nanoseconds since epoch (January 1, 1601)

	inline std::tm tm_empty()
	{
		std::tm tm{};
		tm.tm_year = -1900;
		tm.tm_mday = 1;
		return tm;
	}

	inline std::time_t timet_empty()
	{
		return std::time_t(0);
	}

	inline bool tm_is_empty(std::tm tm)
	{
		try
		{
			if (tm.tm_year <= 0) { return true; }
		}
		catch (...) {}
		return false;
	}

	inline bool timet_is_empty(std::time_t timet)
	{
		try
		{
			if (timet <= 0) { return true; }
		}
		catch (...) {}
		return false;
	}

	inline std::tm time_current()
	{
		try
		{
			std::tm current_tm;
			const std::time_t current_t = std::time(NULL);
			localtime_s(&current_tm, &current_t);
			return current_tm;
		}
		catch (...) {}
		return tm_empty();
	}

	inline std::time_t tm_to_timet(std::tm tm)
	{
		try
		{
			return mktime(&tm);
		}
		catch (...) {}
		return timet_empty();
	}

	inline std::tm timet_to_tm(std::time_t timeT)
	{
		try
		{
			std::tm tm;
			localtime_s(&tm, &timeT);
			return tm;
		}
		catch (...) {}
		return tm_empty();
	}

	inline std::time_t filetime_to_timet(FILETIME fileTime)
	{
		try
		{
			ULARGE_INTEGER fileInt;
			fileInt.LowPart = fileTime.dwLowDateTime;
			fileInt.HighPart = fileTime.dwHighDateTime;
			return fileInt.QuadPart / 10000000ULL - 11644473600ULL;
		}
		catch (...) {}
		return timet_empty();
	}

	inline std::tm filetime_to_tm(FILETIME fileTime)
	{
		try
		{
			std::time_t time_t = filetime_to_timet(fileTime);
			return timet_to_tm(time_t);
		}
		catch (...) {}
		return tm_empty();
	}

	inline std::time_t systemtime_to_timet(SYSTEMTIME systemTime)
	{
		try
		{
			//Convert SYSTEMTIME to FILETIME
			FILETIME fileTime;
			SystemTimeToFileTime(&systemTime, &fileTime);

			//Convert FILETIME to TIMET
			return filetime_to_timet(fileTime);
		}
		catch (...) {}
		return timet_empty();
	}

	inline std::tm systemtime_to_tm(SYSTEMTIME systemTime)
	{
		try
		{
			//Convert SYSTEMTIME to FILETIME
			FILETIME fileTime;
			SystemTimeToFileTime(&systemTime, &fileTime);

			//Convert FILETIME to TM
			return filetime_to_tm(fileTime);
		}
		catch (...) {}
		return tm_empty();
	}

	/// <summary>
	/// Example: tm_to_string(tm, "%d.%m.%Y %H:%M:%S")
	/// </summary>
	inline std::string tm_to_string(std::tm tm, std::string timeFormat)
	{
		try
		{
			if (!tm_is_empty(tm))
			{
				char buffer[256];
				std::strftime(buffer, sizeof(buffer), timeFormat.c_str(), &tm);
				return std::string(buffer);
			}
		}
		catch (...) {}
		return "";
	}

	/// <summary>
	/// Example: string_to_tm("30.01.2000 10:20:40", "%d.%m.%Y %H:%M:%S")
	/// </summary>
	inline std::tm string_to_tm(std::string timeString, std::string timeFormat)
	{
		try
		{
			std::tm tm;
			std::istringstream iss(timeString);
			iss >> std::get_time(&tm, timeFormat.c_str());
			tm.tm_year += 1900;
			tm.tm_mon += 1;
			return tm;
		}
		catch (...) {}
		return tm_empty();
	}

	inline std::tm time_add(std::tm targetTime, std::tm valueTime)
	{
		try
		{
		}
		catch (...) {}
		return tm_empty();
	}

	inline std::tm time_subtract(std::tm targetTime, std::tm valueTime)
	{
		try
		{
		}
		catch (...) {}
		return tm_empty();
	}

	inline CTimeSpan time_difference(std::tm timeLeft, std::tm timeRight)
	{
		try
		{
			//Check times
			if (tm_is_empty(timeLeft) || tm_is_empty(timeRight))
			{
				return CTimeSpan(-1);
			}

			//Get times
			std::time_t left_t = tm_to_timet(timeLeft);
			std::time_t right_t = tm_to_timet(timeRight);

			//Calculate difference
			std::time_t difference_t;
			if (left_t > right_t)
			{
				difference_t = left_t - right_t;
			}
			else
			{
				difference_t = right_t - left_t;
			}
			return CTimeSpan(difference_t);
		}
		catch (...) {}
		return CTimeSpan(-1);
	}
}