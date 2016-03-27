using NUnit.Framework;
using prinject;
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

        }
    }
}
