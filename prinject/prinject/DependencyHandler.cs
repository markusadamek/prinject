using prinject.DependencyContainer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prinject
{

    /// <summary>
    /// delegate when dependency changed
    /// </summary>
    /// <param name="sender">DependencyHandler or Subscriber</param>
    /// <param name="e">The <see cref="DependencyChangedEventArgs"/> instance containing the event data.</param>
    public delegate void DependencyChanged(object sender, DependencyChangedEventArgs e);
    /// <summary>
    /// DependencyHandler os able to resolve properties with the Attribute [Resolve]
    /// </summary>
    public class DependencyHandler
    {

        /// <summary>
        /// Occurs when a dependency has changed.
        /// </summary>
        public event DependencyChanged DependencyChanged;
        private static DependencyHandler _handler = null;
        private ConcurrentDictionary<Type, object> _dependencies;
        private ConcurrentBag<Subscriber> _subscriptions;
        private ConcurrentBag<KeyValuePair<object,DependencyChanged>> _eventMapping;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyHandler"/> class.
        /// </summary>
        public DependencyHandler()
        {
            _dependencies = new ConcurrentDictionary<Type, object>();
            _subscriptions = new ConcurrentBag<Subscriber>();
            _eventMapping = new ConcurrentBag<KeyValuePair<object, DependencyChanged>>();
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
                return _handler = _handler ?? new DependencyHandler();
            }
        }


        protected void OnDependencyChanged(Type t, object olditem, object newitem)
        {
            DependencyChanged?.Invoke(this, new DependencyChangedEventArgs(t, olditem, newitem));
        }


        /// <summary>
        /// Determines whether the specified object is dependency.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public bool IsDependency<T>(object obj)
        {
            return IsDependency(typeof(T), obj);
        }

        /// <summary>
        /// Determines whether the specified t is dependency.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public bool IsDependency(Type t, object obj)
        {
            if (!_dependencies.ContainsKey(t))
                return false;

            return (_dependencies[t].Equals(obj));
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="t">The type of Dependency</param>
        /// <param name="obj">The object.</param>
        public void InstallDependency(Type t, object obj)
        {
            if (!_dependencies.ContainsKey(t))
                _dependencies.TryAdd(t, obj);
            else
                _dependencies[t] = obj;

            removeLostReferences();
            foreach (Subscriber s in _subscriptions.Where(subscr => subscr.Dependencies.Contains(t)))
                s.SetDepencency(t, obj);
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
        /// Uninstalls all Dependencies created from this object
        /// </summary>
        /// <param name="o">The o.</param>
        public void UnInstallDependency(object o)
        {
            List<Type> to_remove = new List<Type>();
            foreach (var t in _dependencies.Where(p => ReferenceEquals(p.Value, o)))
                to_remove.Add(t.Key);

            foreach (var item in to_remove)
                UnInstallDependency(item);

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

            removeLostReferences();
            foreach (Subscriber subscr in _subscriptions.Where(types => types.Dependencies.Contains(t)))
            {
                subscr.SetDepencency(t, null);
            }

            object output = null;
            _dependencies.TryRemove(t, out output);

        }

        /// <summary>
        /// Removes the lost references.
        /// </summary>
        private void removeLostReferences()
        {

            _subscriptions = new ConcurrentBag<Subscriber>(_subscriptions.Except(_subscriptions.Where(rem => !rem.IsReferenceValid())));
        }




        /// <summary>
        /// Subscribes the specified object
        /// </summary>
        /// <param name="o">The object</param>
        /// <exception cref="DependencyException">Object is already Subscribed</exception>
        public void Subscribe(object o)
        {
            removeLostReferences();
            if (_subscriptions.Any(t => t.CompareToObject(o)))
                throw new DependencyException("Object is already Subscribed");

            var subscr = new Subscriber(o);
            subscr.DependencyChanged += Subscr_DependencyChanged;
            _subscriptions.Add(subscr);

            foreach (Type t in subscr.Dependencies)
            {
                if (_dependencies.ContainsKey(t))
                    subscr.SetDepencency(t, _dependencies[t]);
                else
                    subscr.SetDepencency(t, null);
            }

        }

        /// <summary>
        /// Subscribes to object change.
        /// </summary>
        /// <param name="o">The object changed to which to subscribe</param>
        /// <param name="e">The dependencyChanged </param>
        public void SubscribeToObjectChange(object o, DependencyChanged e)
        {
            foreach (var s in _subscriptions)
            {
                if (s.CompareToObject(o))
                    s.DependencyChanged += e;
            }
        }


        /// <summary>
        /// UnSubscribes to object change.
        /// </summary>
        /// <param name="o">The object changed to which to subscribe</param>
        /// <param name="e">The dependencyChanged </param>
        public void UnSubscribeFromObjectChange(object o, DependencyChanged e)
        {
            foreach (var s in _subscriptions)
            {
                if (s.CompareToObject(o))
                    s.DependencyChanged -= e;
            }
        }

        /// <summary>
        /// forward subscription changes
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DependencyChangedEventArgs"/> instance containing the event data.</param>
        private void Subscr_DependencyChanged(object sender, DependencyChangedEventArgs e)
        {
            OnDependencyChanged(e.DependencyType, e.OldItem, e.NewItem);
        }

        /// <summary>
        /// Unsubscribes the specified object from the DependencyHandler
        /// all dependencies are set to null
        /// </summary>
        /// <param name="o">The object</param>
        /// <exception cref="DependencyException">Object has no subscription</exception>
        public void Unsubscribe(object o)
        {

            var subscriber = _subscriptions.Where(t => t.CompareToObject(o)).FirstOrDefault();

            if (subscriber == null)
                throw new DependencyException("Object has no subscription");

            foreach (var item in subscriber.Dependencies)
                subscriber.SetDepencency(item, null);


            _subscriptions = new ConcurrentBag<Subscriber>(_subscriptions.Except(new[] { subscriber }));
        }

    }
}
