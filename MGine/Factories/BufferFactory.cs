using MGine.Core;
using MGine.Interfaces;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace MGine.Factories
{
    public class BufferFactory : IService
    {
        private Engine engine;
        private List<Buffer> createBuffers = new List<Buffer>();

        public BufferFactory(Engine Engine)
        {
            this.engine = Engine;
        }

        public void Init() { }
        public void Dispose()
        {
            foreach (Buffer buffer in createBuffers)
                buffer.Dispose();
        }

        public Buffer CreateConstantBuffer<TBufferStruct>() where TBufferStruct : struct
        {
            int size = SharpDX.Utilities.SizeOf<TBufferStruct>();
            return CreateConstantBuffer(size);
        }

        public Buffer CreateConstantBuffer(int StructSize)
        {
            Buffer result = new Buffer(
                engine.GraphicsServices.GetService<Device>(),
                StructSize,
                ResourceUsage.Default,
                BindFlags.ConstantBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                StructSize
                );

            createBuffers.Add(result);

            return result;
        }
    }
}
