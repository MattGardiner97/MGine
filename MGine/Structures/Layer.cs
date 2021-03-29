using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Structures
{
    public class Layer
    {
        public string Name { get; private set; }
        public int Index { get; private set; }

        public Layer(string Name,int LayerIndex)
        {
            this.Name = Name;
            this.Index = LayerIndex;
        }
    }
}
