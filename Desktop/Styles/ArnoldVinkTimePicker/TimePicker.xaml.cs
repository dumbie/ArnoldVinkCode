using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ArnoldVinkCode.Styles
{
    public partial class TimePicker : UserControl
    {
        //It is recommended to make use of CultureInfo.InvariantCulture datetimes.
        public Action<DateTime?> DateTimeChanged;

        public TimePicker()
        {
            InitializeComponent();
        }

        public DateTime? DateTimeValue
        {
            get
            {
                string hours = textbox_Hours.Text;
                string minutes = textbox_Minutes.Text;
                string amPm = textbox_AmPm.Text;
                if (!string.IsNullOrWhiteSpace(hours) && !string.IsNullOrWhiteSpace(minutes) && !string.IsNullOrWhiteSpace(amPm))
                {
                    string rawTime = textbox_Hours.Text + ":" + textbox_Minutes.Text + " " + textbox_AmPm.Text;
                    return DateTime.Parse(rawTime);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                DateTime? dateTime = value;
                if (dateTime.HasValue)
                {
                    int currentHour = dateTime.Value.Hour;
                    textbox_Hours.Text = dateTime.Value.ToString("%h");
                    textbox_Minutes.Text = dateTime.Value.ToString("mm");
                    if (currentHour >= 12)
                    {
                        textbox_AmPm.Text = "PM";
                    }
                    else
                    {
                        textbox_AmPm.Text = "AM";
                    }
                }
            }
        }

        private void button_Down_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FrameworkElement focusedElement = (FrameworkElement)Keyboard.FocusedElement;
                if (focusedElement == textbox_Hours)
                {
                    ChangeHours(false);
                }
                else if (focusedElement == textbox_Minutes)
                {
                    ChangeMinutes(false);
                }
                else if (focusedElement == textbox_AmPm)
                {
                    ChangeAmPm();
                }
            }
            catch { }
        }

        private void button_Up_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FrameworkElement focusedElement = (FrameworkElement)Keyboard.FocusedElement;
                if (focusedElement == textbox_Hours)
                {
                    ChangeHours(true);
                }
                else if (focusedElement == textbox_Minutes)
                {
                    ChangeMinutes(true);
                }
                else if (focusedElement == textbox_AmPm)
                {
                    ChangeAmPm();
                }
            }
            catch { }
        }

        private void textbox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            try
            {
                if (e.Delta > 0)
                {
                    button_Up_Click(this, null);
                }
                else
                {
                    button_Down_Click(this, null);
                }
            }
            catch { }
        }

        private void textbox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Up)
                {
                    button_Up_Click(this, null);
                }
                else if (e.Key == Key.Down)
                {
                    button_Down_Click(this, null);
                }
            }
            catch { }
        }

        private void textbox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key != Key.Up && e.Key != Key.Down)
                {
                    TextBox textBox = (TextBox)sender;
                    ValidateNotifyTime(textBox);
                }
            }
            catch { }
        }

        private void textbox_AmPm_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Up)
                {
                    button_Up_Click(this, null);
                }
                else if (e.Key == Key.Down)
                {
                    button_Down_Click(this, null);
                }
                else if (e.Key != Key.Tab)
                {
                    e.Handled = true;
                }
            }
            catch { }
        }

        private void ChangeHours(bool TimeUp)
        {
            try
            {
                int value = Convert.ToInt32(textbox_Hours.Text);
                if (TimeUp)
                {
                    value += 1;
                    if (value >= 13)
                    {
                        value = 1;
                    }
                }
                else
                {
                    value -= 1;
                    if (value <= 0)
                    {
                        value = 12;
                    }
                }

                textbox_Hours.Text = Convert.ToString(value);
                ValidateNotifyTime(textbox_Hours);
            }
            catch { }
        }

        private void ChangeMinutes(bool isUp)
        {
            try
            {
                int value = Convert.ToInt32(textbox_Minutes.Text);
                if (isUp)
                {
                    value += 1;
                    if (value >= 60)
                    {
                        value = 0;
                    }
                }
                else
                {
                    value -= 1;
                    if (value <= -1)
                    {
                        value = 59;
                    }
                }

                if (value < 10)
                {
                    textbox_Minutes.Text = "0" + Convert.ToString(value);
                }
                else
                {
                    textbox_Minutes.Text = Convert.ToString(value);
                }

                ValidateNotifyTime(textbox_Minutes);
            }
            catch { }
        }

        private void ChangeAmPm()
        {
            try
            {
                if (textbox_AmPm.Text == "AM")
                {
                    textbox_AmPm.Text = "PM";
                }
                else
                {
                    textbox_AmPm.Text = "AM";
                }

                if (DateTimeChanged != null)
                {
                    DateTimeChanged(DateTimeValue);
                }
            }
            catch { }
        }

        private bool ValidateNotifyTime(TextBox textBox)
        {
            try
            {
                //Color brushes
                BrushConverter BrushConvert = new BrushConverter();
                Brush BrushInvalid = BrushConvert.ConvertFromString("#cd1a2b") as Brush;
                Brush BrushValid = BrushConvert.ConvertFromString("#1db954") as Brush;

                //Check for empty string
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    border_TimePicker.BorderBrush = BrushInvalid;
                    return false;
                }

                //Remove invalid characters
                textBox.Text = Regex.Replace(textBox.Text, @"[^\d]", string.Empty);

                //Check if input are numbers
                bool allNumbers = textBox.Text.All(char.IsNumber);
                if (!allNumbers)
                {
                    border_TimePicker.BorderBrush = BrushInvalid;
                    return false;
                }

                //Validate the entered time
                int enteredTime = int.Parse(textBox.Text);
                if (textBox == textbox_Hours && (enteredTime > 12 || enteredTime < 0))
                {
                    border_TimePicker.BorderBrush = BrushInvalid;
                    return false;
                }
                if (textBox == textbox_Minutes && (enteredTime > 59 || enteredTime < 0))
                {
                    border_TimePicker.BorderBrush = BrushInvalid;
                    return false;
                }

                border_TimePicker.BorderBrush = BrushValid;

                if (DateTimeChanged != null)
                {
                    DateTimeChanged(DateTimeValue);
                }
            }
            catch { }
            return true;
        }
    }
}