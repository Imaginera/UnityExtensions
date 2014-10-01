namespace Imaginera.UnityExtensions.Registration
{
    using Microsoft.Practices.Unity;

    /// <summary>
    /// Enum containing options for <see cref="LifetimeManagerAttribute" /> to indicate 
    /// which implementation of lifetime manager it would like to be managed by
    /// </summary>
    public enum LifetimeManagerType
    {
        /// <summary>
        /// Will force usage of a <see cref="PerResolveLifetimeManager" />
        /// </summary>
        PerResolve,

        /// <summary>
        /// Will force usage of a <see cref="ContainerControlledLifetimeManager" />
        /// </summary>
        ContainerControlled
    }
}
