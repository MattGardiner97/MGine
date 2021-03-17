using MGine.Core;
using MGine.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using System.Text;
using System.Threading.Tasks;
using Buffer = SharpDX.Direct3D11.Buffer;
using SharpDX.Direct3D11;
using MGine.Factories;

namespace MGine.Materials
{
    public class StandardMaterial : Material
    {
        private Vector4 colour;

        public Vector4 Colour
        {
            get { return colour; }
            set { colour = value; }
        }

        public StandardMaterial(Engine Engine) : base(Engine)
        {
            Shader = engine.Services.GetService<ShaderFactory>().GetShader<StandardShader>();
        }

        public override void BeginRender(RenderService RenderService)
        {
                Vector4 colour = this.colour;
                RenderService.UpdateConstantBuffer(Constants.ConstantBufferNames.COLOUR_CB,ref colour);
        }

        public override void Dispose()
        {
            
        }

        public override void Init()
        {
            
        }
    }
}
