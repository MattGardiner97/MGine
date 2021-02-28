using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MGine.Core;
using SharpDX.Direct3D11;

using Buffer = SharpDX.Direct3D11.Buffer ;

namespace MGine.Structures
{
    public class Mesh
    {
        private Vector4[] vertices = new Vector4[0];
        private Engine engine;

        public Buffer VertexBuffer { get; private set; }

        public Vector4[] Vertices
        {
            get { return vertices; }
            set { vertices = value; Rebuild(); }
        }

        public Mesh(Engine Engine)
        {
            this.engine = Engine;
        }

        public void Rebuild()
        {
            VertexBuffer = Buffer.Create(engine.GetService<Device>(), BindFlags.VertexBuffer, vertices);
        }


        

    }
}
