using MGine.Core;
using MGine.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Materials
{
    public class Material
    {
        private Engine engine;
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

    }
}
