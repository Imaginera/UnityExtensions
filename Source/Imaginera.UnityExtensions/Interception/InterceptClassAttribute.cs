namespace Imaginera.UnityExtensions.Interception
{
    using System;

    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.InterceptionExtension;

    /// <summary>
    /// Base class for inceptor attributes
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class InterceptClassAttribute : HandlerAttribute
    {
        /// <summary>
        /// Derived classes implement this method. When called, it
        ///             creates a new call handler as specified in the attribute
        ///             configuration.
        /// </summary>
        /// <param name="container">The <see cref="T:Microsoft.Practices.Unity.IUnityContainer"/> to use when creating handlers,
        ///             if necessary.</param>
        /// <returns>
        /// A new call handler object.
        /// </returns>
        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return container.Resolve<CallHandler>();
        }
    }
}
