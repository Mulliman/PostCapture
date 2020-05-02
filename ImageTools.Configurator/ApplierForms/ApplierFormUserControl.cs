using ImageTools.Core.Selection;
using System;
using System.Security.RightsManagement;
using System.Windows.Controls;

namespace ImageTools.Configurator.ApplierForms
{
    public abstract class ApplierFormUserControl : UserControl
    {
        public event EventHandler OnUpdate;
        protected DebounceDispatcher _resizeThrottle = new DebounceDispatcher();

        public abstract ProcessStepConfiguration GetData();

        protected void SendOnUpdateEvent(object sender, EventArgs args)
        {
            if(OnUpdate != null)
            {
                _resizeThrottle.Throttle(1000, parm => OnUpdate(sender, args)); 
            }
        }
    }
}
