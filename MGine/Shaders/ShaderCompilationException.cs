using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Shaders
{
    public class ShaderCompilationException : Exception
    {
        public ShaderCompilationException(string Message) : base(Message)
        {
        }
    }
}
