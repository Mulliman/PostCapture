using ImageTools.Configurator.ApplierForms;
using ImageTools.Core;
using ImageTools.Core.Builder;
using ImageTools.Core.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
                    control.OnUpdate += new EventHandler(ShowUpdatedPreviewImageFromEvent);
                }
            }

            ShowUpdatedPreviewImage();
        }

        private void ShowUpdatedPreviewImageFromEvent(object sender, EventArgs args)
        {
            SelectedProcessFile = GetProcessFileFromCurrentUiState();
            ShowUpdatedPreviewImage();
        }

        private void ShowUpdatedPreviewImage()
        {
            var testImageMemoryStream = new MemoryStream();
            Properties.Resources.TestImage.Save(testImageMemoryStream, ImageFormat.Jpeg);
            testImageMemoryStream.Seek(0, SeekOrigin.Begin);

            var exampleImage = new EditableImage("test.jpg", new ImageMagick.MagickImage(testImageMemoryStream));
            var builder = new ApplierBuilder();

            foreach (var step in SelectedProcessFile.Steps)
            {
                var applier = builder.CreateApplier(step);
                exampleImage = applier.Apply(exampleImage);
            }

            var memoryStream = new MemoryStream();
            memoryStream.Seek(0, SeekOrigin.Begin);
            exampleImage.Image.Write(memoryStream, ImageMagick.MagickFormat.Jpg);
            memoryStream.Seek(0, SeekOrigin.Begin);

            var imageSource = new BitmapImage();
            imageSource.BeginInit();
            imageSource.StreamSource = memoryStream;
            imageSource.EndInit();

            PreviewImage.Source = imageSource;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you want to overwrite the existing file?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ProcessConfigurationFile processFile = GetProcessFileFromCurrentUiState();

                _processRepo.SaveProcessConfigurationFile(processFile);

                SelectedProcessFile = processFile;
            }
        }

        private ProcessConfigurationFile GetProcessFileFromCurrentUiState()
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

            return processFile;
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
