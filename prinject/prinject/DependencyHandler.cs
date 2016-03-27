using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prinject
{
    public class DependencyHandler
    {
        private static DependencyHandler _handler = null;

        public static DependencyHandler Instance
        {
            get { return _handler ?? new DependencyHandler(); }
        }

        public void InstallDependency(Type t, object obj)
        {
        }

        public void InstallDependency<T>(T obj)
        {
            InstallDependency(typeof(T), obj);
        }

    }
}
