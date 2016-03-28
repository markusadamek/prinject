using DependencyHandlerTests.Mockings;
using NUnit.Framework;
using prinject;
using prinject.DependencyContainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyHandlerTests
{
    [TestFixture]
    public class HandlerTests
    {
        
        [Test]
        public void SimpleDependencyTest()
        {
            Assert.That(DependencyHandler.Instance, Is.Not.Null);
            DependencyHandler.Instance.InstallDependency(new object());
            DependencyHandler.Instance.InstallDependency(new object());
            DependencyHandler.Instance.UnInstallDependency<object>();
            Assert.That(() => DependencyHandler.Instance.UnInstallDependency<object>(), Throws.Exception.TypeOf<DependencyException>());

        }

        [Test]
        public void DependencyTestWithBaseClass()
        {
            MockingDependorWABase subscr = new MockingDependorWABase();
            using (ADependency dep = new MockingDependencyWABase())
            {
                
                Assert.That(subscr.Depend, Is.EqualTo(dep));
            }
          
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Assert.That(subscr.Depend, Is.EqualTo(null));


        }

        [Test]
        public void TestInjection()
        {
            MockingDependor dependor = new MockingDependor();
            MockingDependency dependency = new MockingDependency();
            DependencyHandler.Instance.InstallDependency<MockingDependency>(dependency);
            DependencyHandler.Instance.Subscribe(dependor);
            Assert.That(dependor.Depend,Is.EqualTo(dependency));
            DependencyHandler.Instance.UnInstallDependency<MockingDependency>();
            Assert.That(dependor.Depend,Is.Null);
           
        }

        [Test]
        public void TestSubscriptions()
        {
            MockingDependor dependor = new MockingDependor();
            MockingDependency dependency = new MockingDependency();
            DependencyHandler.Instance.InstallDependency<MockingDependency>(dependency);
            DependencyHandler.Instance.Subscribe(dependor);
            Assert.That(dependor.Depend, Is.EqualTo(dependency));
            DependencyHandler.Instance.UnInstallDependency(dependency);
            Assert.That(dependor.Depend, Is.Null);
        }

        [Test]
        public void TestSubscriber()
        {
            MockingDependor dependor = new MockingDependor();
            Subscriber subscr = new Subscriber(dependor);
            Assert.That(subscr.Dependencies.Length, Is.EqualTo(1));
            Assert.That(subscr.Dependencies[0], Is.EqualTo(typeof(MockingDependency)));
            Assert.That(subscr.IsObject(subscr),Is.False);
            Assert.That(subscr.IsObject(dependor), Is.True);
            Assert.That(subscr.IsReferenceValid());
            Assert.That(() => subscr.SetDepencency(typeof(object), new object()), Throws.Exception.TypeOf<DependencyException>());
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Assert.That(subscr.IsReferenceValid(), Is.True);
            dependor = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Assert.That(subscr.IsReferenceValid(),Is.False);
            Assert.That(() => subscr.SetDepencency(typeof(MockingDependency), null), Throws.Exception.TypeOf<DependencyException>());
        }
    }
}
