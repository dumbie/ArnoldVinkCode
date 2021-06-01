using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ArnoldVinkCode
{
    public partial class AVMessageBox : Window
    {
        //Window Initialize
        public AVMessageBox()
        {
            try
            {
                InitializeComponent();
            }
            catch
            {
                Debug.WriteLine("Failed to initialize messagebox, check app.xaml styles.");
            }
        }

        //Popup Variables
        private bool vPopupDone = false;
        private string vPopupResult = string.Empty;

        //Show and close popup
        public async Task<string> Popup(FrameworkElement disableElement, string Question, string Description, List<string> Answers)
        {
            try
            {
                //Disable the source frameworkelement
                if (disableElement != null)
                {
                    disableElement.IsEnabled = false;
                }

                //Set messagebox question
                if (!string.IsNullOrWhiteSpace(Question))
                {
                    grid_MessageBox_Question.Text = Question;
                    grid_MessageBox_Question.Visibility = Visibility.Visible;
                    grid_MessageBox_Border.Visibility = Visibility.Visible;
                }
                else
                {
                    grid_MessageBox_Question.Text = string.Empty;
                    grid_MessageBox_Question.Visibility = Visibility.Collapsed;
                    grid_MessageBox_Border.Visibility = Visibility.Collapsed;
                }

                //Set messagebox description
                if (!string.IsNullOrWhiteSpace(Description))
                {
                    grid_MessageBox_Description.Text = Description;
                    grid_MessageBox_Description.Visibility = Visibility.Visible;
                }
                else
                {
                    grid_MessageBox_Description.Text = string.Empty;
                    grid_MessageBox_Description.Visibility = Visibility.Collapsed;
                }

                //Set the messagebox answers
                listbox_MessageBox.ItemsSource = Answers;
                listbox_MessageBox.SelectedIndex = 0;

                //Reset popup variables
                vPopupResult = string.Empty;
                vPopupDone = false;

                //Show the popup
                Show();

                //Wait for user messagebox input
                while (vPopupResult == string.Empty && !vPopupDone && this.IsVisible) { await Task.Delay(500); }

                //Enable the source frameworkelement
                if (disableElement != null)
                {
                    disableElement.IsEnabled = true;
                }

                //Close the messagebox popup
                Close();
            }
            catch { }
            return vPopupResult;
        }

        //Set the popup result
        private void listbox_MessageBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button originalSource = (Button)e.OriginalSource;
                vPopupResult = originalSource.Content.ToString();
                vPopupDone = true;
                Debug.WriteLine("Selected messagebox answer: " + vPopupResult);
            }
            catch { }
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