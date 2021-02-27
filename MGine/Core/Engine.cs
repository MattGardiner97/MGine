using MGine.Components;
using MGine.Materials;
using MGine.Shaders;
using MGine.Structures;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Core
{
    public class Engine
    {
        private Dictionary<Type, object> services = new Dictionary<Type, object>();
        private List<GameObject> gameObjects = new List<GameObject>();

        public Grouping<Shader, Material> ShaderMaterialGroupings { get; private set; } = new Grouping<Shader, Material>();
        public Grouping<Material, MeshRenderer> MaterialRendererGroupings { get; private set; } = new Grouping<Material, MeshRenderer>();

        public Camera MainCamera { get; set; }

        //Service references
        private Graphics graphics;
        private Time time;

        public bool IsInitialised { get; private set; } = false;

        public Engine()
        {
            RegisterDefaultService();
        }

        public TService RegisterService<TService>()
        {
            if (services.ContainsKey(typeof(TService)))
                return default(TService);

            TService service;
            if (typeof(TService).GetConstructor(new Type[] { typeof(Engine) }) != null)
                service = (TService)Activator.CreateInstance(typeof(TService), new object[] { this });
            else
                service = Activator.CreateInstance<TService>();

            services.Add(typeof(TService), service);

            return service;
        }

        public TService RegisterService<TService>(TService Service)
        {
            if (services.ContainsKey(typeof(TService)))
                throw new ArgumentOutOfRangeException($"{typeof(TService).FullName} has not been registered.");

            services.Add(typeof(TService), Service);

            return Service;
        }

        public TService GetService<TService>()
        {
            if (services.ContainsKey(typeof(TService)) == false)
                throw new ArgumentOutOfRangeException($"{typeof(TService).FullName} has not been registered.");

            return (TService)services[typeof(TService)];
        }

        private void RegisterDefaultService()
        {
            this.graphics = RegisterService(new Graphics(this, 800, 600));
            this.time = RegisterService<Time>();
            RegisterService<ShaderManager>();
        }

        public void RegisterShaderMaterialGrouping(Shader OldShader, Shader NewShader, Material Material)
        {
            ShaderMaterialGroupings.Remove(OldShader, Material);
            ShaderMaterialGroupings.Add(NewShader, Material);

        }

        public void RegisterMaterialRendererGrouping(Material OldMaterial, Material NewMaterial, MeshRenderer MeshRenderer)
        {
            MaterialRendererGroupings.Remove(OldMaterial, MeshRenderer);
            MaterialRendererGroupings.Add(NewMaterial, MeshRenderer);
        }

        public void Initialise()
        {
            GetService<Graphics>().Init();
            GetService<ShaderManager>().RegisterDefaultShaders();

            GameObject cameraObject = this.CreateGameObject();
            this.MainCamera = cameraObject.AddComponent<Camera>();

            this.IsInitialised = true;
        }

        public void Run()
        {
            if (this.IsInitialised == false)
                throw new InvalidOperationException("Engine not initialised.");

            using (var renderLoop = new RenderLoop(GetService<RenderForm>()))
            {
                while (renderLoop.NextFrame())
                {
                    this.Update();
                    this.Render();
                }
            }
        }

        private void Update()
        {
            time.EarlyUpdate();
            GetService<RenderForm>().Text = $"{1 / time.DeltaTime}";
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Update();
            }
            time.LateUpdate();
        }

        public void Render()
        {
            graphics.Render();
            graphics.EndRender();
        }

        public GameObject CreateGameObject()
        {
            GameObject gameObject = new GameObject(this);
            gameObjects.Add(gameObject);
            return gameObject;
        }
    }
}
