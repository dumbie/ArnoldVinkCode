using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ArnoldVinkCode
{
    public partial class AVMessageBox : Window
    {
        //Window Initialize
        public AVMessageBox() { InitializeComponent(); }

        //MessageBox Variables
        private bool vMessageBoxPopupCancelled = false;
        private int vMessageBoxPopupResult = 0;

        //Set MessageBox Popup Result
        void grid_MessageBox_Btn1_Click(object sender, RoutedEventArgs e) { vMessageBoxPopupResult = 1; }
        void grid_MessageBox_Btn2_Click(object sender, RoutedEventArgs e) { vMessageBoxPopupResult = 2; }
        void grid_MessageBox_Btn3_Click(object sender, RoutedEventArgs e) { vMessageBoxPopupResult = 3; }
        void grid_MessageBox_Btn4_Click(object sender, RoutedEventArgs e) { vMessageBoxPopupResult = 4; }

        //Show and close Messagebox Popup
        public async Task<int> Popup(FrameworkElement disableElement, string Question, string Description, string Answer1, string Answer2, string Answer3, string Answer4)
        {
            try
            {
                //Disable the source frameworkelement
                if (disableElement != null)
                {
                    disableElement.IsEnabled = false;
                }

                //Set messagebox question content
                grid_MessageBox_Text.Text = Question;
                if (!string.IsNullOrWhiteSpace(Description))
                {
                    grid_MessageBox_Description.Text = Description;
                    grid_MessageBox_Description.Visibility = Visibility.Visible;
                }
                else
                {
                    grid_MessageBox_Description.Text = "";
                    grid_MessageBox_Description.Visibility = Visibility.Collapsed;
                }
                if (!string.IsNullOrWhiteSpace(Answer1))
                {
                    grid_MessageBox_Btn1.Content = Answer1;
                    grid_MessageBox_Btn1.Visibility = Visibility.Visible;
                }
                else
                {
                    grid_MessageBox_Btn1.Content = "";
                    grid_MessageBox_Btn1.Visibility = Visibility.Collapsed;
                }
                if (!string.IsNullOrWhiteSpace(Answer2))
                {
                    grid_MessageBox_Btn2.Content = Answer2;
                    grid_MessageBox_Btn2.Visibility = Visibility.Visible;
                }
                else
                {
                    grid_MessageBox_Btn2.Content = "";
                    grid_MessageBox_Btn2.Visibility = Visibility.Collapsed;
                }
                if (!string.IsNullOrWhiteSpace(Answer3))
                {
                    grid_MessageBox_Btn3.Content = Answer3;
                    grid_MessageBox_Btn3.Visibility = Visibility.Visible;
                }
                else
                {
                    grid_MessageBox_Btn3.Content = "";
                    grid_MessageBox_Btn3.Visibility = Visibility.Collapsed;
                }
                if (!string.IsNullOrWhiteSpace(Answer4))
                {
                    grid_MessageBox_Btn4.Content = Answer4;
                    grid_MessageBox_Btn4.Visibility = Visibility.Visible;
                }
                else
                {
                    grid_MessageBox_Btn4.Content = "";
                    grid_MessageBox_Btn4.Visibility = Visibility.Collapsed;
                }

                //Reset messagebox variables
                vMessageBoxPopupResult = 0;
                vMessageBoxPopupCancelled = false;

                //Show the messagebox popup
                Show();

                //Wait for user messagebox input
                while (vMessageBoxPopupResult == 0 && !vMessageBoxPopupCancelled) { await Task.Delay(500); }
                if (vMessageBoxPopupCancelled) { return 0; }

                //Enable the source frameworkelement
                if (disableElement != null)
                {
                    disableElement.IsEnabled = true;
                }

                //Close the messagebox popup
                Close();
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