using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prinject
{
    public abstract class ADependency :IDisposable
    {
        
        public void Dispose()
        {
            DependencyHandler.Instance.UnInstallDependency(this);
        }

        protected void Install<T>()
        {
            DependencyHandler.Instance.InstallDependency(typeof(T),this);
        }
    }
}
