using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prinject
{
    public abstract class ASubscriber
    {
        public ASubscriber()
        {
            DependencyHandler.Instance.Subscribe(this);
        }
    }
}
