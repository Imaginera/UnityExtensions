namespace Imaginera.UnityExtensions.Example
{
    using System;
    using System.Diagnostics;

    using Microsoft.Practices.EnterpriseLibrary.Logging;
    using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;

    public class DebugTraceListener : CustomTraceListener
    {
        /// <summary>
        /// Writes trace information, a data object and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">
        /// A <see cref="T:System.Diagnostics.TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.
        /// </param>
        /// <param name="source">
        /// A name used to identify the output, typically the name of the application that generated the trace event.
        /// </param>
        /// <param name="eventType">
        /// One of the <see cref="T:System.Diagnostics.TraceEventType"/> values specifying the type of event that has caused the trace.
        /// </param>
        /// <param name="id">
        /// A numeric identifier for the event.
        /// </param>
        /// <param name="data">
        /// The trace data to emit.
        /// </param>
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

        /// <summary>
        /// When overridden in a derived class, writes the specified message to the listener you create in the derived class.
        /// </summary>
        /// <param name="message">
        /// A message to write. 
        /// </param>
        public override void Write(string message)
        {
            Debug.Write(message);
        }

        /// <summary>
        /// When overridden in a derived class, writes a message to the listener you create in the derived class, followed by a line terminator.
        /// </summary>
        /// <param name="message">
        /// A message to write. 
        /// </param>
        public override void WriteLine(string message)
        {
            this.Write(message, Environment.NewLine);
        }
    }
}