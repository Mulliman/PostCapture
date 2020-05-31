using PostCapture.Core;
using PostCapture.Core.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PostCapture.Studio.ApplierForms
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

            WatermarkImage = applier?.WatermarkImage?.Path;

            LocationComboBox.ItemsSource = Enum.GetValues(typeof(Location)).Cast<Location>();

            if (applier.WatermarkLocation != null)
            {
                PercentageMargin = applier.WatermarkLocation.ImageMarginPercentage;
                PercentageSize = applier.WatermarkLocation.ImageSizePercentage;
                Location = applier.WatermarkLocation.Location;
            }
                
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
            if (string.IsNullOrWhiteSpace(WatermarkImage))
            {
                return;
            }

            var image = new ImageFile(WatermarkImage);

            Uri fileUri = new Uri(image.Path);
            ExampleImage.Source = new BitmapImage(fileUri);

            switch (Location)
            {
                case Location.BottomLeft:
                case Location.Left:
                case Location.TopLeft:
                    ExampleImage.HorizontalAlignment = HorizontalAlignment.Left;
                    break;

                case Location.BottomRight:
                case Location.Right:
                case Location.TopRight:
                    ExampleImage.HorizontalAlignment = HorizontalAlignment.Right;
                    break;

                case Location.Bottom:
                case Location.Top:
                case Location.Middle:
                default:
                    ExampleImage.HorizontalAlignment = HorizontalAlignment.Center;
                    break;
            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files|*.png;*.jpeg;*.jpg;*.gif"
            };

            var result = fileDialog.ShowDialog();

            if (result == true)
            {
                WatermarkImage = fileDialog.FileName;
                FileNameTextBox.Text = WatermarkImage;
                SetUpImage();
                ValueChanged(sender, e);
            }
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            SendOnUpdateEvent(sender, e);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            SendDeletedEvent(sender, e);
        }

        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            SendMoveUpEvent(sender, e);
        }

        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            SendMoveDownEvent(sender, e);
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