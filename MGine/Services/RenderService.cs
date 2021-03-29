using MGine.Core;
using MGine.Enum;
using MGine.Interfaces;
using MGine.Shaders;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace MGine.Services
{
    public class RenderService : IService
    {
        private Engine engine;
        private DeviceContext deviceContext;

        private Dictionary<string, (Buffer Buffer, ConstantBufferType Type)> constantBuffers = new Dictionary<string, (Buffer, ConstantBufferType)>();
        private Dictionary<int, Buffer> vertexShaderSetBuffers = new Dictionary<int, Buffer>();
        private Dictionary<int, Buffer> pixelShaderSetBuffers = new Dictionary<int, Buffer>();


        public Matrix WorldViewProjectionMatrix { get; set; }

        public RenderService(Engine Engine)
        {
            this.engine = Engine;
        }

        public void Dispose() { }

        public void Init()
        {
            this.deviceContext = engine.GraphicsServices.GetService<DeviceContext>();
        }

        public void SetShader(Shader Shader)
        {
            deviceContext.InputAssembler.InputLayout = Shader.InputLayout;
            deviceContext.VertexShader.Set(Shader.VertexShader);
            deviceContext.PixelShader.Set(Shader.PixelShader);
        }

        public void RegisterConstantBuffer(string ConstantBufferName, Buffer ConstantBuffer, ConstantBufferType Type)
        {
            if (constantBuffers.ContainsKey(ConstantBufferName))
                return;

            constantBuffers.Add(ConstantBufferName, (ConstantBuffer, Type));
        }

        public void UpdateConstantBuffer<T>(string ConstantBufferName, ref T Data) where T : struct
        {
            if (constantBuffers.ContainsKey(ConstantBufferName) == false)
                throw new KeyNotFoundException($"Constant Buffer: {ConstantBufferName} has not been registerd.");

            Buffer cBuffer = constantBuffers[ConstantBufferName].Buffer;
            deviceContext.UpdateSubresource(ref Data, cBuffer);
        }

        public void UpdateConstantBuffer<T>(string ConstantBufferName, T[] Data) where T : struct
        {
            if (constantBuffers.ContainsKey(ConstantBufferName) == false)
                throw new KeyNotFoundException($"Constant Buffer: {ConstantBufferName} has not been registerd.");

            Buffer cBuffer = constantBuffers[ConstantBufferName].Buffer;
            deviceContext.UpdateSubresource(Data, cBuffer);
        }

        public void SetConstantBuffer(string ConstantBufferName, int Slot)
        {
            if (constantBuffers.ContainsKey(ConstantBufferName) == false)
                throw new KeyNotFoundException($"Constant Buffer: {ConstantBufferName} has not been registerd.");

            var cBuffer = constantBuffers[ConstantBufferName];
            if (cBuffer.Type == ConstantBufferType.VertexShader && IsBufferSetOnVertexShader(cBuffer.Buffer, Slot) == false)
            {
                UpdateSetVSConstantBuffer(cBuffer.Buffer,Slot);
                deviceContext.VertexShader.SetConstantBuffer(Slot, cBuffer.Buffer);
            }
            else if (cBuffer.Type == ConstantBufferType.PixelShader && IsBufferSetOnPixelShader(cBuffer.Buffer, Slot) == false)
            {
                UpdateSetPSConstantBuffer(cBuffer.Buffer,Slot);
                deviceContext.PixelShader.SetConstantBuffer(Slot, cBuffer.Buffer);
            }
        }

        public void SetConstantBuffer(string ConstantBufferName, Shader Shader)
        {
            SetConstantBuffer(ConstantBufferName, Shader.GetConstantBufferSlot(ConstantBufferName));
        }

        private bool IsBufferSetOnVertexShader(Buffer Buffer, int Slot) =>
            vertexShaderSetBuffers.ContainsKey(Slot) && vertexShaderSetBuffers[Slot] == Buffer;

        private bool IsBufferSetOnPixelShader(Buffer Buffer, int Slot) =>
            pixelShaderSetBuffers.ContainsKey(Slot) && pixelShaderSetBuffers[Slot] == Buffer;

        private void UpdateSetVSConstantBuffer(Buffer Buffer, int Slot)
        {
            if(vertexShaderSetBuffers.ContainsKey(Slot))
                vertexShaderSetBuffers[Slot] = Buffer;
            else
                vertexShaderSetBuffers.Add(Slot, Buffer);
        }

        private void UpdateSetPSConstantBuffer(Buffer Buffer, int Slot)
        {
            if (pixelShaderSetBuffers.ContainsKey(Slot))
                pixelShaderSetBuffers[Slot] = Buffer;
            else
                pixelShaderSetBuffers.Add(Slot, Buffer);
        }
    }
}
