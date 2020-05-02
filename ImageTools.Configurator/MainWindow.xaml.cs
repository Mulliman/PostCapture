using ImageTools.Configurator.ApplierForms;
using ImageTools.Core;
using ImageTools.Core.Builder;
using ImageTools.Core.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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
                { PercentageBorderApplier.IdConst, new PercentageBorderFormBuilder() },
                { WatermarkApplier.IdConst, new WatermarkFormBuilder() }
            };

            OperationsListBox.ItemsSource = Forms;

            DataContext = this;
        }

        public List<ProcessConfigurationFile> Processes { get; set; }

        public Dictionary<string, IApplierFormBuilder>  Forms { get; set; }

        public ProcessConfigurationFile SelectedProcessFile { get; set; }

        private void ConfigsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SelectedProcessFile = ConfigsListBox.SelectedItem as ProcessConfigurationFile;

            ProcessNameTextBox.Text = SelectedProcessFile.Id;
            MatchPropertyTextBox.Text = SelectedProcessFile.MatchProperty;
            MatchValueTextBox.Text = SelectedProcessFile.MatchValue;

            ProcessOperationsPanel.Children.Clear();

            foreach (var step in SelectedProcessFile.Steps)
            {
                if (Forms.ContainsKey(step.Id))
                {
                    var control = Forms[step.Id].Build(step.Parameters);
                    ProcessOperationsPanel.Children.Add(control);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you want to overwrite the existing file?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var processFile = SelectedProcessFile;

                var steps = new List<ProcessStepConfiguration>();

                foreach (var control in ProcessOperationsPanel.Children.Cast<ApplierFormUserControl>())
                {
                    var data = control.GetData();
                    steps.Add(data);
                }

                processFile.Steps = steps;
                processFile.Id = SelectedProcessFile.Id;
                processFile.MatchProperty = SelectedProcessFile.MatchProperty;
                processFile.MatchValue = SelectedProcessFile.MatchValue;
                processFile.Id = SelectedProcessFile.Id;

                _processRepo.SaveProcessConfigurationFile(processFile);
            }
        }

        #region Window Functionality 

        /// <summary>
        /// TitleBar_MouseDown - Drag if single-click, resize if double-click
        /// </summary>
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                if (e.ClickCount == 2)
                {
                    AdjustWindowSize();
                }
                else
                {
                    Application.Current.MainWindow.DragMove();
                }
        }

        /// <summary>
        /// CloseButton_Clicked
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// MaximizedButton_Clicked
        /// </summary>
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            AdjustWindowSize();
        }

        /// <summary>
        /// Minimized Button_Clicked
        /// </summary>
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Adjusts the WindowSize to correct parameters when Maximize button is clicked
        /// </summary>
        private void AdjustWindowSize()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }

        }

        #endregion
    }
}
