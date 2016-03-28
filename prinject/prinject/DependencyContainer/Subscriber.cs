using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace prinject.DependencyContainer
{
    public class Subscriber
    {
        private WeakReference<object> _internalref;

        private Dictionary<Type, MethodInfo> _setDictionary;

        public Type[] Dependencies
        {
            get { return _setDictionary.Keys.ToArray(); }
        }
        
        public Subscriber(object t)
        {
            
            _internalref = new WeakReference<object>(t);
            findDependencies(t);
            
        }

        private void findDependencies(object obj)
        {
            _setDictionary = (from prop in obj.GetType().GetProperties() where obj.GetType().IsPublic && prop.CanWrite && prop.CustomAttributes.Any(attr => attr.AttributeType == typeof(ResolveAttribute)) select new KeyValuePair<Type,MethodInfo>(prop.PropertyType,prop.SetMethod)).ToDictionary( p => p.Key,p=> p.Value);

        }

        public bool IsReferenceValid()
        {
            object t;
            return _internalref.TryGetTarget(out t);
        }

        public void SetDepencency(Type t, object o)
        {
            if (!Dependencies.Contains(t))
                throw new DependencyException("Dependency does not contain given Type");

            object obj;
            if (!_internalref.TryGetTarget(out obj))
                throw new DependencyException("Reference is not valid");

            _setDictionary[t].Invoke(obj, new object[] { o });

        }

        public bool IsObject(object check)
        {
            object obj;
            if (!_internalref.TryGetTarget(out obj))
                throw new DependencyException("Reference is not valid");

            return object.ReferenceEquals(obj, check);
        }

       

    }
}
