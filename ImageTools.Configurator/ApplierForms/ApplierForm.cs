using ImageTools.Core;
using ImageTools.Core.Selection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace ImageTools.Configurator.ApplierForms
{
    public interface IApplierForm<T> where T : IApplier
    {
        //ProcessStepConfiguration GetData();
    }
}