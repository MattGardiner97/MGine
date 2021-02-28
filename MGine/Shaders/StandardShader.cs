using MGine.Core;
using MGine.ShaderDefinitions;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Buffer = SharpDX.Direct3D11.Buffer;

namespace MGine.Shaders
{
    public class StandardShader : Shader
    {
        public StandardShader(IShaderDefinition ShaderDefinition, Engine Engine) : base(ShaderDefinition, Engine)
        {
            
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
