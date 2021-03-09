using MGine.Core;
using MGine.Interfaces;
using MGine.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Factories
{
    public class MaterialFactory : IService
    {
        private Engine engine;

        public MaterialFactory(Engine Engine)
        {
            engine = Engine;
        }

        public void Init() { }
        public void Dispose() { }

        public TMaterial GetMaterial<TMaterial>() where TMaterial : Material
        {
            TMaterial result = (TMaterial)Activator.CreateInstance(typeof(TMaterial), new object[] { engine });
            result.Init();
            return result;
        }

    }
}
