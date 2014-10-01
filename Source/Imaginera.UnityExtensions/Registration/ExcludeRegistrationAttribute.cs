namespace Imaginera.UnityExtensions.Registration
{
    using System;

    /// <summary>
    /// The exclude registration attribute will prevent a class from being configured in the container
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class ExcludeRegistrationAttribute : Attribute
    {
    }
}
