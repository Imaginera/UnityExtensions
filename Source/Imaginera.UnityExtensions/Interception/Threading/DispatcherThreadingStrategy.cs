namespace Imaginera.UnityExtensions.Interception.Threading
{
    using System;
    using System.Windows;

    /// <summary>
    /// Implementation of <see cref="IThreadingStrategy" /> which runs the requested <see cref="Action" />
    /// on the GUI thread associated to <see cref="Application.Current" />
    /// If the application is not running in a GUI context, the <see cref="Action" /> is invoked on the calling thread
    /// </summary>
    public class DispatcherThreadingStrategy : IThreadingStrategy
    {
        /// <summary>
        /// Run the requested <see cref="action" /> using a predefined threading strategy
        /// </summary>
        /// <param name="action">
        /// The action to run
        /// </param>
        public void Run(Action action)
        {
            if (Application.Current != null && !Application.Current.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(action);
            }
            else
            {
                action();
            }
        }
    }
}