using MGine.Core;
using SharpDX.Direct3D11;
using SharpDX;

using Buffer = SharpDX.Direct3D11.Buffer;
using MGine.Services;
using MGine.Shaders.ShaderDefinitions;
using MGine.Factories;
using MGine.Shaders.ShaderStructures;

namespace MGine.Shaders
{
    public class StandardShader : Shader
    {
        private Buffer materialConstantBuffer;

        public StandardShader(ShaderDefinition ShaderDefinition, Engine Engine) : base(ShaderDefinition, Engine)
        {
            
        }

        public override void Init(RenderService RenderService)
        {
            materialConstantBuffer = engine.Services.GetService<BufferFactory>().CreateConstantBuffer<StandardMaterialStructure>();
            RenderService.RegisterConstantBuffer(Constants.ConstantBufferNames.STANDARD_MATERIAL_CB, materialConstantBuffer,Enum.ConstantBufferType.PixelShader);
        }

        public override void BeginRender(RenderService RenderService)
        {
            RenderService.SetShader(this);
            RenderService.SetConstantBuffer(Constants.ConstantBufferNames.STANDARD_MATERIAL_CB,this);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
