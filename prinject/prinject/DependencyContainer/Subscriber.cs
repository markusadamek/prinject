using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace prinject.DependencyContainer
{


    /// <summary>
    /// Creates a weak reference and changes all marked types
    /// </summary>
    public class Subscriber
    {

        private WeakReference<object> _internalref;

        private Dictionary<Type, PropertyInfo> _setDictionary;

        /// <summary>
        /// Occurs when the dependency changes
        /// </summary>
        public event DependencyChanged DependencyChanged;

        /// <summary>
        /// Available Dependencies of the Subscriber
        /// </summary>
        /// <value>
        /// The dependencies.
        /// </value>
        public Type[] Dependencies
        {
            get { return _setDictionary.Keys.ToArray(); }
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="Subscriber"/> class.
        /// </summary>
        /// <param name="t">The t.</param>
        public Subscriber(object t)
        {

            _internalref = new WeakReference<object>(t);

            findDependencies(t);

        }

        /// <summary>
        /// sets the dependency dictionary
        /// </summary>
        /// <param name="obj">The object.</param>
        private void findDependencies(object obj)
        {
            _setDictionary = (from prop in obj.GetType().GetProperties() where obj.GetType().IsPublic && prop.CanWrite && prop.CustomAttributes.Any(attr => attr.AttributeType == typeof(ResolveAttribute)) select new KeyValuePair<Type, PropertyInfo>(prop.PropertyType, prop)).ToDictionary(p => p.Key, p => p.Value);

        }

        /// <summary>
        /// Determines whether the dependency is resolved.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is dependency resolved]; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="DependencyException">
        /// Object does not exist anymore
        /// or
        /// Unkown dependency flag
        /// </exception>
        public bool IsDependencyResolved()
        {
            object obj = null;
            bool isResolved = true;
            if (!_internalref.TryGetTarget(out obj))
                throw new DependencyException("Object does not exist anymore");

            Dictionary<uint, int> shareDict = new Dictionary<uint, int>();

            foreach (var item in _setDictionary)
            {
                var attr = item.Value.GetCustomAttribute<ResolveAttribute>();
                if (!isResolved)
                    break;
                switch (attr.Flag)
                {
                    case DependendyFlag.Essential:
                        isResolved &= (item.Value.GetMethod.Invoke(obj, new object[] { }) != null);
                        break;
                    case DependendyFlag.Optional:

                        break;
                    case DependendyFlag.Shared:
                        if (!shareDict.ContainsKey(attr.ShareId))
                            shareDict.Add(attr.ShareId, 0);
                        if (item.Value.GetMethod.Invoke(obj, new object[] { }) != null)
                            shareDict[attr.ShareId]++;
                        break;
                    default:
                        throw new DependencyException("Unkown dependency flag");
                }
            }

            //resolve shared
            if (shareDict.ContainsValue(0))
                return false;

            return isResolved;
        }

        /// <summary>
        /// Determines whether the reference is valid.
        /// </summary>
        /// <returns></returns>
        public bool IsReferenceValid()
        {
            object t = null;
            return _internalref.TryGetTarget(out t);
        }

        /// <summary>
        /// Sets the depencency.
        /// </summary>
        /// <param name="t">Type of the dependency</param>
        /// <param name="o">object on which it is set</param>
        /// <exception cref="DependencyException">
        /// Dependency does not contain given Type
        /// or
        /// Reference is not valid
        /// </exception>
        public void SetDepencency(Type t, object o)
        {
            if (!Dependencies.Contains(t))
                throw new DependencyException("Dependency does not contain given Type");

            object obj;
            if (!_internalref.TryGetTarget(out obj))
                throw new DependencyException("Reference is not valid");

            setDependencyInternal(t, o, obj);

        }

        /// <summary>
        /// Sets the dependency internal.
        /// </summary>
        /// <param name="t">type to set</param>
        /// <param name="o">object to set</param>
        /// <param name="obj">reference to the object (resolved weakref)</param>
        private void setDependencyInternal(Type t, object o, object obj)
        {

            object old = _setDictionary[t].GetMethod.Invoke(obj, new object[] { });
            _setDictionary[t].SetMethod.Invoke(obj, new object[] { o });

            OnDependencyChanged(t, old, o);
        }

        /// <summary>
        /// Tries the set dependency.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dep">The dep.</param>
        /// <returns></returns>
        public bool TrySetDependency<T>(object dep)
        {
            object obj = null;

            if (!Dependencies.Contains(typeof(T)))
                return false;


            if (!_internalref.TryGetTarget(out obj))
                return false;

            setDependencyInternal(typeof(T), dep, obj);
            return true;
        }

        /// <summary>
        /// Compares to check if the object is equal to the object in the weak reference
        /// </summary>
        /// <param name="check">The check.</param>
        /// <returns>true if objects are equal</returns>
        /// <exception cref="DependencyException">Reference is not valid</exception>
        public bool CompareToObject(object check)
        {
            object obj;
            if (!_internalref.TryGetTarget(out obj))
                throw new DependencyException("Reference is not valid");

            return object.ReferenceEquals(obj, check);
        }


        /// <summary>
        /// Called when [dependency changed].
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="olditem">The olditem.</param>
        /// <param name="newitem">The newitem.</param>
        private void OnDependencyChanged(Type t, object olditem, object newitem)
        {
            DependencyChanged?.Invoke(this, new DependencyChangedEventArgs(t, olditem, newitem));
        }


    }
}
