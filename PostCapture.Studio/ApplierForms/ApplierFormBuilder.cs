using System.Windows.Controls;

namespace PostCapture.Studio.ApplierForms
{
    public interface IApplierFormBuilder
    {
        public abstract ApplierFormUserControl Build(System.Collections.Generic.Dictionary<string, string> parameters);
    }
}