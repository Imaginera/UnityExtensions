namespace Imaginera.UnityExtensions.Registration
{
    using System.Linq;

    using Microsoft.Practices.EnterpriseLibrary.Logging;
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.InterceptionExtension;

    /// <summary>
    /// The unity interface interception registerer.
    /// </summary>
    public class InterceptionExtension : UnityContainerExtension
    {
        private bool isLoggingEnabled;

        /// <summary>
        /// Initial the container with this extension's functionality.
        /// </summary>
        /// <remarks>
        /// When overridden in a derived class, this method will modify the given
        ///             <see cref="T:Microsoft.Practices.Unity.ExtensionContext"/> by adding strategies, policies, etc. to
        ///             install it's functions into the container.
        /// </remarks>
        protected override void Initialize()
        {
            this.isLoggingEnabled = Extensions.IsLoggingEnabled();

            this.Container.AddNewExtension<Interception>();
            this.Context.Registering += this.OnRegister;
        }

        /// <summary>
        /// Event handler for the ApplicationDbContext.Register callback
        /// </summary>
        /// <param name="sender">
        /// The event sender
        /// </param>
        /// <param name="eventArgs">
        /// The event args
        /// </param>
        private void OnRegister(object sender, RegisterEventArgs eventArgs)
        {
            if (eventArgs != null 
                && eventArgs.TypeTo.GetCustomAttributes(typeof(HandlerAttribute), true).Any())
            {
                if (this.isLoggingEnabled)
                {
                    Logger.Write(string.Format("Configuring {0} for interception", eventArgs.TypeTo));
                }

                this.Container.Configure<Interception>().SetInterceptorFor(
                    eventArgs.TypeTo, eventArgs.Name, new VirtualMethodInterceptor());
            }
        }
    }
}