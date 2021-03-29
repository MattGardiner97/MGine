using MGine.Components;
using MGine.Enum;
using MGine.Services;
using MGine.Structures;
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

        public string Name { get; set; }
        public Transform Transform { get; private set; }
        public Layer Category { get; set; }

        internal GameObject(Engine Engine)
        {
            this.engine = Engine;
            this.components = new Dictionary<Type, Component>();

            this.Transform = new Transform();
            this.Category = engine.Services.GetService<LayerService>().Default;
        }

        public void Update()
        {
            foreach(Component comp in components.Values)
            {
                comp.Update();

            }
        }

        public TComponent AddComponent<TComponent>() where TComponent : Component
        {
            if (components.ContainsKey(typeof(TComponent)))
                throw new ArgumentException($"Component '{typeof(TComponent).FullName}' has already been registered");

            var newComponent = (TComponent)Activator.CreateInstance(typeof(TComponent),new object[] { this, engine});

            if (newComponent is Light)
                engine.Services.GetService<LightService>().RegisterLight(newComponent as Light);

            components.Add(typeof(TComponent), newComponent);

            newComponent.Start();

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
