using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.ShaderDefinitions
{
    public class StandardShaderDefinition : IShaderDefinition
    {
        public InputElement[] GetInputElements()
        {
            return new InputElement[]
            {
                new InputElement("POSITION",0,Format.R32G32B32_Float,0,0,InputClassification.PerVertexData,0)
            };
        }

        public (string Filename, string EntryPoint) GetVertexShaderDetails()
        {
            return ("Standard.hlsl", "VSMain");
        }

        public (string Filename, string EntryPoint) GetPixelShaderDetails()
        {
            return ("Standard.hlsl", "PSMain");
        }

        public int GetInputElementStride()
        {
            return 16;
        }
    }
}
