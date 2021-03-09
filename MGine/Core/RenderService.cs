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

        private Buffer[] currentBuffersInSlots = new Buffer[10]; //Index 0 = Buffer Slot 1

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

        public void UpdateSubresource<T>(ref T Data, int VertexShaderSlot) where T : struct
        {
            if (VertexShaderSlot < 2 || VertexShaderSlot > 10)
                throw new ArgumentException("VertexShaderSlot must be between 2 and 10 inclusive.");
            if (currentBuffersInSlots[VertexShaderSlot - 2] == null)
                throw new ArgumentNullException($"No Buffers are bound in slot {VertexShaderSlot}.");

            Buffer buffer = currentBuffersInSlots[VertexShaderSlot -2];
            deviceContext.UpdateSubresource(ref Data, buffer);
        }

        public void SetConstantBuffer(int Slot, Buffer ConstantBuffer)
        {
            if (Slot < 2)
            {
                if (Slot >= 0)
                    throw new ArgumentException("Slot 0 and 1 are reserved.");
                else
                    throw new ArgumentException("Slot must be greater than 0.");
            }

            currentBuffersInSlots[Slot - 2] = ConstantBuffer;
            deviceContext.VertexShader.SetConstantBuffer(Slot, ConstantBuffer);

        }
    }
}
