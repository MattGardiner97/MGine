using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Shaders.ShaderStructures
{
    public struct PointLightStructure
    {
        public Vector4 ambient;
        public Vector4 diffuse;
        public Vector4 specular;
        public Vector3 position;
        public float range;
        public Vector3 attenuation;
        public float pad0;

        public PointLightStructure(Vector4 Ambient, Vector4 Diffuse, Vector4 Specular, Vector3 Position, float Range, Vector3 Attenuation)
        {
            this.ambient = Ambient;
            this.diffuse = Diffuse;
            this.specular = Specular;
            this.position = Position;
            this.range = Range;
            this.attenuation = Attenuation;
            this.pad0 = 0;
        }
    }
}
