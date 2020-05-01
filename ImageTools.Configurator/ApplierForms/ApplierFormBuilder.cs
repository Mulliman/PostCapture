using System.Windows.Controls;

namespace ImageTools.Configurator.ApplierForms
{
    public interface IApplierFormBuilder
    {
        public abstract ApplierFormUserControl Build(System.Collections.Generic.Dictionary<string, string> parameters);
    }
}