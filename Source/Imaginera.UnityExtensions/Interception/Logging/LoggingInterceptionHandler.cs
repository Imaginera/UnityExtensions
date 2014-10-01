namespace Imaginera.UnityExtensions.Interception.Logging
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    using Microsoft.Practices.EnterpriseLibrary.Logging;
    using Microsoft.Practices.Unity.InterceptionExtension;

    /// <summary>
    /// Implementation of <see cref="IInterceptionHandler" /> for logging
    /// </summary>
    public class LoggingInterceptionHandler : IInterceptionHandler
    {
        /// <summary>
        /// Key to store in the invocation context to mark this handler as having been invoked
        /// </summary>
        private const string InvocationContextKey = "LoggingHandled";

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
            var logAttribute = input.MethodBase.GetCustomAttribute<LogAttribute>();

            if (logAttribute == null
                && input.MethodBase.IsSpecialName
                && input.MethodBase.Name.Length > 4)
            {
                string propertyName = input.MethodBase.Name.Substring(4);
                PropertyInfo property = input.Target.GetType().GetProperty(propertyName);

                if (property != null)
                {
                    logAttribute = property.GetCustomAttribute<LogAttribute>();
                }
            }

            if (logAttribute != null
                && !input.InvocationContext.ContainsKey(InvocationContextKey))
            {
                Debug.Assert(Logger.IsLoggingEnabled(), "No logger is configured, but log attributes are in use");
                input.InvocationContext.Add(InvocationContextKey, true);

                var message = string.Format(
                        "{0}.{1} - Called with parameters [{2}]",
                        input.MethodBase.ReflectedType == null ? string.Empty : input.MethodBase.ReflectedType.Name,
                        input.MethodBase.Name,
                        string.Join(",", input.Inputs.Cast<object>()));

                if (Logger.IsLoggingEnabled())
                {
                    Logger.Write(
                        message,
                        logAttribute.Category,
                        logAttribute.Priority,
                        logAttribute.EventId,
                        logAttribute.Severity == 0 ? TraceEventType.Verbose : logAttribute.Severity);
                }

                IMethodReturn methodReturn;

                if (logAttribute.TimeMethodExecution)
                {
                    var stopwatch = Stopwatch.StartNew();
                    methodReturn = invokeMethod();
                    stopwatch.Stop();

                    message = string.Format(
                        "{0}.{1} - Complete in [{2:N0}ms] and returned [{3}]",
                        input.MethodBase.ReflectedType == null ? string.Empty : input.MethodBase.ReflectedType.Name,
                        input.MethodBase.Name,
                        stopwatch.ElapsedMilliseconds,
                        methodReturn.ReturnValue ?? "void");

                    if (Logger.IsLoggingEnabled())
                    {
                        Logger.Write(
                            message,
                            logAttribute.Category,
                            logAttribute.Priority,
                            logAttribute.EventId,
                            logAttribute.Severity == 0 ? TraceEventType.Verbose : logAttribute.Severity);
                    }
                }
                else
                {
                    methodReturn = invokeMethod();
                }

                return methodReturn;
            }

            return invokeMethod();
        }
    }
}
