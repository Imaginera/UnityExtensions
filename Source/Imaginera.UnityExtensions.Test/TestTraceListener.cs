namespace Imaginera.UnityExtensions.Test
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;

    using Microsoft.Practices.EnterpriseLibrary.Logging;
    using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;

    public class TestTraceListener : CustomTraceListener
    {
        public TestTraceListener()
        {
            this.Messages = new List<string>();
            this.WaitHandle = new AutoResetEvent(false);
        }

        public WaitHandle WaitHandle { get; private set; }

        public List<string> Messages { get; private set; }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            if (data is LogEntry && this.Formatter != null)
            {
                this.WriteLine(this.Formatter.Format(data as LogEntry));
            }
            else
            {
                this.WriteLine(data.ToString());
            }
        }

        public override void Write(string message)
        {
            ((AutoResetEvent)this.WaitHandle).Set();
        }

        public override void WriteLine(string message)
        {
            this.Messages.Add(message);
            ((AutoResetEvent)this.WaitHandle).Set();
        }
    }
}