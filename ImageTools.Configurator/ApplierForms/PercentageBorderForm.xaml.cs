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
    public partial class PercentageBorderForm : UserControl, IApplierForm
    {
        public PercentageBorderForm()
        {
            InitializeComponent();
        }
    }

    public class PercentageBorderFormBuilder : IApplierFormBuilder
    {
        public IApplierForm Build()
        {
            return new PercentageBorderForm();
        }
    }
}
