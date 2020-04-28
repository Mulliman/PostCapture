using ImageTools.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageTools.Configurator.ApplierForms
{
    /// <summary>
    /// Interaction logic for PercentageBorderForm.xaml
    /// </summary>
    public partial class PercentageBorderForm : UserControl, IApplierForm<PercentageBorderApplier>
    {
        public double PercentageBorderWidth { get; set; }

        public double Red { get; set; }

        public double Green { get; set; }

        public double Blue { get; set; }

        public PercentageBorderForm()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        public PercentageBorderForm(PercentageBorderApplier applier)
        {
            InitializeComponent();

            PercentageBorderWidth = applier.BorderWidthPercentage;
            Red = applier.Colour.R;
            Green = applier.Colour.G;
            Blue = applier.Colour.B;

            this.DataContext = this;
        }

        private void ColorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Color color = Color.FromRgb((byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value);
            
            if(DemoLine != null)
            {
                DemoLine.Fill = new SolidColorBrush(color);
            }
        }

        private void WidthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(DemoLine != null)
            {
                DemoLine.Height = PercentageBorderWidthSlider.Value;
            }
        }
    }

    public class PercentageBorderFormBuilder : IApplierFormBuilder
    {
        public UserControl Build(Dictionary<string, string> parameters)
        {
            var applier = new PercentageBorderApplier(parameters);
            return new PercentageBorderForm(applier);
        }
    }
}
