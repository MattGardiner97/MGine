using MGine.Core;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Components
{
    public class DirectionalLight : Light
    {
        private Vector4 ambient;
        private Vector4 diffuse;
        private Vector4 specular;

        public Vector4 Ambient { get => ambient; set { ambient = value; SignalRebuild(); } }
        public Vector4 Diffuse { get => diffuse; set { diffuse = value; SignalRebuild(); } }
        public Vector4 Specular { get => specular; set { specular = value; SignalRebuild(); } }

        public Shaders.ShaderStructures.DirectionalLightStructure LightStructure
        {
            get => new Shaders.ShaderStructures.DirectionalLightStructure(ambient, diffuse, specular, this.GameObject.Transform.Forward);
        }

        public DirectionalLight(GameObject Parent, Engine Engine) : base(Parent, Engine)
        {
            Parent.Transform.Transformed += this.SignalRebuild;
        }

        
    }
}
