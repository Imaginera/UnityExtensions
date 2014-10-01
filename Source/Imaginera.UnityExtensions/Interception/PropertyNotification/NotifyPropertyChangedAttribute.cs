namespace Imaginera.UnityExtensions.Interception.PropertyNotification
{
    using System;

    /// <summary>
    /// The notify property changed attribute .
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NotifyPropertyChangedAttribute : InterceptMethodAttribute
    {
    }
}
