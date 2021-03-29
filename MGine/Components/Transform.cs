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
            get => Parent == null ? localPosition : parent.WorldPosition;
            set => LocalPosition = Parent == null ? value : value - parent.WorldPosition;
        }
        public Vector3 EulerAngles
        {
            get { return eulerAngles; }
            set { eulerAngles = value; rotation = Quaternion.RotationYawPitchRoll(value.X, value.Y, value.Z); RecalculateWorld(); }
        }
        public Vector3 Scale { get { return scale; } set { scale = value; RecalculateWorld(); } }
        public Matrix WorldMatrix { get; private set; }

        public Vector3 Forward { get; private set; }
        public Vector3 Backward { get; private set; }
        public Vector3 Up { get; private set; }
        public Vector3 Down { get; private set; }
        public Vector3 Right { get; private set; }
        public Vector3 Left { get; private set; }

        //Events
        public Action Transformed;

        public Transform()
        {
            RecalculateWorld();
        }

        public void Translate(Vector3 Translation)
        {
            this.LocalPosition += Translation;
        }

        private void RecalculateWorld()
        {
            WorldMatrix = Matrix.Identity *
                Matrix.Scaling(Scale) *
                Matrix.RotationX(eulerAngles.X * MathF.PI / 180) *
                Matrix.RotationY(eulerAngles.Y * MathF.PI / 180) *
                Matrix.RotationZ(eulerAngles.Z * MathF.PI / 180) *
                Matrix.Translation(WorldPosition);

            //Forward and backward are flipped for some reason
            Forward = -WorldMatrix.Forward;
            Backward = -WorldMatrix.Backward;
            Left = WorldMatrix.Left;
            Right = WorldMatrix.Right;
            Up = WorldMatrix.Up;
            Down = WorldMatrix.Down;

            Transformed?.Invoke();
        }
    }
}
