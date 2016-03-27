using NUnit.Framework;
using prinject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DependencyHandlerTests
{
    [TestFixture]
    public class HandlerTests
    {

        [Test]
        void SimpleDependencyTest()
        {
            DependencyHandler.Instance.InstallDependency(new object());
        }
    }
}
