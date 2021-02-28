using MGine.Core;
using MGine.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Factories
{
    public class PrimitiveMeshFactory
    {
        private Engine engine;

        public PrimitiveMeshFactory(Engine Engine)
        {
            this.engine = Engine;
        }

        public Mesh CreateCube()
        {
            Vector4[] points = new Vector4[]
            {
                new Vector4(-0.5f,0,0,1),
                new Vector4(0,0.5f,0,1),
                new Vector4(0.5f,0,0,1)

                //new Vector3(-0.5f,-0.5f,-0.5f), //Rear Bottom left
                //new Vector3(-0.5f,-0.5f,0.5f), //Front bottom left
                //new Vector3(0.5f,-0.5f,0.5f), //Front bottom right
                //new Vector3(0.5f,-0.5f,-0.5f), //Rear bottom right

                //new Vector3(-0.5f,0.5f,-0.5f), //Rear top left
                //new Vector3(-0.5f,0.5f,0.5f), //Front top left
                //new Vector3(0.5f,0.5f,0.5f), //Front top right
                //new Vector3(0.5f,0.5f,-0.5f) //Front rear right
            };

            Mesh result = new Mesh(engine)
            {
                Vertices = points
            };
            return result;
        }

    }
}
