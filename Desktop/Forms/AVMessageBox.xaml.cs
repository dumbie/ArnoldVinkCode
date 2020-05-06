using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AVForms
{
    public partial class AVMessageBox : Window
    {
        //Window Initialize
        public AVMessageBox() { InitializeComponent(); }

        //MessageBox Variables
        private static bool vMessageBoxPopupCancelled = false;
        private static int vMessageBoxPopupResult = 0;
        private static AVMessageBox vAVMessageBox = null;

        //Set MessageBox Popup Result
        void grid_MessageBox_Btn1_Click(object sender, RoutedEventArgs e) { vMessageBoxPopupResult = 1; }
        void grid_MessageBox_Btn2_Click(object sender, RoutedEventArgs e) { vMessageBoxPopupResult = 2; }
        void grid_MessageBox_Btn3_Click(object sender, RoutedEventArgs e) { vMessageBoxPopupResult = 3; }
        void grid_MessageBox_Btn4_Click(object sender, RoutedEventArgs e) { vMessageBoxPopupResult = 4; }

        //Show and close Messagebox Popup
        public async static Task<int> MessageBoxPopup(FrameworkElement disableElement, string Question, string Description, string Answer1, string Answer2, string Answer3, string Answer4)
        {
            try
            {
                //Set the variable class
                if (vAVMessageBox == null)
                {
                    vAVMessageBox = new AVMessageBox();
                }
                else
                {
                    Debug.WriteLine("AVMessageBox is already open.");
                    return -1;
                }

                //Disable the source frameworkelement
                if (disableElement != null)
                {
                    disableElement.IsEnabled = false;
                }

                //Set messagebox question content
                vAVMessageBox.grid_MessageBox_Text.Text = Question;
                if (!string.IsNullOrWhiteSpace(Description))
                {
                    vAVMessageBox.grid_MessageBox_Description.Text = Description;
                    vAVMessageBox.grid_MessageBox_Description.Visibility = Visibility.Visible;
                }
                else
                {
                    vAVMessageBox.grid_MessageBox_Description.Text = "";
                    vAVMessageBox.grid_MessageBox_Description.Visibility = Visibility.Collapsed;
                }
                if (!string.IsNullOrWhiteSpace(Answer1))
                {
                    vAVMessageBox.grid_MessageBox_Btn1.Content = Answer1;
                    vAVMessageBox.grid_MessageBox_Btn1.Visibility = Visibility.Visible;
                }
                else
                {
                    vAVMessageBox.grid_MessageBox_Btn1.Content = "";
                    vAVMessageBox.grid_MessageBox_Btn1.Visibility = Visibility.Collapsed;
                }
                if (!string.IsNullOrWhiteSpace(Answer2))
                {
                    vAVMessageBox.grid_MessageBox_Btn2.Content = Answer2;
                    vAVMessageBox.grid_MessageBox_Btn2.Visibility = Visibility.Visible;
                }
                else
                {
                    vAVMessageBox.grid_MessageBox_Btn2.Content = "";
                    vAVMessageBox.grid_MessageBox_Btn2.Visibility = Visibility.Collapsed;
                }
                if (!string.IsNullOrWhiteSpace(Answer3))
                {
                    vAVMessageBox.grid_MessageBox_Btn3.Content = Answer3;
                    vAVMessageBox.grid_MessageBox_Btn3.Visibility = Visibility.Visible;
                }
                else
                {
                    vAVMessageBox.grid_MessageBox_Btn3.Content = "";
                    vAVMessageBox.grid_MessageBox_Btn3.Visibility = Visibility.Collapsed;
                }
                if (!string.IsNullOrWhiteSpace(Answer4))
                {
                    vAVMessageBox.grid_MessageBox_Btn4.Content = Answer4;
                    vAVMessageBox.grid_MessageBox_Btn4.Visibility = Visibility.Visible;
                }
                else
                {
                    vAVMessageBox.grid_MessageBox_Btn4.Content = "";
                    vAVMessageBox.grid_MessageBox_Btn4.Visibility = Visibility.Collapsed;
                }

                //Reset messagebox variables
                vMessageBoxPopupResult = 0;
                vMessageBoxPopupCancelled = false;

                //Show the messagebox popup
                vAVMessageBox.Show();

                //Wait for user messagebox input
                while (vMessageBoxPopupResult == 0 && !vMessageBoxPopupCancelled) { await Task.Delay(500); }
                if (vMessageBoxPopupCancelled) { return 0; }

                //Enable the source frameworkelement
                if (disableElement != null)
                {
                    disableElement.IsEnabled = true;
                }

                //Hide the messagebox popup
                vAVMessageBox.Hide();
                vAVMessageBox = null;
            }
            catch { }
            return vMessageBoxPopupResult;
        }

        //Drag the window around
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            }
            catch { }
        }
    }
}