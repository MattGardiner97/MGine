using MGine.Core;
using MGine.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Materials
{
    public class StandardMaterial : Material
    {
        //private StandardShader shader;

        public Vector4 Color { get; set; }

        public StandardMaterial(Engine Engine) : base(Engine)
        {
            Shader = Engine.GetService<ShaderManager>().GetShader<StandardShader>();
        }
    }
}
