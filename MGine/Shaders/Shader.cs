using SharpDX.Direct3D11;
using System;
using SharpDX.D3DCompiler;
using Device = SharpDX.Direct3D11.Device;
using MGine.Core;
using MGine.Exceptions;
using MGine.Services;
using MGine.Shaders.ShaderDefinitions;
using System.Collections.Generic;

namespace MGine.Shaders
{
    public abstract class Shader : IDisposable
    {
        private Dictionary<string, int> constantBufferSlots = new Dictionary<string, int>();

        protected Engine engine;

        public InputLayout InputLayout { get; private set; }
        public int InputElementStride { get; private set; }
        public VertexShader VertexShader { get; private set; }
        public PixelShader PixelShader { get; private set; }

        public CompilationResult VertexShaderCompilationResult { get; private set; }
        public CompilationResult PixelShaderCompilationResult { get; private set; }

        public abstract void BeginRender(RenderService RenderService);
        public abstract void Init(RenderService RenderService);

        public Shader(ShaderDefinition ShaderDefinition, Engine Engine)
        {
            this.engine = Engine;

            VertexShaderCompilationResult = CompileVertexShader(ShaderDefinition.GetVertexShaderDetails().Filename, ShaderDefinition.GetVertexShaderDetails().EntryPoint);
            PixelShaderCompilationResult = CompilePixelShader(ShaderDefinition.GetPixelShaderDetails().Filename, ShaderDefinition.GetPixelShaderDetails().EntryPoint);

            if (VertexShaderCompilationResult.HasErrors)
                throw new ShaderCompilationException(VertexShaderCompilationResult.Message);
            if (PixelShaderCompilationResult.HasErrors)
                throw new ShaderCompilationException(PixelShaderCompilationResult.Message);

            this.VertexShader = new VertexShader(engine.GraphicsServices.GetService<Device>(), VertexShaderCompilationResult.Bytecode);
            this.PixelShader = new PixelShader(engine.GraphicsServices.GetService<Device>(), PixelShaderCompilationResult.Bytecode);
            InputLayout = new InputLayout(
                engine.GraphicsServices.GetService<Device>(),
                ShaderSignature.GetInputSignature(VertexShaderCompilationResult.Bytecode),
                ShaderDefinition.GetInputElements());
            this.InputElementStride = ShaderDefinition.GetInputElementStride();

            string[] cbNames = ShaderDefinition.GetConstantBufferNames();
            for(int i = 0; i < cbNames.Length;i++)
            {
                constantBufferSlots.Add(cbNames[i], i);
            }
        }

        public int GetConstantBufferSlot(string ConstantBufferName)
        {
            return constantBufferSlots[ConstantBufferName];
        }

        private CompilationResult CompileVertexShader(string Filename, string EntryPoint)
        {
            string path = System.IO.Path.Join(new string[] { engine.Services.GetService<Settings>().ShaderDirectory, Filename });
#if DEBUG
            return SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(path, EntryPoint, "vs_5_0", ShaderFlags.Debug);
#else
            return SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(path, EntryPoint, "vs_5_0");
#endif
        }

        private CompilationResult CompilePixelShader(string Filename, string EntryPoint)
        {
            string path = System.IO.Path.Join(new string[] { engine.Services.GetService<Settings>().ShaderDirectory, Filename });
#if DEBUG
            return SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(path, EntryPoint, "ps_5_0", ShaderFlags.Debug);
#else
            return SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(path, EntryPoint,"ps_5_0");
#endif
        }

        public virtual void Dispose()
        {
            VertexShader?.Dispose();
            PixelShader?.Dispose();

            VertexShaderCompilationResult?.Dispose();
            PixelShaderCompilationResult?.Dispose();
        }
    }
}
