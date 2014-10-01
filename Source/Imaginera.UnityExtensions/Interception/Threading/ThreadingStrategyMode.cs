namespace Imaginera.UnityExtensions.Interception.Threading
{
    /// <summary>
    /// Enum describing the threading strategy to run code on
    /// </summary>
    public enum ThreadingStrategyMode
    {
        /// <summary>
        /// Runs code on the calling thread
        /// </summary>
        CallingThread,

        /// <summary>
        /// Runs code on the dispatcher
        /// </summary>
        Dispatcher,

        /// <summary>
        /// Runs code on the background thread
        /// </summary>
        Background,

        /// <summary>
        /// Runs code on the background thread which is guaranteed to be newly created
        /// </summary>
        NewBackground
    }
}