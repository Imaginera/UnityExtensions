namespace Imaginera.UnityExtensions.Test.UnitTests.SpecificValidations
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using Imaginera.UnityExtensions.Interception;
    using Imaginera.UnityExtensions.Interception.PropertyNotification;
    using Imaginera.UnityExtensions.Test.Annotations;

    using Microsoft.Practices.EnterpriseLibrary.Logging;
    using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NotifyPropertyTests
    {
        private static UnityContainer container;

        private static TestTraceListener logger;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            var assemblies = new List<Assembly> { typeof(InterceptionTest).Assembly };
            container = new UnityContainer();

            var configuration = new LoggingConfiguration();
            logger = new TestTraceListener { Formatter = new TextFormatter("{message}") }; // [{win32ThreadId}] {timestamp} - {title}: {message}
            configuration.AddLogSource("Test", logger);
            Logger.SetLogWriter(new LogWriter(configuration));

            container.AutoConfigure(assemblies);
        }

        [TestInitialize]
        public void Initialize()
        {
            logger.Messages.Clear();
        }

        [TestMethod]
        public void ValidateNotifyClass()
        {
            var validationMessages = container.ValidateConfiguration();

            Assert.AreEqual(0, validationMessages.Count);
        }
    }

    [InterceptClass]
    public class NotifyClass : INotifyPropertyChanged
    {
        [NotifyPropertyChanged]
        public string NotifyProperty { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

