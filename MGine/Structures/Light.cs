using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Structures
{
    public struct Light
    {
        public Vector3 Direction;
        public float Pad;
        public Vector4 Ambient;
        public Vector4 Diffuse;

    }
}
