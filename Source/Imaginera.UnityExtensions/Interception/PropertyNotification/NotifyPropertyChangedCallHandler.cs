namespace Imaginera.UnityExtensions.Interception.PropertyNotification
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Windows;

    using Microsoft.Practices.EnterpriseLibrary.Common.Utility;
    using Microsoft.Practices.Unity.InterceptionExtension;

    /// <summary>
    /// Implementation of <see cref="ICallHandler" /> dealing with the invocation of <see cref="INotifyPropertyChanged" /> events
    /// </summary>
    public class NotifyPropertyChangedCallHandler : ICallHandler
    {
        /// <summary>
        /// Gets or sets the order in which the handler will be executed
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Implement this method to execute your handler processing.
        /// </summary>
        /// <param name="input">Inputs to the current call to the target.</param><param name="getNext">Delegate to execute to get the next delegate in the handler
        ///             chain.</param>
        /// <returns>
        /// Return value from the target.
        /// </returns>
        // [DebuggerStepThrough]
        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            MethodBase method = input.MethodBase;
            IMethodReturn result = getNext.Invoke().Invoke(input, getNext);

            if (method.DeclaringType != null
                && method.DeclaringType.GetInterfaces().Contains(typeof(INotifyPropertyChanged))
                && method.IsSpecialName
                && method.Name.StartsWith("set_"))
            {
                string propertyName = method.Name.Substring(4);
                PropertyInfo property = input.Target.GetType().GetProperty(propertyName);

                if (property != null && property.GetCustomAttribute<NotifyPropertyChangedAttribute>() != null)
                {
                    var type = input.Target.GetType();

                    while (type != null && !type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                        .Select(x => x.Name)
                        .Contains("PropertyChanged"))
                    {
                        type = type.BaseType;
                    }

                    if (type != null)
                    {
                        FieldInfo fieldInfo = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                            .First(x => x.Name == "PropertyChanged");

                        var multicaseDelegate = (MulticastDelegate)fieldInfo.GetValue(input.Target);

                        if (multicaseDelegate != null)
                        {
                            if (Application.Current.CheckAccess())
                            {
                                multicaseDelegate.GetInvocationList()
                                    .Cast<PropertyChangedEventHandler>()
                                    .ForEach(x => x.Invoke(input.Target, new PropertyChangedEventArgs(propertyName)));
                            }
                            else
                            {
                                Application.Current.Dispatcher.Invoke(() => multicaseDelegate
                                    .GetInvocationList()
                                    .Cast<PropertyChangedEventHandler>()
                                    .ForEach(x => x.Invoke(input.Target, new PropertyChangedEventArgs(propertyName))));
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}