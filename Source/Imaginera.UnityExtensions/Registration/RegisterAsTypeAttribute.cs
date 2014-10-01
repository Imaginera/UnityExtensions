namespace Imaginera.UnityExtensions.Registration
{
    using System;

    /// <summary>
    /// The attribute to allow registrations of the class as a specific type
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterAsTypeAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the type to register the class as
        /// </summary>
        public Type RegistrationType { get; set; }
    }
}
