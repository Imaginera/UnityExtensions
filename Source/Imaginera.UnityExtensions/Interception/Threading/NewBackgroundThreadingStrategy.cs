namespace Imaginera.UnityExtensions.Interception.Threading
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Implementation of <see cref="IThreadingStrategy" /> which runs the requested <see cref="Action" />
    /// on a new <see cref="Task" />
    /// </summary>
    public class NewBackgroundThreadingStrategy : IThreadingStrategy
    {
        /// <summary>
        /// Run the requested <see cref="action" /> using a predefined threading strategy
        /// </summary>
        /// <param name="action">
        /// The action to run
        /// </param>
        public void Run(Action action)
        {
            Task.Run(action);
        }
    }
}