namespace Imaginera.UnityExtensions.Interception.Threading
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// Implementation of <see cref="IThreadingStrategy" /> which determines
    /// if the calling thread is the GUI thread by calling CheckAccess on <see cref="Application.Current" />
    /// If the calling thread is the GUI thread then the action is invoked onto a <see cref="Task" />
    /// </summary>
    public class BackgroundThreadingStrategy : IThreadingStrategy
    {
        /// <summary>
        /// Run the requested <see cref="action" /> using a predefined threading strategy
        /// </summary>
        /// <param name="action">
        /// The action to run
        /// </param>
        public void Run(Action action)
        {
            if (Application.Current != null && Application.Current.CheckAccess())
            {
                Task.Run(action);
            }
            else
            {
                action();
            }
        }
    }
}