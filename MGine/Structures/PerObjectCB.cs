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
        public Matrix WorldViewProjection;
        public Matrix World;
    }
}
