using PostCapture.Core.Selection;
using System;
using System.CodeDom;
using System.Security.RightsManagement;
using System.Windows.Controls;

namespace PostCapture.Studio.ApplierForms
{
    public abstract class ApplierFormUserControl : UserControl
    {
        public event EventHandler OnUpdate;
        public event EventHandler OnDelete;
        public event EventHandler OnMove;

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

        protected void SendMoveUpEvent(object sender, EventArgs args)
        {
            if (OnMove != null)
            {
                OnMove(sender, new FormMovedEventArgs 
                { 
                    ApplierFormInstanceId = this.ApplierFormInstanceId,
                    Direction = MoveDirection.Up
                });
            }
        }

        protected void SendMoveDownEvent(object sender, EventArgs args)
        {
            if (OnMove != null)
            {
                OnMove(sender, new FormMovedEventArgs 
                { 
                    ApplierFormInstanceId = this.ApplierFormInstanceId,
                    Direction = MoveDirection.Down
                });
            }
        }
    }

    public class FormDeletedEventArgs : EventArgs
    {
        public Guid ApplierFormInstanceId { get; set; }
    }

    public class FormMovedEventArgs : EventArgs
    {
        public Guid ApplierFormInstanceId { get; set; }

        public MoveDirection Direction { get; set; }
    }

    public enum MoveDirection
    {
        Up,
        Down
    }
}
