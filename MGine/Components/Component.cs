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

        public GameObject GameObject { get; private set; }
        public Transform Transform { get { return GameObject.Transform; } }
        public bool Active { get; set; } = true;

        protected Time Time { get; private set; }
        protected Input Input { get; private set; }

        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void Render() { }

        public Component(GameObject Parent, Engine Engine)
        {
            this.GameObject = Parent;
            this.engine = Engine;
            this.Time = engine.Services.GetService<Time>();
            this.Input = engine.Services.GetService<Input>();
        }
    }
}
