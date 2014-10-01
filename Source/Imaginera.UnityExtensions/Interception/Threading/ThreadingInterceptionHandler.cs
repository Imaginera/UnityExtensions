namespace Imaginera.UnityExtensions.Interception.Threading
{
    using System;
    using System.Reflection;

    using Microsoft.Practices.Unity.InterceptionExtension;

    /// <summary>
    /// Implementation of <see cref="IInterceptionHandler" /> to apply threading functionality
    /// to the intercepted method call
    /// </summary>
    public class ThreadingInterceptionHandler : IInterceptionHandler
    {
        /// <summary>
        /// Key to store in the invocation context to mark this handler as having been invoked
        /// </summary>
        private const string InvocationContextKey = "ThreadHandled";

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
        public IMethodReturn Intercept(IMethodInvocation input, Func<IMethodReturn> invokeMethod)
        {
            var threadingStrategyAttribute = input.MethodBase.GetCustomAttribute<ThreadingStrategyAttribute>();

            if (!input.InvocationContext.ContainsKey(InvocationContextKey)
                && threadingStrategyAttribute != null
                && input.MethodBase is MethodInfo
                && ((MethodInfo)input.MethodBase).ReturnType == typeof(void))
            {
                input.InvocationContext.Add(InvocationContextKey, true);
                IThreadingStrategy strategy = 
                    threadingStrategyAttribute.ThreadingStrategyMode == ThreadingStrategyMode.Background
                        ? new BackgroundThreadingStrategy() 
                        : threadingStrategyAttribute.ThreadingStrategyMode == ThreadingStrategyMode.Dispatcher
                            ? new DispatcherThreadingStrategy() 
                            : threadingStrategyAttribute.ThreadingStrategyMode == ThreadingStrategyMode.NewBackground
                                ? new NewBackgroundThreadingStrategy()
                                : (IThreadingStrategy) new CallingThreadThreadingStrategy(); 

                if (strategy != null)
                {
                    strategy.Run(() => invokeMethod());
                    return new VirtualMethodReturn(input, null);
                }
            }

            return invokeMethod();
        }
    }
}
