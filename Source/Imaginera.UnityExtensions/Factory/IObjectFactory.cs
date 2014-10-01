namespace Imaginera.UnityExtensions.Factory
{
    /// <summary>
    /// Contract for classes which offer object factory functionaliy
    /// An object factory should be able to create instances of the requested class
    /// </summary>
    /// <typeparam name="T">
    /// The type of class to create
    /// </typeparam>
    public interface IObjectFactory<T>
    {
        /// <summary>
        /// Creates a new instance of an object
        /// </summary>
        /// <returns>
        /// The type of the class to create a new instance of
        /// </returns>
        T Create();
    }
}
