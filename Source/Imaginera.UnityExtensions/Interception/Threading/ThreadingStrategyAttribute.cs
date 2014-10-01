namespace Imaginera.UnityExtensions.Interception.Threading
{
    using System;

    /// <summary>
    /// The threading strategy attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ThreadingStrategyAttribute : InterceptMethodAttribute
    {
        /// <summary>
        /// Gets or sets the threading strategy on which to run the code
        /// </summary>
        public ThreadingStrategyMode ThreadingStrategyMode { get; set; }
    }
}
