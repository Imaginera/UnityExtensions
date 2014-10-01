namespace Imaginera.UnityExtensions.Interception.Threading
{
    using System;

    /// <summary>
    /// Implementation of <see cref="IThreadingStrategy" /> which runs the requested <see cref="Action" />
    /// on the calling thread
    /// </summary>
    public class CallingThreadThreadingStrategy : IThreadingStrategy
    {
        /// <summary>
        /// Run the requested <see cref="action" /> using a predefined threading strategy
        /// </summary>
        /// <param name="action">
        /// The action to run
        /// </param>
        public void Run(Action action)
        {
            action();
        }
    }
}