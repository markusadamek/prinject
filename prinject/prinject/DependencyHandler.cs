using prinject.DependencyContainer;
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
        private Dictionary<Type, object> _dependencies;
        private List<Subscriber> _subscriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyHandler"/> class.
        /// </summary>
        public DependencyHandler()
        {
            _dependencies = new Dictionary<Type, object>();
            _subscriptions = new List<Subscriber>();
        }

        /// <summary>
        /// Gets the current instance of the DependencyHandler
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static DependencyHandler Instance
        {
            get
            {
                _handler = _handler ?? new DependencyHandler();
                return _handler;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="t">The type of Dependency</param>
        /// <param name="obj">The object.</param>
        public void InstallDependency(Type t, object obj)
        {
            if (!_dependencies.ContainsKey(t))
                _dependencies.Add(t, obj);
            else
                _dependencies[t] = obj;

            foreach (Subscriber s in _subscriptions.Where(subscr => subscr.Dependencies.Contains(t)))
            {
                if (s.IsReferenceValid())
                    s.SetDepencency(t, obj);
                else
                    _subscriptions.Remove(s);
            }
        }

        /// <summary>
        /// Installs the dependency.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        public void InstallDependency<T>(T obj)
        {
            InstallDependency(typeof(T), obj);
        }

        /// <summary>
        /// Uns the install dependency.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void UnInstallDependency<T>()
        {
            UnInstallDependency(typeof(T));
        }

        /// <summary>
        /// Uninstalls the install dependency.
        /// sets all depending properties to null
        /// </summary>
        /// <param name="t">The t.</param>
        /// <exception cref="DependencyException">No such dependency installed!</exception>
        public void UnInstallDependency(Type t)
        {
            if (!_dependencies.ContainsKey(t))
                throw new DependencyException("No such dependency installed!");

            foreach (Subscriber subscr in _subscriptions.Where(types => types.Dependencies.Contains(t)))
            {
                if (!subscr.IsReferenceValid())
                    _subscriptions.Remove(subscr);
                else
                    subscr.SetDepencency(t, null);
            }

            _dependencies.Remove(t);

        }



        /// <summary>
        /// Subscribes the specified object
        /// </summary>
        /// <param name="o">The object</param>
        /// <exception cref="DependencyException">Object is already Subscribed</exception>
        public void Subscribe(object o)
        {
            if (_subscriptions.Any(t => t.IsObject(o)))
                throw new DependencyException("Object is already Subscribed");

            _subscriptions.Add(new Subscriber(o));
        }

        /// <summary>
        /// Unsubscribes the specified object from the DependencyHandler
        /// </summary>
        /// <param name="o">The object</param>
        /// <exception cref="DependencyException">Object has no subscription</exception>
        public void Unsubscribe(object o)
        {
            if (!_subscriptions.Any(t => t.IsObject(o)))
                throw new DependencyException("Object has no subscription");

            var subscribor = _subscriptions.Find(t => t.IsObject(o));

            _subscriptions.Remove(subscribor);
        }

    }
}
