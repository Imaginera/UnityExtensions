namespace Imaginera.UnityExtensions.Interception
{
    using System;

    using Microsoft.Practices.Unity.InterceptionExtension;

    /// <summary>
    /// Contract for handling interception
    /// A derived class will be added to the interceptio pipeline and have the ability to 
    /// apply adorning functionality to the underlying method call
    /// </summary>
    public interface IInterceptionHandler
    {
        /// <summary>
        /// Called by <see cref="CallHandler" /> to delegate responsibilty for
        /// applying intercepted functionality to a method or property call
        /// </summary>
        /// <param name="input">
        /// The Information about the method or property being intercepted
        /// </param>
        /// <param name="invokeMethod">
        /// The next method in the pipeline to call after the intercepted funcionality has been applied
        /// </param>
        /// <returns>
        /// An implementation of <see cref="IMethodReturn" /> describing information about the result
        /// of invoking <see cref="invokeMethod"/>
        /// </returns>
        IMethodReturn Intercept(IMethodInvocation input, Func<IMethodReturn> invokeMethod);
    }
}
