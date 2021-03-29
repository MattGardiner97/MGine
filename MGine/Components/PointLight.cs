using MGine.Core;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Components
{
    public class PointLight : Light
    {
        private Vector4 ambient;
        private Vector4 diffuse;
        private Vector4 specular;
        private float range;
        private Vector3 attenuation;

        public Vector4 Ambient { get => ambient; set { ambient = value; SignalRebuild(); } }
        public Vector4 Diffuse { get => diffuse; set { diffuse = value; SignalRebuild(); } }
        public Vector4 Specular { get => specular; set { specular = value; SignalRebuild(); } }
        public float Range { get => range; set { range = value; SignalRebuild(); } }
        public Vector3 Attenuation { get => attenuation; set { attenuation = value; SignalRebuild(); } }

        public Shaders.ShaderStructures.PointLightStructure LightStructure
        {
            get => new Shaders.ShaderStructures.PointLightStructure(ambient, diffuse, specular, this.GameObject.Transform.WorldPosition, range, attenuation);
        }

        public PointLight(GameObject Parent, Engine Engine) : base(Parent, Engine)
        {
            Parent.Transform.Transformed += this.SignalRebuild;
        }

        
    }
}
