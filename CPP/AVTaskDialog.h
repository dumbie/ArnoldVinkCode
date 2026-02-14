#pragma once
#include <windows.h>
#pragma comment(lib, "comctl32.lib")
#pragma comment(linker, "/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='*' publicKeyToken='6595b64144ccf1df' language='*'\"")

namespace ArnoldVinkCode
{
	//Enumerators
	enum class TaskDialogIcon : int
	{
		None = 0,
		File = 2,
		Folder = 3,
		Application = 15,
		ControlPanel = 27,
		RecycleBinFull = 54,
		RecycleBinEmpty = 55,
		ShieldAdministrator = 78,
		Information = 81,
		Warning = 84,
		Cross = 89,
		Error = 98,
		QuestionMark = 99,
		Run = 100,
		ShieldQuestionMark = 104,
		ShieldError = 105,
		ShieldSuccess = 106,
		ShieldWarning = 107,
		Search = 177
	};

	enum class TaskDialogIconBar : int
	{
		None = 0,
		Information = 65533,
		Warning = 65535,
		Error = 65534,
		Shield = 65532,
		ShieldBlueBar = 65531,
		ShieldGrayBar = 65527,
		ShieldWarningYellowBar = 65530,
		ShieldErrorRedBar = 65529,
		ShieldSuccessGreenBar = 65528
	};

	//Structures
	struct TaskDialogCallbackStruct
	{
		TaskDialogIcon TargetIcon = TaskDialogIcon::None;
	};

	//Callback
	inline HRESULT CALLBACK TaskDialogCallbackProc(HWND hWnd, UINT uNotification, WPARAM wParam, LPARAM lParam, LONG_PTR dwRefData)
	{
		try
		{
			//Convert callback reference data
			TaskDialogCallbackStruct* refData = (TaskDialogCallbackStruct*)dwRefData;

			//Set task dialog icon
			SendMessageW(hWnd, TDM_UPDATE_ICON, NULL, (int)refData->TargetIcon);

			//Reset window icon
			SendMessageW(hWnd, WM_SETICON, ICON_SMALL, NULL);
			SendMessageW(hWnd, WM_SETICON, ICON_SMALL2, NULL);
			SendMessageW(hWnd, WM_SETICON, ICON_BIG, NULL);
		}
		catch (...) {}
		return S_OK;
	}

	//Functions
	inline int AVTaskDialog(HWND parentWindow, std::wstring Title, std::wstring Question, std::wstring Description, std::vector<std::wstring> Answers, bool cancelButton)
	{
		try
		{
			//Check parent window
			if (parentWindow == NULL)
			{
				parentWindow = GetActiveWindow();
			}

			//Create callback reference data
			TaskDialogCallbackStruct taskDialogCallbackStruct{};
			taskDialogCallbackStruct.TargetIcon = TaskDialogIcon::None;

			//Set task dialog config
			TASKDIALOGCONFIG taskDialogConfig{};
			taskDialogConfig.cbSize = sizeof(TASKDIALOGCONFIG);
			taskDialogConfig.pfCallback = TaskDialogCallbackProc;
			taskDialogConfig.lpCallbackData = (LONG_PTR)&taskDialogCallbackStruct;
			taskDialogConfig.hwndParent = parentWindow;
			taskDialogConfig.pszWindowTitle = Title.c_str();
			taskDialogConfig.pszMainInstruction = Question.c_str();
			taskDialogConfig.pszContent = Description.c_str();
			taskDialogConfig.pszMainIcon = MAKEINTRESOURCE(TaskDialogIconBar::ShieldBlueBar);
			taskDialogConfig.dwFlags = TDF_POSITION_RELATIVE_TO_WINDOW;

			//Add answer buttons
			std::vector<TASKDIALOG_BUTTON> taskDialogButtons{};
			if (Answers.size() > 0)
			{
				int buttonId = 1000;
				for (std::wstring answer : Answers)
				{
					taskDialogButtons.push_back({ buttonId, _wcsdup(answer.c_str()) });
					buttonId++;
				}
				if (cancelButton)
				{
					taskDialogButtons.push_back({ 999, L"Cancel" });
				}
				taskDialogConfig.dwFlags |= TDF_USE_COMMAND_LINKS;
			}
			else
			{
				taskDialogButtons.push_back({ 999, L"Close" });
			}
			taskDialogConfig.cButtons = taskDialogButtons.size();
			taskDialogConfig.pButtons = taskDialogButtons.data();

			//Show task dialog
			int pnButton = -1;
			TaskDialogIndirect(&taskDialogConfig, &pnButton, NULL, NULL);
			pnButton -= 1000;

			//Return result
			AVDebugWriteLine("Selected task dialog index: " << pnButton);
			return pnButton;
		}
		catch (...)
		{
			//Return result
			AVDebugWriteLine("Task dialog failed.");
			return -1;
		}
	}

	inline std::wstring AVTaskDialogStr(HWND parentWindow, std::wstring Title, std::wstring Question, std::wstring Description, std::vector<std::wstring> Answers, bool cancelButton)
	{
		try
		{
			//Get selected index
			int selectedIndex = AVTaskDialog(parentWindow, Title, Question, Description, Answers, cancelButton);

			//Check selected button
			std::wstring selectedButton = L"";
			if (selectedIndex >= 0)
			{
				selectedButton = Answers[selectedIndex];
			}

			//Return result
			AVDebugWriteLine("Selected task dialog answer: " << selectedButton);
			return selectedButton;
		}
		catch (...)
		{
			//Return result
			AVDebugWriteLine("Task dialog failed.");
			return L"";
		}
	}
}