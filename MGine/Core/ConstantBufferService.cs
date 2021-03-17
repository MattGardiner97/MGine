using MGine.Interfaces;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Core
{
    public class ConstantBufferService : IService
    {
        private Engine engine;
        private DeviceContext deviceContext;

        public ConstantBufferService(Engine Engine)
        {
            this.engine = Engine;
            this.deviceContext = engine.GraphicsServices.GetService<DeviceContext>();
        }

        public void Dispose() { }
        public void Init() { }


    }
}
