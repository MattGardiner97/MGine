using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Components
{
    public class Transform
    {
        private Transform parent;
        private Vector3 localPosition;
        private Quaternion rotation;
        private Vector3 eulerAngles = Vector3.Zero;
        private Vector3 scale = Vector3.One;

        public Transform Parent { get { return parent; } set { parent = value; RecalculateWorld(); } }
        public Vector3 LocalPosition { get { return localPosition; } set { localPosition = value; RecalculateWorld(); } }
        public Vector3 WorldPosition
        {
            get
            {
                if (Parent == null)
                    return LocalPosition;
                return Parent.WorldPosition;
            }
            set
            {
                if (Parent == null)
                    LocalPosition = value;
                else
                    LocalPosition = value - Parent.WorldPosition;
            }
        }
        public Vector3 EulerAngles
        {
            get { return eulerAngles; }
            set { eulerAngles = value; rotation = Quaternion.RotationYawPitchRoll(value.X, value.Y, value.Z); RecalculateWorld(); }
        }
        public Vector3 Scale { get { return scale; } set { scale = value; RecalculateWorld(); } }
        public Matrix WorldMatrix { get; private set; }

        public Vector3 Forward { get; private set; }

        public Transform()
        {
            RecalculateWorld();
        }

        private void RecalculateWorld()
        {
            this.Forward = RotateVector(this.rotation, Vector3.UnitZ);



            WorldMatrix = Matrix.Identity *
                Matrix.Scaling(Scale) *
                Matrix.RotationX(eulerAngles.X * MathF.PI / 180) *
                Matrix.RotationY(eulerAngles.Y * MathF.PI / 180) *
                Matrix.RotationZ(eulerAngles.Z * MathF.PI / 180) *
                Matrix.Translation(WorldPosition);

            ;
        }

        private Vector3 RotateVector(Quaternion rotation, Vector3 point)
        {
            float num1 = rotation.X * 2f;
            float num2 = rotation.Y * 2f;
            float num3 = rotation.Z * 2f;
            float num4 = rotation.X * num1;
            float num5 = rotation.Y * num2;
            float num6 = rotation.Z * num3;
            float num7 = rotation.X * num2;
            float num8 = rotation.X * num3;
            float num9 = rotation.Y * num3;
            float num10 = rotation.W * num1;
            float num11 = rotation.W * num2;
            float num12 = rotation.W * num3;
            Vector3 vector3;
            vector3.X = (float)((1.0 - ((double)num5 + (double)num6)) * (double)point.X + ((double)num7 - (double)num12) * (double)point.Y + ((double)num8 + (double)num11) * (double)point.Z);
            vector3.Y = (float)(((double)num7 + (double)num12) * (double)point.X + (1.0 - ((double)num4 + (double)num6)) * (double)point.Y + ((double)num9 - (double)num10) * (double)point.Z);
            vector3.Z = (float)(((double)num8 - (double)num11) * (double)point.X + ((double)num9 + (double)num10) * (double)point.Y + (1.0 - ((double)num4 + (double)num5)) * (double)point.Z);
            return vector3;
        }

    }
}
