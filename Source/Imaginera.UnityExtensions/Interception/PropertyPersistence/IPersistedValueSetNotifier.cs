namespace Imaginera.UnityExtensions.Interception.PropertyPersistence
{
    /// <summary>
    /// Contract for classes with properties intercepted for persistence using <see cref="PersistedPropertyAttribute" />
    /// When implemented by the containing class provides a calback when the persisted value is retrieved from 
    /// isolated storage
    /// </summary>
    public interface IPersistedValueSetNotifier
    {
        /// <summary>
        /// The property set.
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        void PropertySet(string property);
    }
}
