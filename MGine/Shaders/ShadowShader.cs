using MGine.Core;
using MGine.ShaderDefinitions;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using System.Text;
using System.Threading.Tasks;

using Buffer = SharpDX.Direct3D11.Buffer;

namespace MGine.Shaders
{
    public class ShadowShader : Shader
    {
        public ShadowShader(ShaderDefinition ShaderDefinition, Engine Engine) : base(ShaderDefinition, Engine)
        {
            
        }

        public override void Init(RenderService RenderService)
        {
            
        }

        public override void BeginRender(RenderService RenderService)
        {
            RenderService.SetShader(this);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
