using MGine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Core
{
    public class Time : IService
    {
        private DateTime lastUpdate = DateTime.Now;

        public float DeltaTime { get; private set; }

        public void Dispose() { }
        public void Init() { }

        public void EarlyUpdate()
        {
            DeltaTime = (float)(DateTime.Now - lastUpdate).TotalSeconds;
        }

        public void LateUpdate()
        {
            lastUpdate = DateTime.Now;
        }

    }
}
