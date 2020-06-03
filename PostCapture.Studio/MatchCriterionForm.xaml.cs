using PostCapture.Core.Selection;
using PostCapture.Studio.ApplierForms;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace PostCapture.Studio
{
    /// <summary>
    /// Interaction logic for MatchCriterionForm.xaml
    /// </summary>
    public partial class MatchCriterionForm : UserControl
    {
        public event EventHandler OnDelete;

        public Guid InstanceId { get; set; }

        public MatchCriterionForm()
        {
            InitializeComponent();

            InstanceId = Guid.NewGuid();
        }

        public MatchCriterionForm(ExtraMatchCriterion criterion)
        {
            InitializeComponent();

            InstanceId = Guid.NewGuid();
            MatchPropertyTextBox.Text = criterion.MatchProperty;
            MatchValueTextBox.Text = criterion.MatchValue;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (OnDelete != null)
            {
                OnDelete(sender, new FormDeletedEventArgs { ApplierFormInstanceId = this.InstanceId });
            }
        }

        public ExtraMatchCriterion GetData()
        {
            if(string.IsNullOrWhiteSpace(MatchPropertyTextBox.Text))
            {
                return null;
            }

            return new ExtraMatchCriterion
            {
                MatchProperty = MatchPropertyTextBox.Text,
                MatchValue = MatchValueTextBox.Text
            };
        }
    }
}