using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Core
{
    public class Time
    {
        private DateTime lastUpdate = DateTime.Now;

        public double DeltaTime { get; private set; }

        public void EarlyUpdate()
        {
            DeltaTime = (DateTime.Now - lastUpdate).TotalSeconds;
        }

        public void LateUpdate()
        {
            lastUpdate = DateTime.Now;
        }

    }
}
