using System;

namespace PostCapture.Studio.ApplierForms
{
    public class FormDeletedEventArgs : EventArgs
    {
        public Guid ApplierFormInstanceId { get; set; }
    }
}
