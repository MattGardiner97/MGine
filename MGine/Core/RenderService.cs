using MGine.Enum;
using MGine.Shaders;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace MGine.Core
{
    public class RenderService : IDisposable
    {
        private Engine engine;
        private DeviceContext deviceContext;

        private Dictionary<string, (Buffer Buffer, ConstantBufferType Type)> constantBuffers = new Dictionary<string, (Buffer, ConstantBufferType)>();

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

        public void SetConstantBuffer(string ConstantBufferName, int Slot)
        {
            if (constantBuffers.ContainsKey(ConstantBufferName) == false)
                throw new KeyNotFoundException($"Constant Buffer: {ConstantBufferName} has not been registerd.");

            var cBuffer = constantBuffers[ConstantBufferName];
            if (cBuffer.Type == ConstantBufferType.VertexShader)
                deviceContext.VertexShader.SetConstantBuffer(Slot, cBuffer.Buffer);
            else
                deviceContext.PixelShader.SetConstantBuffer(Slot, cBuffer.Buffer);
        }
    }
}
