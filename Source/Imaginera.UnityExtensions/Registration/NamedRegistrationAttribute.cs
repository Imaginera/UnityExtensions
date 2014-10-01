namespace Imaginera.UnityExtensions.Registration
{
    using System;

    /// <summary>
    /// The named registration attribute to allow registrations to be specifically named in the container
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class NamedRegistrationAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the registration name.
        /// </summary>
        public string RegistrationName { get; set; }
    }
}
