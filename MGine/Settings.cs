using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MGine.Interfaces;

namespace MGine
{
    public class Settings : IService
    {
        public string ShaderDirectory { get; set; }

        public Settings()
        {
            ShaderDirectory = Path.Join(AppContext.BaseDirectory, "Shaders");
        }

        public void Init() { }
        public void Dispose() { }
    }
}
