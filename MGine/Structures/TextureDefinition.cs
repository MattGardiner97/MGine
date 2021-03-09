using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Structures
{
    public class TextureDefinition
    {
        public string Filename { get; private set; }
        public string Name { get; private set; }

        public TextureDefinition(string Filename, string Name)
        {
            this.Filename = Filename;
            this.Name = Name;
        }

    }
}
