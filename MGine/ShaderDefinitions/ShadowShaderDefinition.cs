using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.ShaderDefinitions
{
    public class ShadowShaderDefinition : ShaderDefinition
    {
        public override InputElement[] GetInputElements()
        {
            throw new Exception();

            return new InputElement[]
            {
                new InputElement("POSITION",0,Format.R32G32B32A32_Float,0,0),
                new InputElement("NORMAL",0,Format.R32G32B32_Float,16,0,InputClassification.PerVertexData,0)
            };
        }

        public override (string Filename, string EntryPoint) GetVertexShaderDetails()
        {
            return ("Standard.hlsl", "VSMain");
        }

        public override (string Filename, string EntryPoint) GetPixelShaderDetails()
        {
            return ("Standard.hlsl", "PSMain");
        }
    }
}
