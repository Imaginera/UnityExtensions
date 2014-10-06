UnityExtensions
===============

AOP Extensions to Unity

## Extensions Setup

To utilise the AutoConfigure extension, call the method passing in the project assemblies. Before this is done the logging application block is configured.

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

To configure the LifeTimeManager for a class:
```csharp
namespace NewApp
{
    [LifetimeManager(Type = LifetimeManagerType.PerResolve]
    public class SomeViewModel : ISomeViewModel
    {
    }
}
```
If you were creating a design view model that inherited the same interface as the runtime one, you can tell unity not register that view model using the ExcludeRegistration attribute:
```csharp
namespace NewApp.DesignTime
{
    [ExcludeRegistration]
    public class SomeViewModel : ISomeViewModel
    {
    }
}
```
If you have many concrete types inheriting from the same interface, you can name these registrations to let unity know which concrete type to fetch using the NameRegistration.
```csharp
namespace NewApp.DesignTime
{
    [NamedRegistration(RegistrationName = “SomeViewModel”)]
    public class SomeViewModel : ISomeViewModel
    {
    }
}
```
The unity extensions also allow you to register a type to another specified type by using the RegisterAsType attribute.
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
