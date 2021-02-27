using MGine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Components
{
    public abstract class Component
    {
        protected Engine engine;

        public GameObject Parent { get; private set; }
        public Transform Transform { get { return Parent.Transform; } }
        public bool Active { get; set; } = true;

        public void Update() { }
        public void Render() { }

        public Component(GameObject Parent, Engine Engine)
        {
            this.Parent = Parent;
            this.engine = Engine;
        }
    }
}
