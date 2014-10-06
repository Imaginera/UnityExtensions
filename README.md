UnityExtensions
===============

AOP Extensions for Unity

## Extensions Setup

To utilise the `AutoConfigure` extension, call the method passing in the project assemblies. Before this is done the logging application block is configured.

```csharp
var configurationSource = ConfigurationSourceFactory.Create();
var logWriterFactory = new LogWriterFactory(configurationSource);
Microsoft.Practices.EnterpriseLibrary.Logging.Logger.SetLogWriter(logWriterFactory.Create());

// Use the Unity extensions AutoConfigure to map all the types
this.Container.AutoConfigure(this.assemblies);

foreach (var error in this.Container.ValidateConfiguration())
{
    Debug.Assert(false, error);
}
```

## Configuring Registrations

It is possible to instruct Unity on how to manage your object by using the `LifeTimeManager` attribute on a class:
```csharp
namespace NewApp
{
    [LifetimeManager(Type = LifetimeManagerType.PerResolve]
    public class SomeViewModel : ISomeViewModel
    {
    }
}
```
Unity can be instructed not to register a class using the `ExcludeRegistration` attribute:
```csharp
namespace NewApp.DesignTime
{
    [ExcludeRegistration]
    public class SomeViewModel : ISomeViewModel
    {
    }
}
```
If you have many concrete types inheriting from the same interface, you can name these registrations to let Unity know which concrete type to `Resolve` using the `NameRegistration`.
```csharp
namespace NewApp.DesignTime
{
    [NamedRegistration(RegistrationName = “SomeViewModel”)]
    public class SomeViewModel : ISomeViewModel
    {
    }
}
```
The Unity extensions allow you to register a type to another specified type by using the `RegisterAsType` attribute.
```csharp
namespace NewApp.Views
{
    [NamedRegistration(RegistrationName = “NewAppMainView”)]
    [RegisterAsType(RegistrationType = typeof(object))]
    public partial class NewAppMainView
    {
    }
}
```
## Setting up Interception
To setup a class to be intercepted it needs to be decorated with the `InterceptClass` attribute:
```csharp
namespace NewApp
{
    [InterceptClass]
    public class SomeViewModel : ISomeViewModel
    {
    }
}
```
In order for interception to work, methods and properties that use these attributes need to `overridable`. Therefore, they need to be marked with `virtual` or `protected virtual`.
## Applying Intercept Attributes
### NotifyPropertyChanged
The `NotifyPropertyChanged` attribute is a simpler way to apply the `PropertyChanged` event to a property.

```csharp
namespace NewApp
{
    [InterceptClass]
    public class SomeViewModel : ISomeViewModel, INotifyPropertyChanged
    {
         public event PropertyChangedEventHandler PropertyChanged;
 
         [NotifyPropertyChanged]
         public virtual string Text { get; set; }
    }
}
```
### Log
The `Log` attribute will output method usage using the specified logger
```csharp
namespace NewApp
{
    [InterceptClass]
    public class SomeViewModel : ISomeViewModel
    {
         [Log(Priority = 2, Severity = TraceEventType.Information, Category = "ViewModels")]
         public virtual void ApplyLogic()
         {
            // method logic.
         }
    }
}
```
### PersistedProperty
```csharp
namespace NewApp
{
    [InterceptClass]
    public class SomeViewModel : ISomeViewModel
    {
         [PersistedProperty("F98B4D43-AA8F-44D4-B0FA-482ADC2A6611")]
         public virtual string Text { get; set; }
    }
}
```
### ThreadingStrategy
The `ThreadingStrategy` will place the method on the appropriate thread to be executed. Threaded methods should only return the type `void`.
```csharp
namespace NewApp
{
    [InterceptClass]
    public class SomeViewModel : ISomeViewModel
    {
        [ThreadingStrategy(ThreadingStrategyMode = ThreadingStrategyMode.Background)]
        public virtual void DoTaskInBackground()
        {
            
        }

        [ThreadingStrategy(ThreadingStrategyMode = ThreadingStrategyMode.NewBackground)]
        protected virtual void PrivateDoTaskAlwaysInANewBackgound()
        {
            
        }
        
        [ThreadingStrategy(ThreadingStrategyMode = ThreadingStrategyMode.Dispatcher)]
        public virtual void DoTaskOnDispatcher()
        {
            
        }
        
        [ThreadingStrategy(ThreadingStrategyMode = ThreadingStrategyMode.CallingThread)]
        public virtual void DoTaskOnTheCallingThread()
        {
            
        }
    }
}
```
