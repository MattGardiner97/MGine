﻿using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Shaders.ShaderDefinitions
{
    public class TextureShaderDefinition : ShaderDefinition
    {
        public override string[] GetConstantBufferNames()
        {
            throw new NotImplementedException();
        }

        public override InputElement[] GetInputElements()
        {
            return new InputElement[]
            {
                new InputElement("POSITION",0,SharpDX.DXGI.Format.R32G32B32A32_Float,0,0),
                new InputElement("NORMAL",0,SharpDX.DXGI.Format.R32G32B32_Float,16,0),
                new InputElement("TEXCOORD",0,SharpDX.DXGI.Format.R32G32_Float,28,0)
            };
        }

        public override (string Filename, string EntryPoint) GetPixelShaderDetails()
        {
            return ("Texture.hlsl", "PSMain");
        }

        public override (string Filename, string EntryPoint) GetVertexShaderDetails()
        {
            return ("Texture.hlsl", "VSMain");
        }
    }
}
