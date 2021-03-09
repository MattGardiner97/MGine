using MGine.Components;
using MGine.Factories;
using MGine.Interfaces;
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
        //Service references
        private Graphics graphics;
        private Time time;
        private Settings settings;
        private Input input;

        private List<GameObject> gameObjects = new List<GameObject>();

        public ServiceManager<IService> Services { get; private set; }
        public ServiceManager<IDisposable> GraphicsServices { get; private set; }
        public Grouping<Shader, Material> ShaderMaterialGroupings { get; private set; } = new Grouping<Shader, Material>();
        public Grouping<Material, MeshRenderer> MaterialRendererGroupings { get; private set; } = new Grouping<Material, MeshRenderer>();

        public Camera MainCamera { get; set; }
        public bool IsInitialised { get; private set; } = false;

        public Engine(Settings Settings)
        {
            this.Services = new ServiceManager<IService>(this);
            this.GraphicsServices = new ServiceManager<IDisposable>(this);

            this.settings = Settings;
            RegisterDefaultServices();
        }

        private void RegisterDefaultServices()
        {
            Services.RegisterService(settings);
            this.graphics = Services.RegisterService(new Graphics(this, 800, 600));
            this.time = Services.RegisterService<Time>();
            this.input = Services.RegisterService<Input>();
            Services.RegisterService<PrimitiveMeshFactory>();
            Services.RegisterService<ShaderFactory>();
            Services.RegisterService<MaterialFactory>();
            Services.RegisterService<TextureFactory>();
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
            Services.ForAll(service => service.Init());

            GameObject cameraObject = this.CreateGameObject();
            this.MainCamera = cameraObject.AddComponent<Camera>();

            GraphicsServices.GetService<RenderForm>().FormClosed += (_, __) => this.Dispose();

            this.IsInitialised = true;
        }

        public void Dispose()
        {
            Services.ForAll(service => service.Dispose());

        }

        public void Run()
        {
            if (this.IsInitialised == false)
                throw new InvalidOperationException("Engine not initialised.");

            RenderLoop.Run(GraphicsServices.GetService<RenderForm>(), () =>
             {
                 this.Update();
                 this.Render();
             });
        }

        private void Update()
        {
            time.EarlyUpdate();
            input.EarlyUpdate();

            foreach (GameObject gameObject in gameObjects)
                gameObject.Update();


            time.LateUpdate();
            input.LateUpdate();
        }

        public void Render()
        {
            graphics.Render();
        }

        public GameObject CreateGameObject()
        {
            return CreateGameObject(string.Empty);
        }

        public GameObject CreateGameObject(string Name)
        {
            GameObject gameObject = new GameObject(this);
            gameObject.Name = Name;
            gameObjects.Add(gameObject);
            return gameObject;
        }

        public GameObject FindGameObjectByName(string Name)
        {
            for (int i = 0; i < gameObjects.Count; i++)
                if (gameObjects[i].Name == Name)
                    return gameObjects[i];

            return null;
        }
    }
}
