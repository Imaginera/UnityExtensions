namespace Imaginera.UnityExtensions.Interception.PropertyPersistence
{
    using System;

    /// <summary>
    /// The notify property changed attribute .
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PersistedPropertyAttribute : InterceptMethodAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersistedPropertyAttribute"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public PersistedPropertyAttribute(string id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Gets or sets the id of the persisted property - ideally a GUID
        /// </summary>
        public string Id { get; set; }
    }
}