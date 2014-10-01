namespace Imaginera.UnityExtensions.Test.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading;

    using Imaginera.UnityExtensions.Factory;
    using Imaginera.UnityExtensions.Interception;
    using Imaginera.UnityExtensions.Interception.Logging;
    using Imaginera.UnityExtensions.Interception.PropertyNotification;
    using Imaginera.UnityExtensions.Interception.PropertyPersistence;
    using Imaginera.UnityExtensions.Interception.Threading;
    using Imaginera.UnityExtensions.Registration;

    using Microsoft.Practices.EnterpriseLibrary.Logging;
    using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class InterceptionTest
    {
        private static IObjectFactory<Interceptable> factory;

        private static UnityContainer container;

        private static TestTraceListener logger;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            var assemblies = new List<Assembly> { typeof(InterceptionTest).Assembly };
            container = new UnityContainer();
            
            var configuration = new LoggingConfiguration();
            logger = new TestTraceListener { Formatter = new TextFormatter("[{severity}] {message}") };
            configuration.AddLogSource("Test", logger);
            Logger.SetLogWriter(new LogWriter(configuration));

            container.AutoConfigure(assemblies);
            factory = container.Resolve<IObjectFactory<Interceptable>>();
        }

        [TestInitialize]
        public void Initialize()
        {
            logger.Messages.Clear();
        }

        [TestMethod]
        public void LogTest()
        {
            const string DataToTestWith = "data to test with";

            var data = factory.Create();
            data.LogTestHelper(DataToTestWith);

            var loggedItems = logger.Messages;
            Assert.AreEqual(2, loggedItems.Count);
            Assert.AreEqual("[Verbose] Interceptable.LogTestHelper - Called with parameters [data to test with]", loggedItems[0]);
            Assert.IsTrue(new Regex(@"^\[Verbose\] Interceptable.LogTestHelper - Complete in \[.*ms\] and returned \[data to test with\]$").IsMatch(loggedItems[1]));
        }

        [TestMethod]
        public void ThreadingTest()
        {
            var data = factory.Create();
        
            data.RunOnCallingThread();
            logger.WaitHandle.WaitOne(1000);
            
            data.RunOnBackgroundThread();
            logger.WaitHandle.WaitOne(1000);
            
            data.RunOnNewBackgroundThread();
            logger.WaitHandle.WaitOne(1000);
            
            data.RunOnDispatcherThread();
            logger.WaitHandle.WaitOne(1000);
            
            Assert.AreEqual(4, logger.Messages.Count);
            Assert.AreEqual(logger.Messages[0], logger.Messages[1]);
            Assert.AreNotEqual(logger.Messages[0], logger.Messages[2]);
            Assert.AreEqual(logger.Messages[0], logger.Messages[3]);
        }

        [TestMethod]
        public void PersistenceTest()
        {
            int value = new Random().Next(100);

            var data = factory.Create();
            data.PersistedValue = value;

            var data2 = factory.Create();

            Assert.AreNotSame(data, data2);
            Assert.AreEqual(value, data.PersistedValue); 
            Assert.AreEqual(data.PersistedValue, data2.PersistedValue);
        }

        [TestMethod]
        public void NotificationTest()
        {
            int value = new Random().Next(100);
            var waitHandle = new AutoResetEvent(false);

            var data = factory.Create();
            data.PropertyChanged += (sender, args) =>
                {
                    Assert.AreEqual("NotifyValue", args.PropertyName);
                    waitHandle.Set();
                };
            data.NotifyValue = value;
            bool notified = waitHandle.WaitOne(1000);
            Assert.IsTrue(notified);
        }

        [TestMethod]
        public void NotifyOnlyWithChangedTest()
        {
            int value = new Random().Next(100);
            var waitHandle = new AutoResetEvent(false);

            var data = factory.Create();
            data.PropertyChanged += (sender, args) =>
                {
                    Assert.AreEqual("NotifyValue", args.PropertyName);
                    waitHandle.Set();
                };

            data.NotifyValue = value;
            bool notified = waitHandle.WaitOne(1000);

            waitHandle.Reset();
            data.NotifyValue = value;

            bool notifiedAgain = waitHandle.WaitOne(1000);

            Assert.IsTrue(notified);
            Assert.IsFalse(notifiedAgain);
        }

        [TestMethod]
        public void CombinedTest()
        {
            int value = 1;
            var waitHandle = new AutoResetEvent(false);

            var data = factory.Create();
            data.PropertyChanged += (sender, args) =>
                {
                    Assert.AreEqual("NotifyValue", args.PropertyName);
                    waitHandle.Set();
                };

            data.NotifyValue = value;
            bool notified = waitHandle.WaitOne(1000);
            Assert.IsTrue(notified);

            var loggedItems = logger.Messages;
            Assert.IsTrue(loggedItems.Count >= 2);
            Assert.IsTrue(loggedItems.First().Contains("[Verbose] Interceptable.set_NotifyValue - Called with parameters [1]"));
            Assert.IsTrue(new Regex(@"^\[Verbose\] Interceptable.set_NotifyValue - Complete in \[.*ms\] and returned \[void\]$").IsMatch(loggedItems.Last()));
        }

        [TestMethod]
        public void SubClassTest()
        {
            var subClassFactory = container.Resolve<IObjectFactory<SubClass>>();
            var subClass = subClassFactory.Create();

            subClass.SubProperty = 1;
            subClass.BaseProperty = 2;

            var loggedItems = logger.Messages;
            Assert.AreEqual(4, loggedItems.Count);
            Assert.AreEqual("[Verbose] SubClass.set_SubProperty - Called with parameters [1]", loggedItems[0]);
            Assert.AreEqual("[Verbose] BaseClass.set_BaseProperty - Called with parameters [2]", loggedItems[2]);
            
        }
    }

    [InterceptClass]
    [ExcludeRegistration]
    public class BaseClass : IBaseInterface
    {
        [Log(Category = "Test", Severity = TraceEventType.Verbose)]
        public virtual int BaseProperty { get; set; }
    }

    public interface IBaseInterface
    {
        int BaseProperty { get; set; }
    }

    [InterceptClass]
    public class SubClass : BaseClass, ISubInterface
    {
        [Log(Category = "Test")]
        public virtual int SubProperty { get; set; }        
    }

    public interface ISubInterface : IBaseInterface
    {
        int SubProperty { get; set; }
    }

    [InterceptClass]
    [LifetimeManager(Type = LifetimeManagerType.PerResolve)]
    public class Interceptable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [PersistedProperty("67C60580-C9EC-49C8-A320-2BD649129578")]
        public virtual int PersistedValue { get; set; }

        [Log(Category = "Test")]
        [NotifyPropertyChanged]
        public virtual int NotifyValue { get; set; }

        [Log(Category = "Test")]
        public virtual string LogTestHelper(string itemToLog)
        {
            return itemToLog;
        }

        [ThreadingStrategy(ThreadingStrategyMode = ThreadingStrategyMode.Background)]
        public virtual void RunOnBackgroundThread()
        {
            Logger.Write(Thread.CurrentThread.ManagedThreadId);
        }

        [ThreadingStrategy(ThreadingStrategyMode = ThreadingStrategyMode.CallingThread)]
        public virtual void RunOnCallingThread()
        {
            Logger.Write(Thread.CurrentThread.ManagedThreadId);
        }

        [ThreadingStrategy(ThreadingStrategyMode = ThreadingStrategyMode.Dispatcher)]
        public virtual void RunOnDispatcherThread()
        {
            Logger.Write(Thread.CurrentThread.ManagedThreadId);
        }

        [ThreadingStrategy(ThreadingStrategyMode = ThreadingStrategyMode.NewBackground)]
        public virtual void RunOnNewBackgroundThread()
        {
            Logger.Write(Thread.CurrentThread.ManagedThreadId);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}