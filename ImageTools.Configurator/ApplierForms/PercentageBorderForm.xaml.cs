using ImageTools.Core;
using ImageTools.Core.Selection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ImageTools.Configurator.ApplierForms
{

    /// <summary>
    /// Interaction logic for PercentageBorderForm.xaml
    /// </summary>
    public partial class PercentageBorderForm : ApplierFormUserControl, IApplierForm<PercentageBorderApplier>
    {
        public double PercentageBorderWidth { get; set; }

        public double Red { get; set; }

        public double Green { get; set; }

        public double Blue { get; set; }

        public PercentageBorderForm()
        {
            InitializeComponent();

            base.DataContext = this;
        }

        public PercentageBorderForm(PercentageBorderApplier applier)
        {
            InitializeComponent();

            PercentageBorderWidth = applier.BorderWidthPercentage;
            Red = applier.Colour.R;
            Green = applier.Colour.G;
            Blue = applier.Colour.B;

            base.DataContext = this;
        }

        private void ColorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Color color = Color.FromRgb((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value);
            
            if(DemoLine != null)
            {
                DemoLine.Fill = new SolidColorBrush(color);
            }

            SendOnUpdateEvent(sender, e);
        }

        private void WidthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(DemoLine != null)
            {
                DemoLine.Height = PercentageBorderWidthSlider.Value;
            }

            SendOnUpdateEvent(sender, e);
        }

        public override ProcessStepConfiguration GetData()
        {
            return new ProcessStepConfiguration
            {
                Id = PercentageBorderApplier.IdConst,
                Parameters = new Dictionary<string, string>
                {
                    { "BorderWidthPercentage", this.PercentageBorderWidth.ToString() },
                    { "R", this.Red.ToString() },
                    { "G", this.Green.ToString() },
                    { "B", this.Blue.ToString() },
                }
            };
        }
    }

    public class PercentageBorderFormBuilder : IApplierFormBuilder
    {
        public ApplierFormUserControl Build(Dictionary<string, string> parameters)
        {
            var applier = new PercentageBorderApplier(parameters);
            return new PercentageBorderForm(applier);
        }
    }
}
