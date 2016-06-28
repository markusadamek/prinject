using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prinject
{


    /// <summary>
    /// Behaviour of dependency
    /// </summary>
    public enum DependendyFlag 
    {
        /// <summary>
        /// Even if the dependeny is not resolved it is marked as a resolved dependency
        /// </summary>
        Optional = 1,
        /// <summary>
        /// Shared dependencies are marked as resolved if one of the group is resolved
        /// </summary>
        Shared = 2,
        /// <summary>
        /// Dependency has to be resolved
        /// </summary>
        Essential = 4
    }
    /// <summary>
    /// Attribute marks a Property that it needs 
    /// external resolving by the DependencyHandler
    /// </summary>
    /// <seealso cref="System.Attribute" />
    public class ResolveAttribute : Attribute
    {
        /// <summary>
        /// Gets the flag.
        /// </summary>
        /// <value>
        /// The flag.
        /// </value>
        public DependendyFlag Flag
        {
            get;
            private set;
        } = DependendyFlag.Essential;

        public uint ShareId
        {
            get;
            private set;
        } = 0;

        public ResolveAttribute()
        {
        }
        public ResolveAttribute(DependendyFlag flag)
        {
            Flag = flag;
            if ( flag ==DependendyFlag.Shared)
                throw new DependencyException("If the dependency is marked as shared it needs a share Id!");
        }

        public ResolveAttribute(DependendyFlag flag,uint shareId)
        {
            Flag = flag;
            ShareId = shareId;
        }

    }
}
