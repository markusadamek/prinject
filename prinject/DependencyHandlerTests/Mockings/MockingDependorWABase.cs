using prinject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyHandlerTests.Mockings
{

    public class MockingDependorWABase : ASubscriber
    {
        [Resolve]
        public IMock Depend
        {
            get;
            set;
        }
    }
}
