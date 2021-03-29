using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Structures
{
    public class LayerGroup
    {
        private int bitField;

        public void AddLayer(Layer Layer)
        {
            ValidateLayer(Layer);

            int mask = 1 << Layer.Index;
            bitField |= mask;
        }

        public void SetLayer(Layer Layer)
        {
            ValidateLayer(Layer);

            bitField = 1 << Layer.Index;
        }

        public void RemoveLayer(Layer Layer)
        {
            ValidateLayer(Layer);

            int mask = ~(1 << Layer.Index);
            bitField &= mask;
        }

        public void ClearLayer(Layer Layer)
        {
            ValidateLayer(Layer);

            bitField = 0;
        }

        private void ValidateLayer(Layer Layer)
        {
            if (Layer.Index > 31)
                throw new ArgumentException($"A maximum of 32 layers are supported. Layer Index must be in range of 0-31 inclusive.");
        }
    }
}
