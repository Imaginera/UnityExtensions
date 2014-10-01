namespace Imaginera.UnityExtensions.Test.UnitTests
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;

    using Imaginera.UnityExtensions.Interception;
    using Imaginera.UnityExtensions.Interception.Logging;
    using Imaginera.UnityExtensions.Interception.PropertyNotification;
    using Imaginera.UnityExtensions.Interception.Threading;

    using Microsoft.Practices.EnterpriseLibrary.Logging;
    using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ValidationTest
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
        public void PropertyChangedTest()
        {
            var validationFailures = container.ValidateConfiguration();

            Assert.AreEqual(9, validationFailures.Count);
            
            Assert.IsTrue(validationFailures.Contains("Private method PrivateMethod in type Imaginera.UnityExtensions.Test.UnitTests.Validation will not be intercepted. Change method to be at least protected"));
            Assert.IsTrue(validationFailures.Contains("Private property PrivateProperty in type Imaginera.UnityExtensions.Test.UnitTests.Validation will not be intercepted. Change property to be at least protected"));
            Assert.IsTrue(validationFailures.Contains("Non virtual method PrivateMethod in type Imaginera.UnityExtensions.Test.UnitTests.Validation will not be intercepted. Mark method as virtual"));
            Assert.IsTrue(validationFailures.Contains("NotifyPropertyChanged used on NotifyPropertyChangedProperty in type Imaginera.UnityExtensions.Test.UnitTests.Validation which does not implement INotifyPropertyChanged"));
            Assert.IsTrue(validationFailures.Contains("Private property NotifyPropertyChangedProperty in type Imaginera.UnityExtensions.Test.UnitTests.Validation will not raise property changed notifications. Change property to be at least protected"));
            Assert.IsTrue(validationFailures.Contains("Class ValidationLogNotMarkedIntercept has intercept methods but no InterceptClass attribute defined."));
            Assert.IsTrue(validationFailures.Contains("Class ValidationNotifyPropertyChangedNotMarkedIntercept has intercept methods but no InterceptClass attribute defined."));
        }

        [TestMethod]
        public void SetterNotExistPropertyTest()
        {
            var validationFailures = container.ValidateConfiguration();

            Assert.AreEqual(9, validationFailures.Count);

            Assert.IsTrue(validationFailures.Contains("Property NotifyPropertyChangedProperty in type Imaginera.UnityExtensions.Test.UnitTests.PropertyNotMarkedVirtual will not raise property changed notifications. Change property to at least have a setter"));
        }

        [TestMethod]
        public void MethodNotVoid()
        {
            var validationFailures = container.ValidateConfiguration();

            Assert.AreEqual(9, validationFailures.Count);

            Assert.IsTrue(validationFailures.Contains("Method DoThisInBackground in type Imaginera.UnityExtensions.Test.UnitTests.MethodNotVoidThreading will not return any object. Change method to return type of Void"));
        }
    }

    [InterceptClass]
    public class MethodNotVoidThreading
    {
        [ThreadingStrategy(ThreadingStrategyMode = ThreadingStrategyMode.Background)]
        public virtual string DoThisInBackground()
        {
            return string.Empty;
        }
    }


    [InterceptClass]
    public class PropertyNotMarkedVirtual : INotifyPropertyChanged
    {
        [NotifyPropertyChanged]
        public virtual int NotifyPropertyChangedProperty
        {
            get
            {
                return 1;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    [InterceptClass]
    public class Validation
    {
        [Log]
        private int PrivateProperty { get; set; }

        [Log]
        private void PrivateMethod()
        {
        }

        [NotifyPropertyChanged]
        public virtual int NotifyPropertyChangedProperty { get; private set; }
    }



    public class ValidationLogNotMarkedIntercept
    {
        [Log]
        public virtual void PrivateMethod()
        {

        }
    }

    public class ValidationNotifyPropertyChangedNotMarkedIntercept : INotifyPropertyChanged
    {
        [NotifyPropertyChanged]
        public virtual int NotifyPropertyChangedProperty { get; protected set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    [InterceptClass]
    public class BaseValidationClass
    {
        [Log]
        public virtual void PublicMethod()
        {

        }
    }

    public class NotMarkedInterceptButInheritsIntercept : BaseValidationClass
    {
    }
}