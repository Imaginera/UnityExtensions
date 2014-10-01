namespace Imaginera.UnityExtensions.Interception.PropertyNotification
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows;

    using Microsoft.Practices.EnterpriseLibrary.Common.Utility;
    using Microsoft.Practices.Unity.InterceptionExtension;

    /// <summary>
    /// Implementation of <see cref="IInterceptionHandler" /> for handling property notification
    /// </summary>
    public class NotifyPropertyInterceptionHandler : IInterceptionHandler
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
        public IMethodReturn Intercept(IMethodInvocation input, Func<IMethodReturn> invokeMethod)
        {
            IMethodReturn result = null;
            
            if (!input.InvocationContext.ContainsKey("NotificationHandled")
                && input.MethodBase.DeclaringType != null
                && input.MethodBase.DeclaringType.GetInterfaces().Contains(typeof(INotifyPropertyChanged))
                && input.MethodBase.IsSpecialName
                && input.MethodBase.Name.StartsWith("set_"))
            {
                input.InvocationContext.Add("NotificationHandled", true);

                string propertyName = input.MethodBase.Name.Substring(4);
                PropertyInfo property = input.Target.GetType().GetProperty(propertyName);

                if (property != null && property.GetCustomAttribute<NotifyPropertyChangedAttribute>() != null)
                {
                    var initialValue = property.GetValue(input.Target);
                    result = invokeMethod();
                    var currentValue = property.GetValue(input.Target);

                    if (!Equals(initialValue, currentValue))
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
                                if (Application.Current == null || Application.Current.CheckAccess())
                                {
                                    multicaseDelegate.GetInvocationList()
                                        .Cast<PropertyChangedEventHandler>()
                                        .ForEach(x => x.Invoke(input.Target, new PropertyChangedEventArgs(propertyName)));
                                }
                                else
                                {
                                    Application.Current.Dispatcher.Invoke(() =>
                                        multicaseDelegate.GetInvocationList()
                                            .Cast<PropertyChangedEventHandler>()
                                            .ForEach(x => x.Invoke(input.Target, new PropertyChangedEventArgs(propertyName))));
                                }
                            }
                        }
                    }
                }
                else
                {
                    result = invokeMethod();
                }
            }
            else
            {
                result = invokeMethod();
            }

            return result;
        }
    }
}
