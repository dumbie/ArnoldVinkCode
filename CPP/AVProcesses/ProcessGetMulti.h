#pragma once
#include <windows.h>
#include <string>
#include <vector>
#include "AVString.h"
#include "AVFinally.h"

namespace ArnoldVinkCode::AVProcesses
{
	//Get all running processes multi
	inline std::vector<ProcessMulti> Get_ProcessesMultiAll()
	{
		PSYSTEM_PROCESS_INFORMATION spiQueryBuffer;
		std::vector<ProcessMulti> listProcessMulti;
		AVFinallySafe(
			{
				free(spiQueryBuffer);
			});
		try
		{
			//Query process information
			spiQueryBuffer = Query_SystemProcessInformation();

			//Loop process information
			PSYSTEM_PROCESS_INFORMATION spiQueryBufferLoop = spiQueryBuffer;
			while (true)
			{
				try
				{
					//Get executable name
					std::wstring exeNameW = std::wstring(spiQueryBufferLoop->ImageName.Buffer, spiQueryBufferLoop->ImageName.Length / sizeof(WCHAR));
					std::string exeNameA = wstring_to_string(exeNameW);

					//Add multi process to list
					ProcessMulti processMulti = ProcessMulti((int)spiQueryBufferLoop->UniqueProcessId, (int)spiQueryBufferLoop->Reserved2, exeNameA);
					listProcessMulti.push_back(processMulti);

					//Move to next process
					if (spiQueryBufferLoop->NextEntryOffset != 0)
					{
						spiQueryBufferLoop = (PSYSTEM_PROCESS_INFORMATION)((BYTE*)spiQueryBufferLoop + spiQueryBufferLoop->NextEntryOffset);
					}
					else
					{
						break;
					}
				}
				catch (...) {}
			}
		}
		catch (...) {}
		return listProcessMulti;
	}

	//Get multi process by executable name
	inline std::vector<ProcessMulti> Get_ProcessesMultiByName(std::string executableName)
	{
		std::vector<ProcessMulti> listProcessMulti;
		try
		{
			//List all processes
			std::vector<ProcessMulti> processList = Get_ProcessesMultiAll();

			//Lowercase executable name
			std::string exeNameLowerSearch = string_to_lower(executableName);

			//Look for executable name
			for (ProcessMulti process : processList)
			{
				//Lowercase executable name
				std::string exeNameLowerProcess = string_to_lower(process.ExeName());

				//Add multi process to list
				if (exeNameLowerProcess == exeNameLowerSearch)
				{
					listProcessMulti.push_back(process);
				}
			}
		}
		catch (...) {}
		return listProcessMulti;
	}
}