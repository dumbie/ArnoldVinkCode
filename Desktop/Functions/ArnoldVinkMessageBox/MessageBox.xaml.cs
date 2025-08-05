using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ArnoldVinkCode
{
    public class AVMessageBox
    {
        public static string Popup(dynamic disableElement, string Question, string Description, List<string> Answers, string ColorHex="")
        {
            return new AVMessageBoxPrivate().Popup(disableElement, Question, Description, Answers, ColorHex);
        }
    }

    public partial class AVMessageBoxPrivate : Window
    {
        //Window Initialize
        public AVMessageBoxPrivate()
        {
            try
            {
                InitializeComponent();
            }
            catch
            {
                Debug.WriteLine("Failed to initialize messagebox.");
            }
        }

        //Popup Variables
        private string vPopupResult = string.Empty;
        private bool vPopupAllowClose = false;

        //Show popup
        public string Popup(dynamic disableElement, string Question, string Description, List<string> Answers, string ColorHex = "")
        {
            try
            {
                //Disable source framework element
                if (disableElement != null)
                {
                    disableElement.IsEnabled = false;
                }

                //Set messagebox color
                if (!string.IsNullOrWhiteSpace(ColorHex))
                {
                    this.Resources["MessageAccentBrush"] = new BrushConverter().ConvertFrom(ColorHex) as SolidColorBrush;
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
                    grid_MessageBox_Border.Visibility = Visibility.Visible;
                }
                else
                {
                    grid_MessageBox_Description.Text = string.Empty;
                    grid_MessageBox_Description.Visibility = Visibility.Collapsed;
                    grid_MessageBox_Border.Visibility = Visibility.Collapsed;
                }

                //Check messagebox answers
                if (Answers == null || Answers.Count == 0)
                {
                    Answers = ["Close"];
                }

                //Set messagebox answers
                listbox_MessageBox.ItemsSource = Answers;
                listbox_MessageBox.SelectedIndex = 0;

                //Focus on listbox
                listbox_MessageBox.Focus();

                //Reset popup variables
                vPopupResult = string.Empty;
                vPopupAllowClose = false;

                //Show messagebox popup
                ShowDialog();

                //Enable source framework element
                if (disableElement != null)
                {
                    disableElement.IsEnabled = true;
                }

                //Return result
                Debug.WriteLine("Selected messagebox answer: " + vPopupResult);
                return vPopupResult;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("AVMessageBox failed: " + ex.Message);
                return string.Empty;
            }
        }

        //Set popup result
        private void Listbox_MessageBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Get pressed button
                Button originalSource = (Button)e.OriginalSource;

                //Allow closing popup
                vPopupAllowClose = true;

                //Set selected answer
                vPopupResult = originalSource.Content.ToString();

                //Close messagebox popup
                Close();
            }
            catch { }
        }

        //Drag window around
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

        //Prevent popup closing
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (!vPopupAllowClose)
                {
                    e.Cancel = true;
                }
            }
            catch { }
        }
    }
}