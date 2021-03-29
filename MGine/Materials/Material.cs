using MGine.Core;
using MGine.Services;
using MGine.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Materials
{
    public abstract class Material : IDisposable
    {
        protected Engine engine;
        private Shader shader;

        public Shader Shader
        {
            get
            {
                return shader;
            }
            set
            {
                var oldShader = shader;
                shader = value;
                engine.RegisterShaderMaterialGrouping(oldShader, value, this);
            }
        }

        public Material(Engine Engine)
        {
            this.engine = Engine;
        }

        public abstract void Init();
        public abstract void BeginRender(RenderService RenderService);
        public abstract void Dispose();

    }
}
