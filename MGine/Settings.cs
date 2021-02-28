using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine
{
    public class Settings
    {
        public string ShaderDirectory { get; set; }

        public Settings()
        {
            ShaderDirectory = Path.Join(AppContext.BaseDirectory, "Shaders");
        }
    }
}
