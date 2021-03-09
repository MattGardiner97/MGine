using MGine;
using MGine.Components;
using MGine.Core;
using MGine.Factories;
using MGine.Materials;
using MGine.ShaderDefinitions;
using MGine.Shaders;
using MGine.Structures;
using SharpDX;
using SharpDX.Windows;
using System;
using SharpDX.DirectInput;

namespace NGine_CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Settings settings = new Settings()
            {
                ShaderDirectory = @"C:\Users\Matt\Source\Repos\MGine\MGine\HLSL"
            };

            Engine engine = new Engine(settings);
            engine.Initialise();

            engine.MainCamera.GameObject.AddComponent<CameraMovementScript>();

            GameObject gObject = engine.CreateGameObject("Test");
            gObject.Transform.LocalPosition = new Vector3(0, 0, 0f);
            MeshRenderer mr = gObject.AddComponent<MeshRenderer>();
            mr.Mesh = engine.Services.GetService<PrimitiveMeshFactory>().CreateCube();
            StandardMaterial standardMaterial = engine.Services.GetService<MaterialFactory>().GetMaterial<StandardMaterial>();
            mr.Material = standardMaterial;
            ((StandardMaterial)mr.Material).Colour = new Vector4(1, 0, 0,1);

            //Random r = new Random();

            //for (int i = 0; i < 1; i++)
            //{
            //    GameObject gObject = engine.CreateGameObject("Test");
            //    gObject.Transform.LocalPosition = new Vector3(0, 0, 0f);
            //    MeshRenderer mr = gObject.AddComponent<MeshRenderer>();
            //    mr.Mesh = engine.Services.GetService<PrimitiveMeshFactory>().CreateCube();
            //    StandardMaterial standardMaterial = engine.Services.GetService<MaterialFactory>().GetMaterial<StandardMaterial>();
            //    mr.Material = standardMaterial;
            //    switch(r.Next(0,4))
            //    {
            //        case 0:
            //            ((StandardMaterial)mr.Material).Colour = new Vector4(1, 0, 0, 1);
            //            break;
            //        case 1:
            //            ((StandardMaterial)mr.Material).Colour = new Vector4(0, 1, 0, 1);
            //            break;
            //        case 2:
            //            ((StandardMaterial)mr.Material).Colour = new Vector4(0, 0, 1, 1);
            //            break;
            //        case 3:
            //            ((StandardMaterial)mr.Material).Colour = new Vector4(1, 1, 0, 1);
            //            break;
            //    }
            //    gObject.AddComponent<TestScript>();
            //}

            engine.MainCamera.Transform.LocalPosition = new Vector3(0, 00, -5);

            engine.Run();
        }
    }

    public class TestScript : Component
    {
        private static Random r = new Random();

        private const int MaxDist = 80;

        private int speed;
        private Vector3 target;
        private Vector3 direction;
        private float targetDistance;
        private float distanceTravelled;


        public TestScript(GameObject Parent, Engine Engine) : base(Parent, Engine) { }

        public override void Start()
        {
            UpdateValues();
        }

        public override void Update()
        {
            if (Vector3.NearEqual(target, Transform.WorldPosition, Vector3.One / 2) || distanceTravelled > targetDistance)
                UpdateValues();

            Transform.WorldPosition += direction * speed * Time.DeltaTime;
            distanceTravelled += (direction * speed * Time.DeltaTime).Length();
        }

        private void UpdateValues()
        {
            speed = r.Next(2, 20);
            float x = r.Next(-MaxDist, MaxDist + 1);
            float y = r.Next(-MaxDist, MaxDist + 1);
            float z = r.Next(-MaxDist, MaxDist + 1);
            target = new Vector3(x, y, z);
            direction = target - Transform.WorldPosition;
            direction.Normalize();

            targetDistance = Vector3.Distance(target, Transform.WorldPosition);
            distanceTravelled = 0;
        }
    }

    public class CameraMovementScript : Component
    {
        private float speed = 3f;
        private float rotateAmount = 10f;

        public CameraMovementScript(GameObject Parent, Engine Engine) : base(Parent, Engine)
        {
        }

        private DateTime lastUpdate;

        public override void Update()
        {
            if (DateTime.Now - lastUpdate > TimeSpan.FromSeconds(0.5))
            {
                engine.GraphicsServices.GetService<RenderForm>().Text = ((int)(1 / Time.DeltaTime)).ToString();
                lastUpdate = DateTime.Now;
            }

            if (Input.GetKeyDown(Key.LeftShift))
                speed = 10f;
            if (Input.GetKeyUp(Key.LeftShift))
                speed = 3f;

            if (Input.GetKey(Key.W))
                this.Transform.LocalPosition += Vector3.ForwardLH * speed * Time.DeltaTime;
            if (Input.GetKey(Key.S))
                this.Transform.LocalPosition += Vector3.BackwardLH * speed * Time.DeltaTime;
            if (Input.GetKey(Key.A))
                this.Transform.LocalPosition += Vector3.Left * speed * Time.DeltaTime;
            if (Input.GetKey(Key.D))
                this.Transform.LocalPosition += Vector3.Right * speed * Time.DeltaTime;

            if (Input.GetKey(Key.Space))
                this.Transform.LocalPosition += Vector3.Up * speed * Time.DeltaTime;
            if (Input.GetKey(Key.LeftControl))
                this.Transform.LocalPosition += Vector3.Down * speed * Time.DeltaTime;

            if (Input.GetKeyDown(Key.Up))
                this.Transform.EulerAngles += Vector3.UnitX * rotateAmount;
            if (Input.GetKeyDown(Key.Down))
                this.Transform.EulerAngles -= Vector3.UnitX * rotateAmount;
        }
    }
}
