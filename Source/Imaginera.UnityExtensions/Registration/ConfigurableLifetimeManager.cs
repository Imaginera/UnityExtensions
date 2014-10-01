namespace Imaginera.UnityExtensions.Registration
{
    using System;
    using System.Deployment.Application;
    using System.Diagnostics;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    using Imaginera.UnityExtensions.Interception.PropertyPersistence;

    using Microsoft.Practices.Unity;

    using Newtonsoft.Json;

    /// <summary>
    /// The configurable lifetime manager.
    /// </summary>
    public class ConfigurableLifetimeManager : LifetimeManager
    {
        /// <summary>
        /// Lock to ensure only one lifetime manager is created
        /// </summary>
        private readonly object sync;

        /// <summary>
        /// The lifetime manager.
        /// </summary>
        private LifetimeManager lifetimeManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurableLifetimeManager"/> class.
        /// </summary>
        public ConfigurableLifetimeManager()
        {
            this.sync = new object();
        }

        /// <summary>
        /// Retrieve a value from the backing store associated with this Lifetime policy.
        /// </summary>
        /// <returns>
        /// the object desired, or null if no such object is currently stored.
        /// </returns>
        public override object GetValue()
        {
            return this.lifetimeManager == null
                ? null
                : this.lifetimeManager.GetValue();
        }

        /// <summary>
        /// Stores the given value into backing store for retrieval later.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        public override void SetValue(object newValue)
        {
            lock (this.sync)
            {
                if (this.lifetimeManager == null)
                {
                    var attribute = newValue
                        .GetType()
                        .GetCustomAttributes(typeof(LifetimeManagerAttribute), true)
                        .FirstOrDefault() as LifetimeManagerAttribute;

                    this.lifetimeManager = attribute != null && attribute.Type == LifetimeManagerType.PerResolve
                        ? (LifetimeManager)new PerResolveLifetimeManager()
                        : new ContainerControlledLifetimeManager();
                }
            }

            this.lifetimeManager.SetValue(newValue);

            foreach (var propertyInfo in newValue.GetType()
                    .GetProperties()
                    .Where(x => x.GetCustomAttribute<PersistedPropertyAttribute>() != null))
            {
                string id = propertyInfo.GetCustomAttribute<PersistedPropertyAttribute>().Id;

                try
                {
                    Thread.SetData(Thread.GetNamedDataSlot(id), true);

                    bool valueSet = false;

                    using (var store = ApplicationDeployment.IsNetworkDeployed
                        ? IsolatedStorageFile.GetUserStoreForApplication()
                        : IsolatedStorageFile.GetUserStoreForAssembly())
                    {
                        if (store.FileExists(id))
                        {
                            using (var stream = new IsolatedStorageFileStream(id, FileMode.Open, store))
                            using (var reader = new StreamReader(stream))
                            {
                                string text = reader.ReadLine();
                                propertyInfo.SetValue(newValue, JsonConvert.DeserializeObject(text, propertyInfo.PropertyType));
                                valueSet = true;
                            }
                        }
                        else
                        {
                            var defaultProvider = newValue as IPersistedValueDefaultValueProvider;

                            if (defaultProvider != null)
                            {
                                propertyInfo.SetValue(newValue, defaultProvider.GetDefaultValue(propertyInfo.Name));
                                valueSet = true;
                            }
                        }
                    }

                    if (valueSet)
                    {
                        var notifier = newValue as IPersistedValueSetNotifier;

                        if (notifier != null)
                        {
                            notifier.PropertySet(propertyInfo.Name);
                        }
                    }
                }
                catch (Exception exception)
                {
                    Debug.WriteLine("Could not set default value for persisted property: " + exception);
                }
                finally
                {
                    Thread.SetData(Thread.GetNamedDataSlot(id), false);
                }
            }
        }

        /// <summary>
        /// Remove the given object from backing store.
        /// </summary>
        public override void RemoveValue()
        {
            if (this.lifetimeManager != null)
            {
                this.lifetimeManager.RemoveValue();
            }
        }
    }
}