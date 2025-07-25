using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVTaskDialog
    {
        //Functions
        public static string Popup(dynamic disableElement, string Title, string Question, string Description, List<string> Answers)
        {
            try
            {
                //Disable source framework element
                if (disableElement != null)
                {
                    disableElement.IsEnabled = false;
                }

                //Set dialog page
                TaskDialogPage page = new TaskDialogPage();
                page.Caption = Title;
                page.Heading = Question;
                page.Text = Description;

                //Set answers
                TaskDialogButtonCollection buttonCollection = new TaskDialogButtonCollection();
                if (Answers != null)
                {
                    if (Answers.Count > 1)
                    {
                        page.AllowCancel = false;
                        foreach (string answer in Answers)
                        {
                            buttonCollection.Add(new TaskDialogCommandLinkButton(answer, string.Empty));
                        }
                    }
                    else if (Answers.Count == 1)
                    {
                        page.AllowCancel = true;
                        buttonCollection.Add(new TaskDialogButton(Answers.FirstOrDefault()));
                    }
                    else
                    {
                        page.AllowCancel = true;
                        buttonCollection.Add(new TaskDialogButton("Close"));
                    }
                }
                else
                {
                    page.AllowCancel = true;
                    buttonCollection.Add(new TaskDialogButton("Close"));
                }
                page.Buttons = buttonCollection;

                //Set bar background
                page.Icon = TaskDialogIcon.ShieldBlueBar;

                //Set target icon
                page.Created += (sender, args) => TaskDialogPage_Created(sender, DialogIcon.None);

                //Show task dialog
                Application.EnableVisualStyles();
                TaskDialogButton result = TaskDialog.ShowDialog(page);

                //Enable source framework element
                if (disableElement != null)
                {
                    disableElement.IsEnabled = true;
                }

                //Return result
                Debug.WriteLine("Selected task dialog answer: " + result.Text);
                return result.Text;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Task dialog failed: " + ex.Message);
                return string.Empty;
            }
        }

        //Events
        private static void TaskDialogPage_Created(object sender, DialogIcon targetIcon)
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
                SendMessage(windowHandle, WindowMessages.WM_SETICON, (int)GetSetIconFlags.ICON_BIG, IntPtr.Zero);
            }
            catch { }
        }

        //Enumerators
        public enum DialogIcon : int
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