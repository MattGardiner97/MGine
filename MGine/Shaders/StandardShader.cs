using MGine.Core;
using MGine.ShaderDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Shaders
{
    public class StandardShader : Shader
    {
        public StandardShader(IShaderDefinition ShaderDefinition, Engine Engine) : base(ShaderDefinition, Engine)
        {
        }
    }
}
