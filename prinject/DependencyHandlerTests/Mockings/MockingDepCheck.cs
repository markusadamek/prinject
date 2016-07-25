using prinject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyHandlerTests.Mockings
{
    public class MockingDepCheck
    {
        [Resolve(DependendyFlag.Essential )]
        public string Essential
        {
            get;
            set;
        } = null;


        [Resolve(DependendyFlag.Optional)]
        public int? Optional
        {
            get;
            set;
        } = null;


        [Resolve(DependendyFlag.Shared,0)]
        public double? Shared1
        {
            get;
            set;
        }

        [Resolve(DependendyFlag.Shared, 0)]
        public object Shared2
        {
            get;
            set;
        }
    }
}
