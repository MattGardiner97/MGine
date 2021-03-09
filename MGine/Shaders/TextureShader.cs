using MGine.Core;
using MGine.ShaderDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Shaders
{
    public class TextureShader : Shader
    {
        public TextureShader(IShaderDefinition ShaderDefinition, Engine Engine) : base(ShaderDefinition, Engine)
        {
        }

        public override void BeginRender(RenderService RenderService)
        {
            throw new NotImplementedException();
        }

        public override void Init()
        {
            throw new NotImplementedException();
        }
    }
}
