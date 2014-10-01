namespace Imaginera.UnityExtensions.Registration
{
    using System;

    /// <summary>
    /// The lifetime manager attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class LifetimeManagerAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets a value indicating whether a singleton instance will be resolved.
        /// </summary>
        public LifetimeManagerType Type { get; set; }
    }
}