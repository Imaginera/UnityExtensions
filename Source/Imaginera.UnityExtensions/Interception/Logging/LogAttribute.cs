namespace Imaginera.UnityExtensions.Interception.Logging
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Attribute indicating that the adorned property or method should be intercepted for logging
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class LogAttribute : InterceptMethodAttribute
    {
        public LogAttribute()
        {
            this.TimeMethodExecution = true;
        }

        public string Category { get; set; }

        public int Priority { get; set; }

        public int EventId { get; set; }

        public bool TimeMethodExecution { get; set; }

        public TraceEventType Severity { get; set; } 
    }
}
