using MGine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Core
{
    public class GameObject
    {
        private Engine engine;
        private Dictionary<Type, Component> components;

        public Transform Transform { get; private set; }

        internal GameObject(Engine Engine)
        {
            this.engine = Engine;
            this.components = new Dictionary<Type, Component>();

            this.Transform = new Transform();
        }

        public void Update()
        {

        }

        public TComponent AddComponent<TComponent>() where TComponent : Component
        {
            if (components.ContainsKey(typeof(TComponent)))
                return default(TComponent);

            var newComponent = (TComponent)Activator.CreateInstance(typeof(TComponent),new object[] { engine });
            components.Add(typeof(TComponent), newComponent);
            return newComponent;
        }

        public TComponent GetComponent<TComponent>() where TComponent : Component
        {
            if (components.ContainsKey(typeof(TComponent)) == false)
                return default(TComponent);

            return (TComponent)(components[typeof(TComponent)]);
        }

        public bool HasComponent<TComponent>() where TComponent : Component
        {
            return (components.ContainsKey(typeof(TComponent)));
        }

    }
}
