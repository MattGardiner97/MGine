using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Structures
{
    public struct PerObjectCB
    {
        public PerObjectCB(Matrix WorldViewProjection,Matrix World)
        {
            this.WorldViewProjection = WorldViewProjection;
            this.World = World;
        }

        public Matrix WorldViewProjection;
        public Matrix World;
    }
}
