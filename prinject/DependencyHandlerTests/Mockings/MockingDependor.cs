using prinject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyHandlerTests.Mockings
{


    public class MockingDependor
    {
        [Resolve]
        public MockingDependency Depend
        {
            get;
            set;
        }

        public MockingDependor()
        {
            Depend = null;
        }
    }
}
