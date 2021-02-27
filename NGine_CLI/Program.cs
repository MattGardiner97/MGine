using MGine;
using MGine.Components;
using MGine.Core;
using MGine.Factories;
using MGine.Materials;
using MGine.ShaderDefinitions;
using MGine.Shaders;
using MGine.Structures;
using SharpDX.Windows;
using System;
using Vector3 = System.Numerics.Vector3;

namespace NGine_CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Engine engine = new Engine();
            engine.Initialise();

            GameObject gObject = engine.CreateGameObject();
            MeshRenderer mr = gObject.AddComponent<MeshRenderer>();
            mr.Mesh = engine.GetService<PrimitiveMeshFactory>().CreateCube();
            StandardMaterial standardMaterial = new StandardMaterial(engine);
            mr.Material = standardMaterial;

            engine.Run();
        }
    }
}
