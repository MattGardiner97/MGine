using MGine.Core;
using MGine.ShaderDefinitions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Shaders
{
    public class ShaderManager
    {
        private Engine engine;

        private Dictionary<Type, Shader> shaders = new Dictionary<Type, Shader>();

        public IReadOnlyList<Shader> AllShaders { get; private set; }

        public ShaderManager(Engine Engine)
        {
            this.engine = Engine;
        }

        public void RegisterDefaultShaders()
        {
            RegisterShaderDefinition<StandardShader,StandardShaderDefinition>();
        }

        public void RegisterShaderDefinition<TShader,TShaderDefinition>() where TShader: Shader where TShaderDefinition : IShaderDefinition
        {
            if (shaders.ContainsKey(typeof(TShaderDefinition)))
                return;

            var shaderDefinition = Activator.CreateInstance<TShaderDefinition>();
            var newShader = (TShader)Activator.CreateInstance(typeof(TShader),new object[] { shaderDefinition, engine });
            shaders.Add(typeof(TShader), newShader);

            AllShaders = new ReadOnlyCollection<Shader>(shaders.Values.ToArray());
        }

        public TShader GetShaderByDefintion<TShader>() where TShader : Shader
        {
            if (shaders.ContainsKey(typeof(TShader)) == false)
                return null;

            return (TShader)shaders[typeof(TShader)];
        }

    }
}
