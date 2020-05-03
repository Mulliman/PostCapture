using ImageTools.Core.Selection;
using System;
using System.CodeDom;
using System.Security.RightsManagement;
using System.Windows.Controls;

namespace ImageTools.Configurator.ApplierForms
{
    public abstract class ApplierFormUserControl : UserControl
    {
        public event EventHandler OnUpdate;
        public event EventHandler OnDelete;
        protected DebounceDispatcher _resizeThrottle = new DebounceDispatcher();

        public abstract ProcessStepConfiguration GetData();

        public Guid ApplierFormInstanceId { get; }

        public ApplierFormUserControl()
        {
            ApplierFormInstanceId = Guid.NewGuid();
        }

        protected void SendOnUpdateEvent(object sender, EventArgs args)
        {
            if(OnUpdate != null)
            {
                _resizeThrottle.Throttle(1000, parm => OnUpdate(sender, args)); 
            }
        }

        protected void SendDeletedEvent(object sender, EventArgs args)
        {
            if (OnDelete != null)
            {
                OnDelete(sender, new FormDeletedEventArgs { ApplierFormInstanceId = this.ApplierFormInstanceId });
            }
        }
    }

    public class FormDeletedEventArgs : EventArgs
    {
        public Guid ApplierFormInstanceId { get; set; }
    }
}
