#pragma once
#include <windows.h>

namespace ArnoldVinkCode::AVProcesses
{
	enum class AVProcessActions
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

	class AVProcessAction
	{
	public:
		AVProcessActions Action = AVProcessActions::NoAction;
		std::optional<AVProcess> AVProcess = std::nullopt;
	};
}