using ImageTools.Core;
using ImageTools.Core.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageTools.Configurator.ApplierForms
{
    /// <summary>
    /// Interaction logic for WatermarkForm.xaml
    /// </summary>
    public partial class WatermarkForm : ApplierFormUserControl, IApplierForm<WatermarkApplier>
    {
        public string WatermarkImage { get; set; }

        public double PercentageMargin { get; set; }

        public double PercentageSize { get; set; }

        public Location Location { get; set; }

        public WatermarkForm()
        {
            InitializeComponent();

            DataContext = this;

            LocationComboBox.ItemsSource = Enum.GetValues(typeof(Location)).Cast<Location>();
            LocationComboBox.SelectedItem = Location;
        }

        public WatermarkForm(WatermarkApplier applier)
        {
            InitializeComponent();

            DataContext = this;

            WatermarkImage = applier.WatermarkImage.Path;
            PercentageMargin = applier.WatermarkLocation.ImageMarginPercentage;
            PercentageSize = applier.WatermarkLocation.ImageSizePercentage;
            Location = applier.WatermarkLocation.Location;

            LocationComboBox.ItemsSource = Enum.GetValues(typeof(Location)).Cast<Location>();
            LocationComboBox.SelectedItem = Location;

            SetUpImage();
        }

        public override ProcessStepConfiguration GetData()
        {
            return new ProcessStepConfiguration
            {
                Id = WatermarkApplier.IdConst,
                Parameters = new Dictionary<string, string>
                {
                    { "WatermarkImage", this.WatermarkImage },
                    { "ImageMarginPercentage", this.PercentageMargin.ToString() },
                    { "ImageSizePercentage", this.PercentageSize.ToString() },
                    { "Location", Location.ToString("D") },
                }
            };
        }

        private void SetUpImage()
        {
            var image = new ImageFile(WatermarkImage);

            Uri fileUri = new Uri(image.Path);
            ExampleImage.Source = new BitmapImage(fileUri);

            switch (Location)
            {
                case Location.BottomLeft:
                    ExampleImage.HorizontalAlignment = HorizontalAlignment.Left;
                    break;

                case Location.BottomRight:
                    ExampleImage.HorizontalAlignment = HorizontalAlignment.Right;
                    break;

                case Location.Bottom:
                default:
                    ExampleImage.HorizontalAlignment = HorizontalAlignment.Center;
                    break;
            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();

            fileDialog.Filter = "Image files|*.png;*.jpeg;*.jpg;*.gif";

            // Launch OpenFileDialog by calling ShowDialog method
            var result = fileDialog.ShowDialog();

            // Get the selected file name and display in a TextBox.
            // Load content of file in a TextBlock
            if (result == true)
            {
                WatermarkImage = fileDialog.FileName;
                FileNameTextBox.Text = WatermarkImage;
            }

            SetUpImage();
        }
    }

    public class WatermarkFormBuilder : IApplierFormBuilder
    {
        public ApplierFormUserControl Build(Dictionary<string, string> parameters)
        {
            var applier = new WatermarkApplier(parameters);
            return new WatermarkForm(applier);
        }
    }
}