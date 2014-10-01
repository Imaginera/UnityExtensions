namespace Imaginera.UnityExtensions.Interception.PropertyPersistence
{
    /// <summary>
    /// Contract for classes using <see cref="PersistedPropertyAttribute"/> marked properties which require a default value
    /// </summary>
    public interface IPersistedValueDefaultValueProvider
    {
        /// <summary>
        /// Provides a default value for a persisted property
        /// </summary>
        /// <param name="property">
        /// The property requiring a default value
        /// </param>
        /// <returns>
        /// The default value to use
        /// </returns>
        object GetDefaultValue(string property);
    }
}
