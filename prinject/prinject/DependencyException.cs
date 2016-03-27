using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prinject
{

    [Serializable]
    public class DependencyException : Exception
    {
        public DependencyException() { }
        public DependencyException(string message) : base(message) { }
        public DependencyException(string message, Exception inner) : base(message, inner) { }
        protected DependencyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { }
    }

}
