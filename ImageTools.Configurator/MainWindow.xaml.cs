using ImageTools.Configurator.ApplierForms;
using ImageTools.Core;
using ImageTools.Core.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ImageTools.Configurator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ProcessStepRepository _processRepo;

        public MainWindow()
        {
            InitializeComponent();

            //var folder = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "processes");
            var folder = @"C:\Development\Photography\ImageTools\ImageTools\PostProcess\bin\Debug\netcoreapp3.1\processes";
            _processRepo = new ProcessStepRepository(folder);

            Processes = _processRepo.Configs;

            ConfigsListBox.ItemsSource = Processes;

            Forms = new Dictionary<string, IApplierFormBuilder>
            {
                { PercentageBorderApplier.IdConst, new PercentageBorderFormBuilder() }
            };

            OperationsListBox.ItemsSource = Forms;
        }

        public List<ProcessConfiguration> Processes { get; set; }

        public Dictionary<string, IApplierFormBuilder>  Forms { get; set; }
    }
}
