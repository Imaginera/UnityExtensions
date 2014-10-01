namespace Imaginera.UnityExtensions.Example.ViewModels
{
    using System.ComponentModel;

    using Imaginera.UnityExtensions.Interception;
    using Imaginera.UnityExtensions.Interception.Logging;
    using Imaginera.UnityExtensions.Interception.PropertyNotification;
    using Imaginera.UnityExtensions.Interception.PropertyPersistence;

    [InterceptClass]
    public class DashboardViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [Log]
        [NotifyPropertyChanged]
        [PersistedProperty("F98B4D43-AA8F-44D4-B0FA-482ADC2A6611")]
        public virtual string Text { get; set; }
    }
}
