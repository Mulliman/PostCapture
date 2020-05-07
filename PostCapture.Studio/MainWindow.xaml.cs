using PostCapture.Studio.ApplierForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PostCapture.Core.Builder;
using PostCapture.Core.Selection;
using PostCapture.Core;

namespace PostCapture.Studio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly ProcessStepRepository _processRepo;

        public MainWindow()
        {
            InitializeComponent();

            DisableEditing();

            var folder = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "processes");
            _processRepo = new ProcessStepRepository(folder);

            RefreshProcessListBox();

            Forms = new Dictionary<string, IApplierFormBuilder>
            {
                { PercentageBorderApplier.IdConst, new PercentageBorderFormBuilder() },
                { WatermarkApplier.IdConst, new WatermarkFormBuilder() }
            };

            OperationsListBox.ItemsSource = Forms;

            DataContext = this;
        }

        public MainWindow(string exampleFile) : this()
        {
            ShowExampleImageOptions(exampleFile);
        }

        public List<ProcessConfigurationFile> Processes { get; set; }

        public Dictionary<string, IApplierFormBuilder>  Forms { get; set; }

        private ProcessConfigurationFile _selectedProcessFile;

        public ProcessConfigurationFile SelectedProcessFile
        {
            get { return _selectedProcessFile; }
            set
            {
                if (_selectedProcessFile != value)
                {
                    _selectedProcessFile = value;
                    OnPropertyChanged("SelectedProcessFile");
                }
            }
        }

        public bool IsGridEnabled
        {
            get
            {
                return this.SelectedProcessFile != null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void EnableEditing()
        {
            Column1.IsEnabled = true;
            Column1.Opacity = 1;

            Column2.IsEnabled = true;
            Column2.Opacity = 1;
            
        }
        public void DisableEditing()
        {
            Column1.IsEnabled = false;
            Column1.Opacity = 0.3;

            Column2.IsEnabled = false;
            Column2.Opacity = 0.3;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void ConfigsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SelectedProcessFile = ConfigsListBox.SelectedItem as ProcessConfigurationFile;

            ProcessOperationsPanel.Children.Clear();

            if (SelectedProcessFile == null)
            {
                ProcessNameTextBox.Text = null;
                MatchPropertyTextBox.Text = null;
                MatchValueTextBox.Text = null;
                PriorityTextBox.Text = null;

                CreateCopyPanel.Visibility = Visibility.Collapsed;

                ShowUpdatedPreviewImage();

                DisableEditing();

                return;
            }

            ProcessNameTextBox.Text = SelectedProcessFile.Id;
            MatchPropertyTextBox.Text = SelectedProcessFile.MatchProperty;
            MatchValueTextBox.Text = SelectedProcessFile.MatchValue;
            PriorityTextBox.Text = SelectedProcessFile.Priority.ToString();

            foreach (var step in SelectedProcessFile.Steps)
            {
                if (Forms.ContainsKey(step.Id))
                {
                    var control = Forms[step.Id].Build(step.Parameters);
                    ProcessOperationsPanel.Children.Add(control);

                    RegisterSubFormsEvents(control);
                }
            }

            CreateCopyPanel.Visibility = Visibility.Visible;
            ShowUpdatedPreviewImage();

            EnableEditing();
        }

        private void ShowUpdatedPreviewImageFromEvent(object sender, EventArgs args)
        {
            SelectedProcessFile = GetProcessFileFromCurrentUiState();
            ShowUpdatedPreviewImage();
        }

        private void ShowUpdatedPreviewImage()
        {
            if(SelectedProcessFile?.Steps == null)
            {
                PreviewImage.Source = null;
                return;
            }

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

        private void RefreshProcessListBox(ProcessConfigurationFile selectedProcessFile = null)
        {
            Processes = _processRepo.Configs?.OrderBy(c => c.Priority)?.ToList();
            ConfigsListBox.ItemsSource = Processes;
            ConfigsListBox.Items.Refresh();

            if (selectedProcessFile != null)
            {
                SelectedProcessFile = Processes.FirstOrDefault(p => p.Id == selectedProcessFile.Id);
                ConfigsListBox.SelectedItem = SelectedProcessFile;
            }    
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you want to overwrite the existing file?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ProcessConfigurationFile processFile = GetProcessFileFromCurrentUiState();

                _processRepo.SaveProcessConfigurationFile(processFile);

                RefreshProcessListBox(processFile);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Do you want to delete the selected file {SelectedProcessFile.Id}?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _processRepo.DeleteProcessConfigurationFile(SelectedProcessFile);

                SelectedProcessFile = null;
                RefreshProcessListBox(null);
                DisableEditing();
            }
        }

        private void OperationsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (OperationsListBox.SelectedItem == null)
            {
                return;
            }

            var selectedItem = (KeyValuePair<string, IApplierFormBuilder>)OperationsListBox.SelectedItem;

            var newForm = selectedItem.Value.Build(null);

            ProcessOperationsPanel.Children.Add(newForm);
            RegisterSubFormsEvents(newForm);

            OperationsListBox.UnselectAll();
        }

        private void RegisterSubFormsEvents(ApplierFormUserControl formUserControl)
        {
            formUserControl.OnUpdate += new EventHandler(ShowUpdatedPreviewImageFromEvent);
            formUserControl.OnDelete += new EventHandler(OnOperationDeleted);
            formUserControl.OnMove += new EventHandler(OnOperationMoved);
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
            processFile.Id = ProcessNameTextBox.Text;
            processFile.MatchProperty = MatchPropertyTextBox.Text;
            processFile.MatchValue = MatchValueTextBox.Text;

            var priority = processFile.Priority;
            int.TryParse(PriorityTextBox.Text ?? "0", out priority);
            processFile.Priority = priority;

            return processFile;
        }

        private void OnOperationDeleted(object sender, EventArgs args)
        {
            var typedArgs = args as FormDeletedEventArgs;
            var id = typedArgs.ApplierFormInstanceId;

            foreach(var item in ProcessOperationsPanel.Children)
            {
                var child = item as ApplierFormUserControl;
                if(child.ApplierFormInstanceId == id)
                {
                    ProcessOperationsPanel.Children.Remove(child);
                    break;
                }
            }

            SelectedProcessFile = GetProcessFileFromCurrentUiState();
            ShowUpdatedPreviewImage();
        }

        private void OnOperationMoved(object sender, EventArgs args)
        {
            var typedArgs = args as FormMovedEventArgs;
            var id = typedArgs.ApplierFormInstanceId;

            foreach (var item in ProcessOperationsPanel.Children)
            {
                var child = item as ApplierFormUserControl;
                if (child.ApplierFormInstanceId == id)
                {
                    var oldIndex = ProcessOperationsPanel.Children.IndexOf(child);

                    if(oldIndex == 0 && typedArgs.Direction == MoveDirection.Up)
                    {
                        return;
                    }

                    if (oldIndex == ProcessOperationsPanel.Children.Count -1 && typedArgs.Direction == MoveDirection.Down)
                    {
                        return;
                    }

                    var newIndex = typedArgs.Direction == MoveDirection.Up
                        ? oldIndex - 1
                        : oldIndex + 1;

                    var toMove = ProcessOperationsPanel.Children[oldIndex];
                    ProcessOperationsPanel.Children.Remove(toMove);
                    ProcessOperationsPanel.Children.Insert(newIndex, toMove);

                    break;
                }
            }

            SelectedProcessFile = GetProcessFileFromCurrentUiState();
            ShowUpdatedPreviewImage();
        }

        private void CreateProcessButton_Click(object sender, RoutedEventArgs e)
        {
            var id = NewProcessTextBox.Text;

            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("You must enter a ID for the new process.");
                return;
            }

            if (!_processRepo.IsSafeId(id))
            {
                MessageBox.Show("A process with this ID already exists");
                return;
            }

            var newFile = new ProcessConfigurationFile
            {
                Id = id,
                Filename = _processRepo.GetSafeFilename(id),
                Steps = new List<ProcessStepConfiguration>()
            };

            if (CreateCopyCheckBox.IsChecked == true)
            {
                var selected = SelectedProcessFile;

                newFile.Steps = selected.Steps;
                newFile.MatchProperty = selected.MatchProperty;
                newFile.MatchValue = selected.MatchValue;
            }

            if(ChooseExampleImageCheckBox.IsChecked == true && MatchCategoryAndValueComboBox.SelectedItem != null)
            {
                var selected = (KeyValuePair<string,string>)MatchCategoryAndValueComboBox.SelectedItem;

                newFile.MatchProperty = selected.Key;
                newFile.MatchValue = selected.Value;
            }

            _processRepo.CreateProcessConfigurationFile(newFile);
            RefreshProcessListBox(newFile);

            NewProcessTextBox.Text = string.Empty;
            EnableEditing();
        }

        private void CreateCopyCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (CreateCopyCheckBox.IsChecked == true)
            {
                CreateProcessButton.Content = "Create Copy";
            }
            else
            {
                CreateProcessButton.Content = "Create";
            }
        }

        private void ChooseExampleImageCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if(ChooseExampleImageCheckBox.IsChecked == true)
            {
                UploadExampleImagePanel.Visibility = Visibility.Visible;
            }
            else
            {
                UploadExampleImagePanel.Visibility = Visibility.Collapsed;
            }
        }

        private void ExampleImageBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Jpegs|*.jpeg;*.jpg;"
            };

            var result = fileDialog.ShowDialog();

            if (result == true)
            {
                var file = fileDialog.FileName;
                ShowExampleImageOptions(file);
            }
        }

        private void ShowExampleImageOptions(string file)
        {
            try
            {
                var dict = MetadataSelector.GetCategoryValueDictionary(new ImageFile(file));
                ExampleImageFileNameTextBox.Text = Path.GetFileName(file);

                MatchCategoryAndValueComboBox.ItemsSource = dict;

                ChooseExampleImageCheckBox.IsChecked = true;
                UploadExampleImagePanel.Visibility = Visibility.Visible;
                MatchCategoryAndValueComboBox.Visibility = Visibility.Visible;
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to get metadata for file.");
                MatchCategoryAndValueComboBox.Visibility = Visibility.Collapsed;
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void NumberValidationTextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));

                Regex regex = new Regex("[^0-9]+");
                var isValid = regex.IsMatch(text);

                if (isValid)
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
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
