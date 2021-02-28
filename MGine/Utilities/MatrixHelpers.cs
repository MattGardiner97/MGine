using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Utilities
{
    public static class MatrixHelpers
    {
        public static Matrix4x4 Transpose(this Matrix4x4 Matrix)
        {
            Matrix4x4 result = new Matrix4x4();
            //First col
            result.M11 = Matrix.M11;
            result.M21 = Matrix.M12;
            result.M31 = Matrix.M13;
            result.M41 = Matrix.M14;

            //Second col
            result.M12 = Matrix.M21;
            result.M22 = Matrix.M22;
            result.M32 = Matrix.M23;
            result.M42 = Matrix.M24;

            //Third col
            result.M13 = Matrix.M31;
            result.M23 = Matrix.M32;
            result.M33 = Matrix.M33;
            result.M43 = Matrix.M34;

            //Fourth col
            result.M14 = Matrix.M41;
            result.M24 = Matrix.M42;
            result.M34 = Matrix.M43;
            result.M44 = Matrix.M44;

            return result;
        }

    }
}
