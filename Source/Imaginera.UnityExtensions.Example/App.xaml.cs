namespace Imaginera.UnityExtensions.Example
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Windows;

    using Imaginera.UnityExtensions.Example.Views;

    using Microsoft.Practices.EnterpriseLibrary.Logging;
    using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void ConfigureContainer(object sender, StartupEventArgs e)
        {
            var configuration = new LoggingConfiguration();
            var logger = new DebugTraceListener { Formatter = new TextFormatter("[{win32ThreadId}] {timestamp} - {title}: {message}") };
            configuration.AddLogSource("Test", logger);
            Logger.SetLogWriter(new LogWriter(configuration));

            IUnityContainer container = new UnityContainer();
            container.AutoConfigure(new List<Assembly> { Assembly.GetAssembly(typeof(App)) });

            foreach (var error in container.ValidateConfiguration())
            {
                Debug.Assert(false, error);    
            }

            this.MainWindow = container.Resolve<DashboardView>();
            this.MainWindow.Show();
        }
    }
}
