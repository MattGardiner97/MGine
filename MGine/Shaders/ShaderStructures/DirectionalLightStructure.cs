using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Shaders.ShaderStructures
{
    public struct DirectionalLightStructure
    {
        public Vector4 ambient;
        public Vector4 diffuse;
        public Vector4 specular;
        public Vector3 direction;
        public float range;

        public DirectionalLightStructure(Vector4 Ambient, Vector4 Diffuse, Vector4 Specular, Vector3 Direction)
        {
            this.ambient = Ambient;
            this.diffuse = Diffuse;
            this.specular = Specular;
            this.direction= Direction;
            this.range = 0;
        }
    }
}
