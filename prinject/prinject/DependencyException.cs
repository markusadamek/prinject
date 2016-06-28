using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prinject
{

    /// <summary>
    /// Thrown by the DependencyHandler
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class DependencyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyException"/> class.
        /// </summary>
        public DependencyException() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DependencyException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public DependencyException(string message, Exception inner) : base(message, inner) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected DependencyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { }
    }

}
