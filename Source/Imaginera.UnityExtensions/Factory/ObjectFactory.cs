namespace Imaginera.UnityExtensions.Factory
{
    using Microsoft.Practices.Unity;

    /// <summary>
    /// Implementation of <see cref="IObjectFactory{T}" /> which uses an <see cref="IUnityContainer" />
    /// to resolve new instances of a class.
    /// Using this factory will allow the explicit creation of intercepted objects
    /// </summary>
    /// <typeparam name="T">
    /// The type of class to create an instance of
    /// </typeparam>
    public class ObjectFactory<T> : IObjectFactory<T>
    {
        /// <summary>
        /// The unity container to use to resolve instances of a class
        /// </summary>
        private readonly IUnityContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectFactory{T}"/> class.
        /// </summary>
        /// <param name="container">
        /// The unity container to use to resolve instances of a class
        /// </param>
        public ObjectFactory(IUnityContainer container)
        {
            this.container = container;
        }

        /// <summary>
        /// Creates a new instance of an object
        /// </summary>
        /// <returns>
        /// The type of the class to create a new instance of
        /// </returns>
        public T Create()
        {
            return this.container.Resolve<T>();
        }
    }
}