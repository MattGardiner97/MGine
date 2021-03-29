using MGine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Components
{
    public abstract class Light : Component
    {
        public Action<Light> RebuildSignalled;

        protected Light(GameObject Parent, Engine Engine) : base(Parent, Engine) { }

        protected void SignalRebuild()
        {
            RebuildSignalled?.Invoke(this);
        }
    }
}
