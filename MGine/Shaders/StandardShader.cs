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
    public class StandardShader : Shader
    {
        private Buffer colourConstantBuffer;

        public StandardShader(IShaderDefinition ShaderDefinition, Engine Engine) : base(ShaderDefinition, Engine)
        {
            
        }

        public override void Init()
        {
            colourConstantBuffer = new Buffer(
                engine.GraphicsServices.GetService<Device>(),
                SharpDX.Utilities.SizeOf<Vector4>(),
                ResourceUsage.Default,
                BindFlags.ConstantBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                SharpDX.Utilities.SizeOf<Vector4>());
        }

        public override void BeginRender(RenderService RenderService)
        {
            RenderService.SetShader(this);
            RenderService.SetConstantBuffer(2, colourConstantBuffer);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
