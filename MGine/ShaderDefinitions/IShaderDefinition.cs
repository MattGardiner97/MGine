using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.ShaderDefinitions
{
    public interface IShaderDefinition
    {
        InputElement[] GetInputElements();
        (string Filename,string EntryPoint) GetVertexShaderDetails();
        (string Filename, string EntryPoint) GetPixelShaderDetails();
        int GetInputElementStride();
    }
}
