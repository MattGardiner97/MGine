using MGine.ShaderDefinitions;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.D3DCompiler;
using Device = SharpDX.Direct3D11.Device;
using SharpDX.Direct3D11;
using MGine.Core;

namespace MGine.Shaders
{
    public abstract class Shader : IDisposable
    {
        private Engine engine;

        public InputLayout InputLayout { get; private set; }
        public int InputElementStride { get; private set; }
        public VertexShader VertexShader { get; private set; }
        public PixelShader PixelShader { get; private set; }

        public CompilationResult VertexShaderCompilationResult { get; private set; }
        public CompilationResult PixelShaderCompilationResult { get; private set; }

        public Shader(IShaderDefinition ShaderDefinition, Engine Engine)
        {
            this.engine = Engine;

            VertexShaderCompilationResult = CompileVertexShader(ShaderDefinition.GetVertexShaderDetails().Filename, ShaderDefinition.GetVertexShaderDetails().EntryPoint);
            PixelShaderCompilationResult = CompilePixelShader(ShaderDefinition.GetPixelShaderDetails().Filename, ShaderDefinition.GetPixelShaderDetails().EntryPoint);

            if (VertexShaderCompilationResult.HasErrors)
                throw new ShaderCompilationException(VertexShaderCompilationResult.Message);
            if (PixelShaderCompilationResult.HasErrors)
                throw new ShaderCompilationException(PixelShaderCompilationResult.Message);

            this.VertexShader = new VertexShader(engine.GetService<Device>(), VertexShaderCompilationResult.Bytecode);
            this.PixelShader = new PixelShader(engine.GetService<Device>(), PixelShaderCompilationResult.Bytecode);
            InputLayout = new InputLayout(
                engine.GetService<Device>(), 
                ShaderSignature.GetInputSignature(VertexShaderCompilationResult.Bytecode), 
                ShaderDefinition.GetInputElements());
            this.InputElementStride = ShaderDefinition.GetInputElementStride();
        }

        private CompilationResult CompileVertexShader(string Filename, string EntryPoint)
        {
            string path = System.IO.Path.Join(new string[] { engine.GetService<Settings>().ShaderDirectory, Filename });
#if DEBUG
            return SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(path, EntryPoint, "vs_5_0",ShaderFlags.Debug);
#else
            return SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(path, EntryPoint, "vs_5_0");
#endif
        }

        private CompilationResult CompilePixelShader(string Filename, string EntryPoint)
        {
            string path = System.IO.Path.Join(new string[] { engine.GetService<Settings>().ShaderDirectory, Filename });
#if DEBUG
            return SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(path, EntryPoint, "ps_5_0",ShaderFlags.Debug);
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
