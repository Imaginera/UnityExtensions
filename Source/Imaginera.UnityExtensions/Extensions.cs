namespace Imaginera.UnityExtensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;

    using Imaginera.UnityExtensions.Factory;
    using Imaginera.UnityExtensions.Interception;
    using Imaginera.UnityExtensions.Interception.PropertyNotification;
    using Imaginera.UnityExtensions.Interception.Threading;
    using Imaginera.UnityExtensions.Registration;

    using Microsoft.Practices.EnterpriseLibrary.Logging;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// Extension methods for <see cref="IUnityContainer" />
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Automatically configures an <see cref="IUnityContainer" />
        /// Dependecies are registered if backed by an interface in an <see cref="Assembly" />
        /// in the <see cref="assemblies" /> collection
        /// Registeded dependencies are also considered for interception.
        /// To prevent a class being registered use the <see cref="ExcludeRegistrationAttribute" />
        /// </summary>
        /// <param name="container">
        /// The container into which to register dependencies
        /// </param>
        /// <param name="assemblies">
        /// The assemblies to scan when looking for dependencies to register
        /// </param>
        public static void AutoConfigure(this IUnityContainer container, IEnumerable<Assembly> assemblies)
        {
            container.AddNewExtension<InterceptionExtension>();
            container.RegisterType(typeof(IObjectFactory<>), typeof(ObjectFactory<>));
            
            var classesToRegister = AllClasses
                .FromAssemblies(assemblies)
                .Where(x => x.GetCustomAttribute<ExcludeRegistrationAttribute>(false) == null);

            var registrationName = new Func<Type, string>(x => x.GetCustomAttribute<NamedRegistrationAttribute>() == null
                ? null
                : x.GetCustomAttribute<NamedRegistrationAttribute>().RegistrationName);

            var exportedTypes = assemblies.SelectMany(a => a.GetExportedTypes());

            var registrationType = new Func<Type, IEnumerable<Type>>(
                /* If the class doesnt have a register as type attribute */
                c => c.GetCustomAttribute<RegisterAsTypeAttribute>() == null
                /* Get all the interfaces it needs to be mapped with where assembly contains a visible type AND doesn't have the exclued registration attribute */
                ? WithMappings.FromAllInterfaces(c).Where(
                    i =>
                        {
                            var hasAssembly = exportedTypes.Contains(i.IsGenericType ? i.GetGenericTypeDefinition() : i);
                            var hasAttribute = i.GetCustomAttribute<ExcludeRegistrationAttribute>(false) != null;

                            if (i.Name.Contains("IS"))
                            {
                                
                            }

                            return hasAssembly && !hasAttribute;
                        })
                /* Otherwise return it's registration type */
                : new[]
                      {
                          c.GetCustomAttribute<RegisterAsTypeAttribute>().RegistrationType
                      });

            container.RegisterTypes(
                classesToRegister,
                registrationType,
                registrationName,
                WithLifetime.Custom<ConfigurableLifetimeManager>);

            // log dependencies mapped in container
            if (IsLoggingEnabled())
            {
                foreach (var registration in container.Registrations)
                {
                    string message = string.Format(
                        "Name:{0} RegisteredType:{1} MappedTo:{2}",
                        registration.Name,
                        registration.RegisteredType,
                        registration.MappedToType);

                    Logger.Write(message);
                }
            }
        }

        /// <summary>
        /// Validates the auto configuration of a container.
        /// Validation includes confirming that dependencies with interception attributes attached
        /// have appropriate access modifiers and are marked as virtual.
        /// Properties with <see cref="NotifyPropertyChangedAttribute" /> attached will be checked for 
        /// implementation of <see cref="INotifyPropertyChanged" />
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static List<string> ValidateConfiguration(this IUnityContainer container)
        {
            bool isLoggingEnabled = IsLoggingEnabled();
            var validationFailures = new List<string>();

            foreach (
                var mappedType in
                    container.Registrations.Select(x => x.MappedToType)
                        .Distinct()
                        .Where(x => x.GetCustomAttribute<InterceptClassAttribute>(false) == null))
            {
                // if there are any methods that have this intercept method attribute on then we have an issue
                if (
                    !(mappedType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                          .Any(x => x.GetCustomAttributes<InterceptMethodAttribute>(false).Any())
                      || mappedType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                             .Any(x => x.GetCustomAttributes<InterceptMethodAttribute>(false).Any())))
                {
                    continue;
                }
                
                var message =
                    string.Format(
                        "Class {0} has intercept methods but no InterceptClass attribute defined.",
                        mappedType.Name);

                validationFailures.Add(message);

                if (isLoggingEnabled)
                {
                    Logger.Write(message);
                }
            }

            foreach (var mappedType in container.Registrations.Select(x => x.MappedToType)
                .Distinct()
                .Where(x => x.GetCustomAttributes<InterceptClassAttribute>(true).Any()))
            {
                // private methods
                foreach (var member in mappedType
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.GetCustomAttributes<InterceptMethodAttribute>(true).Any())
                    .Where(x => x.IsPrivate))
                {
                    string message = string.Format(
                        "Private method {0} in type {1} will not be intercepted. Change method to be at least protected",
                        member.Name,
                        mappedType);

                    validationFailures.Add(message);

                    if (isLoggingEnabled)
                    {
                        Logger.Write(message);
                    }
                }

                // virtual methods
                foreach (var member in mappedType
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.GetCustomAttributes<InterceptMethodAttribute>(true).Any())
                    .Where(x => !x.IsVirtual))
                {
                    string message = string.Format(
                        "Non virtual method {0} in type {1} will not be intercepted. Mark method as virtual",
                        member.Name,
                        mappedType);

                    validationFailures.Add(message);

                    if (isLoggingEnabled)
                    {
                        Logger.Write(message);
                    }
                }

                // private properties
                foreach (var member in mappedType
                    .GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.GetCustomAttributes<InterceptMethodAttribute>(true).Any())
                    .Where(x => x.GetMethod.IsPrivate))
                {
                    string message = string.Format(
                        "Private property {0} in type {1} will not be intercepted. Change property to be at least protected", 
                        member.Name,
                        mappedType);

                    validationFailures.Add(message);

                    if (isLoggingEnabled)
                    {
                        Logger.Write(message);
                    }
                }

                // virtual properties
                // Read: http://msdn.microsoft.com/en-us/library/system.reflection.methodbase.isvirtual(v=vs.110).aspx
                // It details that IsFinal needs to be checked as well to see if something is overridable.
                foreach (var member in mappedType
                                        .GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                                        .Where(x => x.GetCustomAttributes<InterceptMethodAttribute>(true).Any())
                                        .Where(x => x.GetGetMethod() != null)
                                        .Where(x => !(x.GetGetMethod().IsVirtual && !x.GetGetMethod().IsFinal)))
                {
                    string message = string.Format(
                        "Non virtual property {0} in type {1} will not be intercepted. Mark property as virtual",
                        member.Name,
                        mappedType);    

                    validationFailures.Add(message);

                    if (isLoggingEnabled)
                    {
                        Logger.Write(message);
                    }
                }

                // INotifyProperty no interface
                if (!mappedType.GetInterfaces().Contains(typeof(INotifyPropertyChanged)))
                { 
                    foreach (var member in mappedType
                        .GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                        .Where(x => x.GetCustomAttributes<NotifyPropertyChangedAttribute>(true).Any()))
                    {
                        string message = string.Format(
                            "NotifyPropertyChanged used on {0} in type {1} which does not implement INotifyPropertyChanged",
                            member.Name,
                            mappedType);

                        validationFailures.Add(message);

                        if (isLoggingEnabled)
                        {
                            Logger.Write(message);
                        }
                    }
                }

                // INotifyProperty has no setter
                foreach (var member in mappedType
                    .GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.GetCustomAttributes<NotifyPropertyChangedAttribute>(true).Any())
                    .Where(x => x.SetMethod == null))
                {
                    string message =
                        string.Format(
                            "Property {0} in type {1} will not raise property changed notifications. Change property to at least have a setter",
                            member.Name,
                            mappedType);

                    validationFailures.Add(message);

                    if (isLoggingEnabled)
                    {
                        Logger.Write(message);
                    }
                }

                // INotifyProperty private setter
                foreach (var member in mappedType
                    .GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.GetCustomAttributes<NotifyPropertyChangedAttribute>(true).Any())
                    .Where(x => x.SetMethod != null && x.SetMethod.IsPrivate))
                {
                    string message = string.Format(
                       "Private property {0} in type {1} will not raise property changed notifications. Change property to be at least protected", 
                        member.Name,
                        mappedType);

                    validationFailures.Add(message);

                    if (isLoggingEnabled)
                    {
                        Logger.Write(message);
                    }
                }

                // making sure threading strategy is applied to voids only
                foreach (var member in mappedType
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.GetCustomAttributes<ThreadingStrategyAttribute>(true).Any())
                    .Where(x => x.ReturnType != typeof(void)))
                {
                    string message =
                        string.Format(
                            "Method {0} in type {1} will not return any object. Change method to return type of Void",
                            member.Name,
                            mappedType);

                    validationFailures.Add(message);

                    if (isLoggingEnabled)
                    {
                        Logger.Write(message);
                    }
                }
            }

            return validationFailures;
        }

        public static bool IsLoggingEnabled()
        {
            try
            {
                return Logger.IsLoggingEnabled();
            }
            catch
            {
                return false;
            }
        }
    }
}