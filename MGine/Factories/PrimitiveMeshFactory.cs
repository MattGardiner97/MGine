using MGine.Core;
using MGine.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using System.Text;
using System.Threading.Tasks;
using MGine.Interfaces;

namespace MGine.Factories
{
    public class PrimitiveMeshFactory : IService
    {
        private Engine engine;

        public PrimitiveMeshFactory(Engine Engine)
        {
            this.engine = Engine;
        }

        public void Init() { }
        public void Dispose() { }

        public Mesh CreateCube()
        {
            Vector4[] points = new Vector4[]
            {
                //Bottom face
                new Vector4(-0.5f,-0.5f,-0.5f,1), //Rear Bottom left
                new Vector4(-0.5f,-0.5f,0.5f,1), //Front bottom left
                new Vector4(0.5f,-0.5f,0.5f,1), //Front bottom right
                new Vector4(0.5f,-0.5f,-0.5f,1), //Rear bottom right

                //Top Face
                new Vector4(-0.5f,0.5f,-0.5f,1), //Rear top left
                new Vector4(-0.5f,0.5f,0.5f,1), //Front top left
                new Vector4(0.5f,0.5f,0.5f,1), //Front top right
                new Vector4(0.5f,0.5f,-0.5f,1), //Rear top right

                //Left Face
                new Vector4(-0.5f,-0.5f,0.5f,1),
                new Vector4(-0.5f,0.5f,0.5f,1),
                new Vector4(-0.5f,0.5f,-0.5f,1),
                new Vector4(-0.5f,-0.5f,-0.5f,1),

                //Right face
                new Vector4(0.5f,-0.5f,0.5f,1),
                new Vector4(0.5f,0.5f,0.5f,1),
                new Vector4(0.5f,0.5f,-0.5f,1),
                new Vector4(0.5f,-0.5f,-0.5f,1),

                //Back face
                new Vector4(-0.5f,-0.5f,-0.5f,1),
                new Vector4(-0.5f,0.5f,-0.5f,1),
                new Vector4(0.5f,0.5f,-0.5f,1),
                new Vector4(0.5f,-0.5f,-0.5f,1),

                //Front face
                new Vector4(-0.5f,-0.5f,0.5f,1),
                new Vector4(-0.5f,0.5f,0.5f,1),
                new Vector4(0.5f,0.5f,0.5f,1),
                new Vector4(0.5f,-0.5f,0.5f,1),
            };

            int[] indices = new int[]
            {
                //Bottom face
                0,3,1,
                3,2,1,

                //Top
                4,5,6,
                6,7,4,

                //Left
                8,9,10,
                10,11,8,

                //Right
                12,15,13,
                15,14,13,

                //Rear
                16,17,18,
                18,19,16,

                //Front
                23,22,21,
                21,20,23               
            };

            Vector3[] normals = new Vector3[]
            {
                //new Vector3(-1f,-1f,-1f), //Rear Bottom left
                //new Vector3(-1f,-1f,1f), //Front bottom left
                //new Vector3(1f,-1f,1f), //Front bottom right
                //new Vector3(1f,-1f,-1f), //Rear bottom right

                ////Top Face
                //new Vector3(-1f,1f,-1f), //Rear top left
                //new Vector3(-1f,1f,1f), //Front top left
                //new Vector3(1f,1f,1f), //Front top right
                //new Vector3(1f,1f,-1f), //Rear top right

                ////Left Face
                //new Vector3(-1f,-1f,1f),
                //new Vector3(-1f,1f,1f),
                //new Vector3(-1f,1f,-1f),
                //new Vector3(-1f,-1f,-1f),

                ////Right face
                //new Vector3(1f,-1f,1f),
                //new Vector3(1f,1f,1f),
                //new Vector3(1f,1f,-1f),
                //new Vector3(1f,-1f,-1f),

                ////Back face
                //new Vector3(-1f,-1f,-1f),
                //new Vector3(-1f,1f,-1f),
                //new Vector3(1f,1f,-1f),
                //new Vector3(1f,-1f,-1f),

                ////Front face
                //new Vector3(-1f,-1f,1f),
                //new Vector3(-1f,1f,1f),
                //new Vector3(1f,1f,1f),
                //new Vector3(1f,-1f,1f),

                //Bottom face
                new Vector3(0,-1,0),
                new Vector3(0,-1,0),
                new Vector3(0,-1,0),
                new Vector3(0,-1,0),

                //Top face
                new Vector3(0,1,0),
                new Vector3(0,1,0),
                new Vector3(0,1,0),
                new Vector3(0,1,0),

                //Left face
                new Vector3(-1,0,0),
                new Vector3(-1,0,0),
                new Vector3(-1,0,0),
                new Vector3(-1,0,0),

                //Right face
                new Vector3(1,0,0),
                new Vector3(1,0,0),
                new Vector3(1,0,0),
                new Vector3(1,0,0),

                //Back face
                new Vector3(0,0,-1),
                new Vector3(0,0,-1),
                new Vector3(0,0,-1),
                new Vector3(0,0,-1),

                //Front face
                new Vector3(0,0,1),
                new Vector3(0,0,1),
                new Vector3(0,0,1),
                new Vector3(0,0,1)
            };

            Mesh result = new Mesh(engine)
            {
                Vertices = points,
                Indices = indices,
                Normals = normals
            };
            return result;
        }
    }
}
