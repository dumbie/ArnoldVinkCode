using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVTaskDialog
    {
        //Functions
        public static int Popup(IntPtr parentWindow, string Title, string Question, string Description, List<string> Answers)
        {
            try
            {
                //Check parent window
                if (parentWindow == IntPtr.Zero)
                {
                    parentWindow = GetActiveWindow();
                }
                NativeWindow parentWindow32 = new NativeWindow();
                parentWindow32.AssignHandle(parentWindow);

                //Set dialog page
                TaskDialogPage taskDialogConfig = new TaskDialogPage();
                taskDialogConfig.Caption = Title;
                taskDialogConfig.Heading = Question;
                taskDialogConfig.Text = Description;

                //Set bar background
                taskDialogConfig.Icon = System.Windows.Forms.TaskDialogIcon.ShieldBlueBar;

                //Set target icon
                taskDialogConfig.Created += (sender, args) => TaskDialogPage_Created(sender, TaskDialogIcon.None);

                //Set answers
                TaskDialogButtonCollection taskDialogButtons = new TaskDialogButtonCollection();
                if (Answers != null && Answers.Count > 0)
                {
                    int buttonId = 1000;
                    foreach (string answer in Answers)
                    {
                        TaskDialogCommandLinkButton taskButton = new TaskDialogCommandLinkButton(answer);
                        taskButton.Tag = buttonId;
                        taskDialogButtons.Add(taskButton);
                        buttonId++;
                    }
                }
                else
                {
                    TaskDialogButton taskButton = new TaskDialogButton(" Close ");
                    taskButton.Tag = 999;
                    taskDialogButtons.Add(taskButton);
                }
                taskDialogConfig.Buttons = taskDialogButtons;     

                //Show task dialog
                Application.EnableVisualStyles();
                TaskDialogButton result = TaskDialog.ShowDialog(parentWindow32, taskDialogConfig, TaskDialogStartupLocation.CenterOwner);
                int pnButton = (int)result.Tag - 1000;

                //Return result
                Debug.WriteLine("Selected task dialog index: " + pnButton);
                return pnButton;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Task dialog failed: " + ex.Message);
                return -1;
            }
        }

        public static string PopupStr(IntPtr parentWindow, string Title, string Question, string Description, List<string> Answers)
        {
            try
            {
                //Get selected index
                int selectedIndex = Popup(parentWindow, Title, Question, Description, Answers);

                //Check selected button
                string selectedButton = string.Empty;
                if (selectedIndex >= 0)
                {
                    selectedButton = Answers[selectedIndex];
                }

                //Return result
                Debug.WriteLine("Selected task dialog answer: " + selectedButton);
                return selectedButton;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Task dialog failed: " + ex.Message);
                return string.Empty;
            }
        }

        //Events
        private static void TaskDialogPage_Created(object sender, TaskDialogIcon targetIcon)
        {
            try
            {
                TaskDialogPage dialogPage = (TaskDialogPage)sender;
                TaskDialog boundDialog = dialogPage.BoundDialog;
                IntPtr windowHandle = boundDialog.Handle;

                //Set task dialog icon
                WindowMessages TDM_UPDATE_ICON = WindowMessages.WM_USER + 116;
                SendMessage(windowHandle, TDM_UPDATE_ICON, IntPtr.Zero, (IntPtr)targetIcon);

                //Reset window icon
                SendMessage(windowHandle, WindowMessages.WM_SETICON, (int)GetSetIconFlags.ICON_SMALL, IntPtr.Zero);
                SendMessage(windowHandle, WindowMessages.WM_SETICON, (int)GetSetIconFlags.ICON_SMALL2, IntPtr.Zero);
                SendMessage(windowHandle, WindowMessages.WM_SETICON, (int)GetSetIconFlags.ICON_BIG, IntPtr.Zero);
            }
            catch { }
        }

        //Enumerators
        public enum TaskDialogIcon : int
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
        }
    }
}