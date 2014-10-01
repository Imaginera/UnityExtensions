namespace Imaginera.UnityExtensions.Interception
{
    using Imaginera.UnityExtensions.Interception.Logging;
    using Imaginera.UnityExtensions.Interception.PropertyNotification;
    using Imaginera.UnityExtensions.Interception.PropertyPersistence;
    using Imaginera.UnityExtensions.Interception.Threading;

    using Microsoft.Practices.Unity.InterceptionExtension;

    /// <summary>
    /// Interception call handler
    /// </summary>
    public class CallHandler : ICallHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CallHandler"/> class.
        /// </summary>
        public CallHandler()
        {
            this.Order = 1;
        }

        /// <summary>
        /// Gets or sets the order in which the handler will be executed
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Implement this method to execute your handler processing.
        /// </summary>
        /// <param name="input">
        /// Inputs to the current call to the target.
        /// </param>
        /// <param name="getNext">
        /// Delegate to execute to get the next delegate in the handler chain.
        /// </param>
        /// <returns>
        /// Return value from the target.
        /// </returns>
        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            return new ThreadingInterceptionHandler().Intercept(
                input,
                () =>
                new LoggingInterceptionHandler().Intercept(
                    input,
                    () =>
                    new PropertyPersistenceInterceptionHandler().Intercept(
                        input,
                        () => new NotifyPropertyInterceptionHandler().Intercept(input, () => getNext()(input, getNext)))));
        }
    }
}
