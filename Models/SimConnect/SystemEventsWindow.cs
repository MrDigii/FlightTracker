using System;
using System.Windows.Forms;

namespace FlightTracker.Models.SimConnect
{
    /// <summary>
    /// Window class to catch system events
    /// </summary>
    public sealed class SystemEventsWindow: NativeWindow
    {
        public event EventHandler<Message> WndProcCalled;

        public SystemEventsWindow()
        {
            CreateHandle(new CreateParams());
        }

        protected override void WndProc(ref Message m)
        {
            WndProcCalled?.Invoke(this, m);
            base.WndProc(ref m);
        }
    }
}
