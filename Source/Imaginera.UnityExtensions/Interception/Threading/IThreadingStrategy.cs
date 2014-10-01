namespace Imaginera.UnityExtensions.Interception.Threading
{
    using System;

    /// <summary>
    /// Contract for classes which can run code under a threading strategy
    /// </summary>
    public interface IThreadingStrategy
    {
        /// <summary>
        /// Run the requested <see cref="action" /> using a predefined threading strategy
        /// </summary>
        /// <param name="action">
        /// The action to run
        /// </param>
        void Run(Action action);
    }
}
