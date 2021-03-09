using MGine.Core;
using MGine.Factories;
using MGine.Shaders;
using MGine.Structures;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Materials
{
    public class TextureMaterial : Material
    {
        private SamplerState sampleState;
        private TextureDefinition textureDefinition;

        public ShaderResourceView TextureResourceView { get; private set; }
        public TextureDefinition TextureDefinition
        {
            get { return textureDefinition;}
            set { textureDefinition = value;Rebuild(); }
        }

        public TextureMaterial(Engine Engine) : base(Engine)
        {
            this.Shader = engine.Services.GetService<ShaderFactory>().GetShader<TextureShader>();
        }

        public override void BeginRender(RenderService RenderService)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            TextureResourceView?.Dispose();
            sampleState?.Dispose();
        }

        public override void Init()
        {
            Rebuild();
        }

        public void Rebuild()
        {
            Dispose();
        }
    }
}
