using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prinject
{
    /// <summary>
    /// Dependency changed event arguments
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class DependencyChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the old item.
        /// </summary>
        /// <value>
        /// The old item.
        /// </value>
        public object OldItem
        {
            get;
        }

        /// <summary>
        /// Gets the new item.
        /// </summary>
        /// <value>
        /// The new item.
        /// </value>
        public object NewItem
        {
            get;
        }

        /// <summary>
        /// Gets the type of the dependency.
        /// </summary>
        /// <value>
        /// The type of the dependency.
        /// </value>
        public Type DependencyType
        {
            get;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyChangedEventArgs"/> class.
        /// </summary>
        /// <param name="t">The Type</param>
        /// <param name="oldItem">The old item.</param>
        /// <param name="newItem">The new item.</param>
        public DependencyChangedEventArgs(Type t,object oldItem, object newItem)
        {
            OldItem = oldItem;
            NewItem = newItem;
            DependencyType = t;
        }
    }
}
