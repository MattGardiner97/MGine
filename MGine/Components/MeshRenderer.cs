using MGine.Core;
using MGine.Materials;
using MGine.Shaders;
using MGine.Structures;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Components
{
    public class MeshRenderer : Component
    {
        private Mesh mesh;
        private Material material;

        public Mesh Mesh
        {
            get { return mesh; }
            set { mesh = value; if (mesh != null) RebuildGraphicsState(); }
        }

        public Material Material
        {
            get { return material; }
            set
            {
                engine.RegisterMaterialRendererGrouping(material, value, this);
                material = value;
            }
        }

        public MeshRenderer(GameObject Parent, Engine Engine) : base(Parent, Engine)
        {

        }

        public void RebuildGraphicsState()
        {

        }
    }
}
