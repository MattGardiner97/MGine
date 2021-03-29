using MGine.Core;
using MGine.Interfaces;
using MGine.Services;
using MGine.Shaders;
using MGine.Shaders.ShaderDefinitions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MGine.Factories
{
    public class ShaderFactory : IService
    {
        private Engine engine;

        private Dictionary<Type, Shader> shaders = new Dictionary<Type, Shader>();

        public IReadOnlyList<Shader> AllShaders { get; private set; }

        public ShaderFactory(Engine Engine)
        {
            this.engine = Engine;
        }

        public void Init()
        {
            RegisterShaderDefinition<StandardShader,StandardShaderDefinition>();
        }

        public void RegisterShaderDefinition<TShader,TShaderDefinition>() where TShader: Shader where TShaderDefinition : ShaderDefinition
        {
            if (shaders.ContainsKey(typeof(TShader)))
                return;

            var shaderDefinition = Activator.CreateInstance<TShaderDefinition>();
            var newShader = (TShader)Activator.CreateInstance(typeof(TShader),new object[] { shaderDefinition, engine });
            newShader.Init(engine.Services.GetService<RenderService>());

            shaders.Add(typeof(TShader), newShader);
            AllShaders = new ReadOnlyCollection<Shader>(shaders.Values.ToArray());
        }

        public TShader GetShader<TShader>() where TShader : Shader
        {
            if (shaders.ContainsKey(typeof(TShader)) == false)
                return null;

            return (TShader)shaders[typeof(TShader)];
        }

        public void Dispose()
        {
            foreach (var shader in shaders.Values)
                shader.Dispose();
        }
    }
}
