using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine
{
    public static class Constants
    {
     
        public static class ConstantBufferNames
        {
            public const string PER_FRAME_CB = "PerFrameCB";
            public const string PER_OBJECT_CB = "PerObjectCB";
            public const string POINT_LIGHT_CB = "PointLightCB";
            public const string DIRECTIONAL_LIGHT_CB = "DirectionalLightCB";

            public const string STANDARD_MATERIAL_CB = "StandardMaterialCB";
        }

        public static class LayerNames
        {
            public const string DEFAULT = "Default";
            public const string LIGHT = "Light";
        }

        public static class MaxLightCounts
        {
            public const int POINT = 32;
            public const int DIRECTIONAL = 8;
        }

    }
}
