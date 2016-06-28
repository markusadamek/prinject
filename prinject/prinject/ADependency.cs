using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prinject
{
    /// <summary>
    /// Baseclass for a Dependency class
    /// If not uninstalled, the dependency will be uninstalled in the destructor
    /// </summary>
    public abstract class ADependency : IDisposable
    {

        /// <summary>
        /// releases the
        /// </summary>
        public void Dispose()
        {
            DependencyHandler.Instance.UnInstallDependency(this);
        }

        /// <summary>
        /// Installs this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        protected void Install<T>()
        {
            DependencyHandler.Instance.InstallDependency(typeof(T),this);
        }

        /// <summary>
        /// Uninstalls the dependency interface, if it is associated with this object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        protected void UnInstall<T>()
        {
            if(DependencyHandler.Instance.IsDependency<T>(this))
                DependencyHandler.Instance.UnInstallDependency<T>();
        }

        ~ADependency()
        {
            Dispose(); //make sure dispose is called --> but this will never happen!
        }
    }
}
