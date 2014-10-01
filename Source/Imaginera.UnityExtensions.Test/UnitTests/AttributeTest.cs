namespace Imaginera.UnityExtensions.Test.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Imaginera.UnityExtensions.Interception;
    using Imaginera.UnityExtensions.Registration;

    using Microsoft.Practices.EnterpriseLibrary.Logging;
    using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AttributeTest
    {
        private static UnityContainer container;

        private static TestTraceListener logger;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            var assemblies = new List<Assembly> { typeof(AttributeTest).Assembly };
            container = new UnityContainer();

            var configuration = new LoggingConfiguration();
            logger = new TestTraceListener { Formatter = new TextFormatter("[{severity}] {message}") };
            configuration.AddLogSource("Test", logger);
            Logger.SetLogWriter(new LogWriter(configuration));
            try
            {
                container.AutoConfigure(assemblies);
            }
            catch (Exception)
            {
                
            }
            
        }
        
        [TestInitialize]
        public void Initialize()
        {
            logger.Messages.Clear();
        }

        [TestMethod]
        public void DuplicateInterfaceImplementationTest()
        {
            try
            {
                container.Resolve<IC>();

                
            }
            catch (Exception)
            {
            }

            Assert.IsNotNull(container.Resolve<A>());
            Assert.IsNotNull(container.Resolve<B>());
        }

        [TestMethod]
        public void GenericInterfaceTest()
        {
            // Resolve a strong class to both respective interface (ID, IE) and IS<T>
            Assert.AreEqual(typeof(StrongClass), container.Resolve<ID>().GetType());
            Assert.AreEqual(typeof(StrongClass), container.Resolve<IS<string>>().GetType());

            Assert.AreEqual(typeof(StrongClass2), container.Resolve<IE>().GetType());
            Assert.AreEqual(typeof(StrongClass2), container.Resolve<IS<bool>>().GetType());
        }
    }

    public interface IA
    {
        
    }

    public interface IB
    {
        
    }

    public interface ID
    {
        
    }

    public interface IE
    {
        
    }

    public interface IS<T>
    {
        
    }

    [ExcludeRegistration]
    public interface IC
    {
        
    }

    [InterceptClass]
    public class A : IA, IC
    {
        
    }

    [InterceptClass]
    public class B : IB, IC
    {
        
    }

    public class StrongClass : ID, IS<string>
    {
        
    }

    public class StrongClass2 : IE, IS<bool>
    {
        
    }
}
