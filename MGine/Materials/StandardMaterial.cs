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
using MGine.Services;
using MGine.Shaders.ShaderStructures;

namespace MGine.Materials
{
    public class StandardMaterial : Material
    {
        private StandardMaterialStructure materialStructure;

        private Vector4 ambient;
        private Vector4 diffuse;
        private Vector4 specular;

        private bool rebuildRequired = true;

        public Vector4 Ambient { get => ambient; set { ambient = value; rebuildRequired = true; } }
        public Vector4 Diffuse { get => diffuse; set { diffuse = value; rebuildRequired = true; } }
        public Vector4 Specular { get => specular; set { specular = value; rebuildRequired = true; } }

        public StandardMaterial(Engine Engine) : base(Engine)
        {
            Shader = engine.Services.GetService<ShaderFactory>().GetShader<StandardShader>();
        }

        public override void BeginRender(RenderService RenderService)
        {
            if (rebuildRequired)
                Rebuild(RenderService);
        }

        public override void Dispose()
        {

        }

        public override void Init()
        {

        }

        private void Rebuild(RenderService RenderService)
        {
            materialStructure = new StandardMaterialStructure()
            {
                ambient = this.ambient,
                diffuse = this.diffuse,
                specular = this.specular
            };
            RenderService.UpdateConstantBuffer(Constants.ConstantBufferNames.STANDARD_MATERIAL_CB, ref materialStructure);
            rebuildRequired = false;
        }
    }
}
