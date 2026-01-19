#pragma once
#include <windows.h>

namespace ArnoldVinkCode::AVProcesses
{
	enum ProcessMultiActions
	{
		Launch,
		Close,
		CloseAll,
		Restart,
		RestartAll,
		Select,
		NoAction,
		Cancel
	};

	class ProcessMultiAction
	{
	public:
		ProcessMultiActions Action = ProcessMultiActions::NoAction;
		std::optional<ProcessMulti> ProcessMulti;
	};
}