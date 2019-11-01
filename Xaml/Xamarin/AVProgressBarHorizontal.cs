using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace ArnoldVinkXaml
{
    public partial class ProgressBarHorizontal : StackLayout
    {
        public static readonly BindableProperty ProgressValueProperty = BindableProperty.Create(nameof(ProgressValue), typeof(string), typeof(StackLayout));

        public string ProgressValue
        {
            get
            {
                if(base.GetValue(ProgressValueProperty) != null)
                    {
                    string GetValue = base.GetValue(ProgressValueProperty).ToString();
                    Debug.WriteLine("Do some magic with:" + GetValue);
                    return GetValue + "MagicHappend";
                }
                return String.Empty;
            }
            set
            {
                SetValue(ProgressValueProperty, value);
                this.testme.Text = value;
            }
        }


//        public static readonly BindableProperty ValueProperty =
//BindableProperty.Create(propertyName: nameof(Value),
//declaringType: typeof(MyUserControl),
//returnType: typeof(double?),
//defaultValue: null);



        public ProgressBarHorizontal()
        {
            InitializeComponent();
        }
    }
}