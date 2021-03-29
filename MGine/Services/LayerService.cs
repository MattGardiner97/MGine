using MGine.Core;
using MGine.Enum;
using MGine.Interfaces;
using MGine.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Services
{
    public class LayerService : IService
    {
        private Engine engine;
        private Dictionary<string, Layer> lookupDictionary = new Dictionary<string, Layer>();

        public Layer Default { get; private set; }
        public Layer Light { get; private set; }

        public LayerService(Engine Engine)
        {
            this.engine = Engine;
        }

        public void Init() 
        {
            this.Default = RegisterLayer(Constants.LayerNames.DEFAULT);
            this.Light = RegisterLayer(Constants.LayerNames.LIGHT);
        }
        public void Dispose() { }

        public Layer RegisterLayer(string Name)
        {
            if (lookupDictionary.ContainsKey(Name))
                throw new ArgumentException($"Layer '{Name}' has already been registered.");

            Layer newLayer = new Layer(Name, lookupDictionary.Count);
            lookupDictionary.Add(Name, newLayer);
            return newLayer;
        }

        public Layer FindByName(string Name)
        {
            Layer result = null;
            lookupDictionary.TryGetValue(Name, out result);
            return result;
        }


    }
}
