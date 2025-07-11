using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    //Note: requires assemblyIdentity Microsoft.Windows.Common-Controls in app manifest
    public partial class AVTaskDialog
    {
        //Functions
        public static string Popup(string Title, string Question, string Description, List<string> Answers, IntPtr hIcon)
        {
            try
            {
                //Set configuration
                TASKDIALOGCONFIG config = new TASKDIALOGCONFIG();
                config.cbSize = (uint)Marshal.SizeOf(config);
                config.dwFlags = TASKDIALOG_FLAGS.TDF_NONE;
                config.pszWindowTitle = Title;
                config.pszMainInstruction = Question;
                config.pszContent = Description;

                //Set main icon
                if (hIcon != IntPtr.Zero)
                {
                    config.dwFlags |= TASKDIALOG_FLAGS.TDF_USE_HICON_MAIN;
                    config.hMainIcon = hIcon;
                }
                else
                {
                    config.hMainIcon = (IntPtr)TASKDIALOG_ICONS.TD_DEFAULT_ICON;
                }

                //Set answers
                if (Answers != null && Answers.Any())
                {
                    config.dwFlags |= TASKDIALOG_FLAGS.TDF_USE_COMMAND_LINKS;
                    config.dwCommonButtons = TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_NONE;

                    int answerIndex = 100;
                    List<TASKDIALOG_BUTTON> answerButtons = new List<TASKDIALOG_BUTTON>();
                    foreach (string answer in Answers)
                    {
                        TASKDIALOG_BUTTON tASKDIALOG_BUTTON = new TASKDIALOG_BUTTON(answerIndex, answer);
                        answerButtons.Add(tASKDIALOG_BUTTON);
                        answerIndex++;
                    }

                    //Set custom buttons
                    config.pButtons = MarshalButtons(answerButtons);
                    config.cButtons = (uint)answerButtons.Count;
                }
                else
                {
                    config.dwCommonButtons = TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_CLOSE_BUTTON;
                }

                //Show task dialog
                TaskDialogIndirect(config, out int pnButton, out int pnRadioButton, out bool pfverificationFlagChecked);

                //Return answer
                if (Answers != null && Answers.Any())
                {
                    return Answers[pnButton - 100];
                }
                else
                {
                    return ((TASKDIALOG_RESULT)pnButton).ToString();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Task dialog failed: " + ex.Message);
                return string.Empty;
            }
        }

        private static IntPtr MarshalButtons(List<TASKDIALOG_BUTTON> buttonList)
        {
            try
            {
                int buttonSize = Marshal.SizeOf(typeof(TASKDIALOG_BUTTON));
                int totalSize = Marshal.SizeOf(typeof(TASKDIALOG_BUTTON)) * buttonList.Count;
                IntPtr allocPointer = Marshal.AllocHGlobal(totalSize);
                IntPtr currentPointer = allocPointer;

                foreach (TASKDIALOG_BUTTON button in buttonList)
                {
                    Marshal.StructureToPtr(button, currentPointer, false);
                    currentPointer += buttonSize;
                }

                return allocPointer;
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        //Interop
        [DllImport("comctl32.dll", CharSet = CharSet.Unicode)]
        public static extern int TaskDialogIndirect([In] TASKDIALOGCONFIG pTaskConfig, out int pnButton, out int pnRadioButton, [MarshalAs(UnmanagedType.Bool)] out bool pfverificationFlagChecked);

        //Callbacks
        public delegate int PFTASKDIALOGCALLBACK(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, IntPtr lpRefData);
        private int TaskDialogCallback(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, IntPtr lpRefData)
        {
            return 0;
        }

        //Enumerators
        public enum TASKDIALOG_FLAGS : uint
        {
            TDF_NONE = 0x0000,
            TDF_ENABLE_HYPERLINKS = 0x0001,
            TDF_USE_HICON_MAIN = 0x0002,
            TDF_USE_HICON_FOOTER = 0x0004,
            TDF_ALLOW_DIALOG_CANCELLATION = 0x0008,
            TDF_USE_COMMAND_LINKS = 0x0010,
            TDF_USE_COMMAND_LINKS_NO_ICON = 0x0020,
            TDF_EXPAND_FOOTER_AREA = 0x0040,
            TDF_EXPANDED_BY_DEFAULT = 0x0080,
            TDF_VERIFICATION_FLAG_CHECKED = 0x0100,
            TDF_SHOW_PROGRESS_BAR = 0x0200,
            TDF_SHOW_MARQUEE_PROGRESS_BAR = 0x0400,
            TDF_CALLBACK_TIMER = 0x0800,
            TDF_POSITION_RELATIVE_TO_WINDOW = 0x1000,
            TDF_RTL_LAYOUT = 0x2000,
            TDF_NO_DEFAULT_RADIO_BUTTON = 0x4000,
            TDF_CAN_BE_MINIMIZED = 0x8000,
            TDF_NO_SET_FOREGROUND = 0x00010000,
            TDF_SIZE_TO_CONTENT = 0x01000000
        }

        public enum TASKDIALOG_COMMON_BUTTON_FLAGS : uint
        {
            TDCBF_NONE = 0x0000,
            TDCBF_OK_BUTTON = 0x0001,
            TDCBF_YES_BUTTON = 0x0002,
            TDCBF_NO_BUTTON = 0x0004,
            TDCBF_CANCEL_BUTTON = 0x0008,
            TDCBF_RETRY_BUTTON = 0x0010,
            TDCBF_CLOSE_BUTTON = 0x0020
        }

        public enum TASKDIALOG_NOTIFICATIONS : uint
        {
            TDN_CREATED = 0,
            TDN_NAVIGATED = 1,
            TDN_BUTTON_CLICKED = 2,
            TDN_HYPERLINK_CLICKED = 3,
            TDN_TIMER = 4,
            TDN_DESTROYED = 5,
            TDN_RADIO_BUTTON_CLICKED = 6,
            TDN_DIALOG_CONSTRUCTED = 7,
            TDN_VERIFICATION_CLICKED = 8,
            TDN_HELP = 9,
            TDN_EXPANDO_BUTTON_CLICKED = 10
        }

        public enum TASKDIALOG_ICONS : uint
        {
            TD_DEFAULT_ICON = 0,
            TD_WARNING_ICON = ushort.MaxValue,
            TD_ERROR_ICON = ushort.MaxValue - 1,
            TD_INFORMATION_ICON = ushort.MaxValue - 2,
            TD_SHIELD_ICON = ushort.MaxValue - 3,
            ICON_FOLDER = 3,
            ICON_APPLICATION = 15,
            ICON_QUESTION_MARK = 99,
            ICON_SEARCH = 177
        }

        public enum TASKDIALOG_RESULT : uint
        {
            IDOK = 1,
            IDCANCEL = 2,
            IDABORT = 3,
            IDRETRY = 4,
            IDIGNORE = 5,
            IDYES = 6,
            IDNO = 7,
            IDCLOSE = 8,
            IDHELP = 9
        }

        //Structures
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
        public struct TASKDIALOGCONFIG
        {
            public uint cbSize;
            public IntPtr hwndParent;
            public IntPtr hInstance;
            public TASKDIALOG_FLAGS dwFlags;
            public TASKDIALOG_COMMON_BUTTON_FLAGS dwCommonButtons;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszWindowTitle;
            public IntPtr hMainIcon;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszMainInstruction;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszContent;
            public uint cButtons;
            public IntPtr pButtons;
            public int nDefaultButton;
            public uint cRadioButtons;
            public IntPtr pRadioButtons;
            public int nDefaultRadioButton;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszVerificationText;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszExpandedInformation;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszExpandedControlText;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszCollapsedControlText;
            public IntPtr hFooterIcon;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszFooter;
            public PFTASKDIALOGCALLBACK pfCallback;
            public IntPtr lpCallbackData;
            public uint cxWidth;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
        public struct TASKDIALOG_BUTTON
        {
            public int nButtonID;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszButtonText;

            public TASKDIALOG_BUTTON(int buttonId, string buttonText)
            {
                nButtonID = buttonId;
                pszButtonText = buttonText;
            }
        }
    }
}