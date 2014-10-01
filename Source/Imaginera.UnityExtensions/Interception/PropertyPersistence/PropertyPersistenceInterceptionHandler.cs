namespace Imaginera.UnityExtensions.Interception.PropertyPersistence
{
    using System;
    using System.Deployment.Application;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Reflection;
    using System.Threading;

    using Microsoft.Practices.Unity.InterceptionExtension;

    using Newtonsoft.Json;

    /// <summary>
    /// Implementation of <see cref="IInterceptionHandler" /> proviing property persistence
    /// </summary>
    public class PropertyPersistenceInterceptionHandler : IInterceptionHandler
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
            IMethodReturn result = invokeMethod();

            if (!input.InvocationContext.ContainsKey("PersistenceHandled")
                && input.MethodBase.DeclaringType != null
                && input.MethodBase.IsSpecialName
                && input.MethodBase.Name.StartsWith("set_"))
            {
                input.InvocationContext.Add("PersistenceHandled", true);

                string propertyName = input.MethodBase.Name.Substring(4);
                PropertyInfo property = input.Target.GetType().GetProperty(propertyName);

                if (property != null && property.GetCustomAttribute<PersistedPropertyAttribute>() != null)
                {
                    var attribute = property.GetCustomAttribute<PersistedPropertyAttribute>();
                    string id = attribute.Id;

                    if (!(Thread.GetData(Thread.GetNamedDataSlot(id)) as bool?).GetValueOrDefault())
                    {
                        using (var store = ApplicationDeployment.IsNetworkDeployed ? IsolatedStorageFile.GetUserStoreForApplication() : IsolatedStorageFile.GetUserStoreForAssembly())
                        using (var stream = new IsolatedStorageFileStream(id, FileMode.Create, store))
                        using (var writer = new StreamWriter(stream))
                        {
                            object value = property.GetValue(input.Target);
                            writer.WriteLine(JsonConvert.SerializeObject(value));
                        }
                    }
                }
            }

            return result;
        }
    }
}
