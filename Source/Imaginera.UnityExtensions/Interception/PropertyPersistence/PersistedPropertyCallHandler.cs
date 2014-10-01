namespace Imaginera.UnityExtensions.Interception.PropertyPersistence
{
    using System.ComponentModel;
    using System.Deployment.Application;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Reflection;
    using System.Threading;

    using Microsoft.Practices.Unity.InterceptionExtension;

    using Newtonsoft.Json;

    /// <summary>
    /// Implementation of <see cref="ICallHandler" /> dealing with the invocation of <see cref="INotifyPropertyChanged" /> events
    /// </summary>
    public class PersistedPropertyCallHandler : ICallHandler
    {
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
        /// Delegate to execute to get the next delegate in the handlerchain.
        /// </param>
        /// <returns>
        /// Return value from the target.
        /// </returns>
        /// [DebuggerStepThrough]
        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            MethodBase method = input.MethodBase;
            IMethodReturn result = getNext.Invoke().Invoke(input, getNext);

            if (method.DeclaringType != null
                && method.IsSpecialName
                && method.Name.StartsWith("set_"))
            {
                string propertyName = method.Name.Substring(4);
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