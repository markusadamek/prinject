using DependencyHandlerTests.Mockings;
using NUnit.Framework;
using prinject;
using prinject.DependencyContainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            Assert.That(dependor.Depend, Is.EqualTo(dependency));
            DependencyHandler.Instance.UnInstallDependency<MockingDependency>();
            Assert.That(dependor.Depend, Is.Null);

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
            Assert.That(subscr.Dependencies.Length, Is.EqualTo(1), "Number of Dependencies wrong");
            Assert.That(subscr.Dependencies[0], Is.EqualTo(typeof(MockingDependency)));
            Assert.That(subscr.CompareToObject(subscr), Is.False, "Object check wrong");
            Assert.That(subscr.CompareToObject(dependor), Is.True, "object check wrong");
            Assert.That(subscr.IsReferenceValid(), Is.True, "Reference not valid");
            Assert.That(() => subscr.SetDepencency(typeof(object), new object()), Throws.Exception.TypeOf<DependencyException>());
            dependor = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.WaitForFullGCComplete();
            Assert.That(subscr.IsReferenceValid(), Is.False, "Reference is still valid; Fails in Release mode");
            Assert.That(subscr.TrySetDependency<MockingDependency>(new object()), Is.False, "Check if Weak reference can still retrieve object with tryset");
            Assert.That(subscr.IsReferenceValid(), Is.False, "Check if Weak reference can still retrieve object");
            Assert.That(() => subscr.SetDepencency(typeof(MockingDependency), null), Throws.Exception.TypeOf<DependencyException>(), "Dependency still exists");
        }

        [Test]
        public void TestUnsubscribe()
        {
            MockingDependor dependor = new MockingDependor();
            MockingDependency dependency = new MockingDependency();
            DependencyHandler.Instance.InstallDependency<MockingDependency>(dependency);
            DependencyHandler.Instance.Subscribe(dependor);
            DependencyHandler.Instance.Unsubscribe(dependor);
            Assert.That(dependor.Depend, Is.Null);
        }

        [Test]
        public void TestResolvingCheck()
        {
            MockingDependor dependor = new MockingDependor();
            Subscriber subscr = new Subscriber(dependor);
            Assert.That(!subscr.IsDependencyResolved());
            subscr.SetDepencency(typeof(MockingDependency), new MockingDependency());
            Assert.That(subscr.IsDependencyResolved());

            MockingDepCheck dep = new MockingDepCheck();
            Subscriber subscr2 = new Subscriber(dep);
            Assert.That(!subscr2.IsDependencyResolved());
            subscr2.SetDepencency(typeof(string), "  ");
            Assert.That(!subscr2.IsDependencyResolved());

            subscr2.SetDepencency(typeof(object), new object());
            Assert.That(subscr2.IsDependencyResolved());

        }

        [Test]
        public void CheckEvents()
        {
            DependencyHandler handler = new DependencyHandler();
            MockingDependor dependor = new MockingDependor();
            MockingDependency dependency = new MockingDependency();
            DependencyChangedEventArgs fullArgs = null;
            handler.DependencyChanged += (s, e) => fullArgs = e;
            handler.Subscribe(dependor);

            DependencyChangedEventArgs args = null;
            
            DependencyChanged ev=(s, e) => { args = e; };
            handler.SubscribeToObjectChange(dependor, ev);

            handler.InstallDependency(dependency);

            Assert.That(args.OldItem == null);
            Assert.That(args.NewItem == dependency);

            Assert.That(fullArgs.OldItem == null);
            Assert.That(fullArgs.NewItem == dependency);


            fullArgs = null;
            args = null;
            handler.UnSubscribeFromObjectChange(dependor, ev);

            handler.UnInstallDependency(dependency);
            Assert.That(args == null);

            Assert.That(fullArgs.NewItem == null);
            Assert.That(fullArgs.OldItem == dependency);
        }

    }
}
