using System;
using Xamarin.Forms;

namespace ArnoldVinkXaml
{
    public class SliderStep : Slider
    {
        public static readonly BindableProperty CurrentStepValueProperty = BindableProperty.Create<SliderStep, double>(p => p.StepValue, 1.0f);

        public double StepValue
        {
            get { return (double)GetValue(CurrentStepValueProperty); }
            set { SetValue(CurrentStepValueProperty, value); }
        }

        public SliderStep()
        {
            ValueChanged += OnSliderValueChanged;
        }

        private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            double newStep = Math.Round(e.NewValue / StepValue);
            Value = newStep * StepValue;
        }
    }
}