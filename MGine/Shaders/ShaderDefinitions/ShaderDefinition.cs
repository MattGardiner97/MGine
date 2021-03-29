using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Shaders.ShaderDefinitions
{
    public abstract class ShaderDefinition
    {
        public abstract InputElement[] GetInputElements();
        public abstract (string Filename, string EntryPoint) GetVertexShaderDetails();
        public abstract (string Filename, string EntryPoint) GetPixelShaderDetails();
        public abstract string[] GetConstantBufferNames();

        public int GetInputElementStride()
        {
            var inputElements = GetInputElements();

            int result = 0;

            foreach (var element in inputElements)
            {
                switch (element.Format)
                {
                    case Format.R32G32B32A32_Float:
                        result += 16;
                        break;
                    case Format.R32G32B32_Float:
                        result += 12;
                        break;
                    default:
                        throw new ArgumentException($"Format: {element.Format} is not supported");
                }

            }

            return result;
        }

    }
}
