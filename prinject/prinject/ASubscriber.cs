using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prinject
{
    /// <summary>
    /// Abstract class that automatically subscribs
    /// the instance to the active Handler
    /// </summary>
    public abstract class ASubscriber
    {
        public ASubscriber()
        {
            DependencyHandler.Instance.Subscribe(this);
        }
    }
}
