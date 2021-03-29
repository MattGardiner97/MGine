using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Shaders.ShaderStructures
{
    public struct PerObjectCB
    {
        public Matrix WorldViewProjection;
        public Matrix World;

        public PerObjectCB(Matrix WorldViewProjection,Matrix World)
        {
            this.WorldViewProjection = WorldViewProjection;
            this.World = World;
        }
    }
}
