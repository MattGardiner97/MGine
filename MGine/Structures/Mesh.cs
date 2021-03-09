using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using System.Text;
using System.Threading.Tasks;
using MGine.Core;
using SharpDX.Direct3D11;

using Buffer = SharpDX.Direct3D11.Buffer;

namespace MGine.Structures
{
    public class Mesh
    {
        private Vector4[] vertices;
        private int[] indices;
        private Vector3[] normals;

        private Engine engine;

        public Buffer VertexBuffer { get; private set; }
        public Buffer IndexBuffer { get; private set; }

        public Vector4[] Vertices
        {
            get { return vertices; }
            set { vertices = value; Rebuild(); }
        }

        public int[] Indices
        {
            get { return indices; }
            set { indices = value; Rebuild(); }
        }

        public Vector3[] Normals
        {
            get { return normals; }
            set { normals = value; Rebuild(); }
        }

        public Mesh(Engine Engine)
        {
            this.engine = Engine;
        }

        public void Rebuild()
        {
            if (vertices == null ||
                vertices.Length == 0 ||
                indices == null ||
                indices.Length == 0 ||
                normals == null ||
                normals.Length == 0 || 
                vertices.Length != normals.Length)
                return;

            VertexInputElement[] elements = new VertexInputElement[vertices.Length];
            for (int i = 0; i < elements.Length; i++)
            {
                elements[i] = new VertexInputElement()
                {
                    Position = vertices[i],
                    Normal = normals[i]
                };
            }

            VertexBuffer = Buffer.Create(engine.GraphicsServices.GetService<Device>(), BindFlags.VertexBuffer, elements);
            IndexBuffer = Buffer.Create(engine.GraphicsServices.GetService<Device>(), BindFlags.IndexBuffer, indices);
        }




    }
}
