using System.Windows.Controls;

namespace ImageTools.Configurator.ApplierForms
{
    public interface IApplierFormBuilder
    {
        public abstract UserControl Build(System.Collections.Generic.Dictionary<string, string> parameters);
    }
}