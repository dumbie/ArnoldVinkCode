#pragma once
#include <windows.h>
#include <string>
#include <vector>
#include "..\AVString.h"
#include "..\AVFinally.h"

namespace ArnoldVinkCode::AVProcesses
{
	//Get process thread information by process id
	inline std::vector<ProcessThreadInfo> Detail_ProcessThreadsByProcessId(int targetProcessId, bool firstThreadOnly)
	{
		std::vector<ProcessThreadInfo> listProcessThread{};
		try
		{
			//AVDebugWriteLine("Getting process threads for process id: " << targetProcessId << "/" << firstThreadOnly);

			//Query process information
			auto spiQueryBuffer = AVFin(AVFinMethod::FreeMarshal, Query_SystemProcessInformation());
			if (spiQueryBuffer.Get() == nullptr)
			{
				AVDebugWriteLine("Failed getting all process threads: query failed.");
				return listProcessThread;
			}

			//Loop process information
			ULONG systemProcessOffset = 0;
			while (true)
			{
				try
				{
					//Get process information
					PSYSTEM_PROCESS_INFORMATION systemProcess = (PSYSTEM_PROCESS_INFORMATION)((BYTE*)spiQueryBuffer.Get() + systemProcessOffset);

					//Check target process id
					if (targetProcessId == (int)systemProcess->UniqueProcessId)
					{
						//AVDebugWriteLine("Found thread process: " << (int)systemProcess->UniqueProcessId << " / " << (int)systemProcess->NumberOfThreads);

						//Loop threads
						PSYSTEM_THREAD_INFORMATION systemThreads = (PSYSTEM_THREAD_INFORMATION)&systemProcess->Reserved7[6];
						for (int i = 0; i < systemProcess->NumberOfThreads; i++)
						{
							try
							{
								//Get thread information
								SYSTEM_THREAD_INFORMATION systemThread = systemThreads[i];

								//Add process thread to list
								ProcessThreadInfo processThread{};
								processThread.Identifier = (int)systemThread.ClientId.UniqueThread;
								processThread.ThreadState = (ProcessThreadState)systemThread.ThreadState;
								processThread.ThreadWaitReason = (ProcessThreadWaitReason)systemThread.WaitReason;
								listProcessThread.push_back(processThread);
								//AVDebugWriteLine("Found thread: " << (int)processThread.Identifier << "/" << (int)processThread.ThreadState << "/" << (int)processThread.ThreadWaitReason);

								//Return first process thread
								if (firstThreadOnly)
								{
									return listProcessThread;
								}
							}
							catch (...) {}
						}
					}

					//Move to next process
					if (systemProcess->NextEntryOffset != 0)
					{
						systemProcessOffset += systemProcess->NextEntryOffset;
					}
					else
					{
						break;
					}
				}
				catch (...) {}
			}
		}
		catch (...)
		{
			AVDebugWriteLine("Failed getting all process threads.");
		}
		return listProcessThread;
	}
}