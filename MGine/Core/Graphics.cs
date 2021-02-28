using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Numerics;
using SharpDX;
using System.Threading;

using Device = SharpDX.Direct3D11.Device;
using Resource = SharpDX.Direct3D11.Resource;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;
using MGine.Components;
using MGine.Shaders;
using MGine.Materials;
using MGine.Factories;
using Buffer = SharpDX.Direct3D11.Buffer;
using MGine.Utilities;

namespace MGine.Core
{
    public class Graphics
    {
        private Engine engine;


        private SwapChain swapChain;
        private Device device;
        private DeviceContext deviceContext;
        private RenderTargetView renderTargetView;
        private Texture2D backBuffer;
        private Viewport viewport;
        private Texture2D depthBuffer;
        private DepthStencilView depthStencilView;
        
        private Buffer wvpConstantBuffer;

        private RenderForm renderForm;

        private int width;
        private int height;
        private float aspectRatio;
        private Matrix4x4 viewMatrix;
        private Matrix4x4 projectionMatrix;


        public Graphics(Engine Engine, int Width, int Height, int FrameCount = 2)
        {
            this.engine = Engine;
            this.width = Width;
            this.height = Height;
            this.aspectRatio = (float)width / height;

            renderForm = new RenderForm()
            {
                Width = width,
                Height = height
            };

        }

        public void Dispose()
        {
            swapChain?.Dispose();
            device?.Dispose();
            deviceContext?.Dispose();
            renderTargetView?.Dispose();
        }

        public void Init()
        {
            CreateDeviceAndSwapChain();
            deviceContext = device.ImmediateContext;

            swapChain.GetParent<Factory>().MakeWindowAssociation(renderForm.Handle, WindowAssociationFlags.IgnoreAll);

            this.backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
            renderTargetView = new RenderTargetView(device, backBuffer);

            deviceContext.OutputMerger.SetRenderTargets(renderTargetView);

            viewport = new Viewport(0, 0, width, height);
            deviceContext.Rasterizer.SetViewport(viewport);

            wvpConstantBuffer = new Buffer(
                this.device,
                SharpDX.Utilities.SizeOf<Matrix4x4>(),
                ResourceUsage.Default,
                BindFlags.ConstantBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                SharpDX.Utilities.SizeOf<Matrix4x4>());


            depthBuffer = new Texture2D(device, new Texture2DDescription()
            {
                Format = Format.D32_Float_S8X24_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = renderForm.ClientSize.Width,
                Height = renderForm.ClientSize.Height,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            });
            depthStencilView = new DepthStencilView(device, depthBuffer);
            deviceContext.OutputMerger.SetTargets(depthStencilView, renderTargetView);

            renderForm.Show();
            RegisterServices();
        }

        private void CreateDeviceAndSwapChain()
        {
            SwapChainDescription swapChainDescription = new SwapChainDescription()
            {
                BufferCount = 1,
                Flags = SwapChainFlags.None,
                IsWindowed = true,
                ModeDescription = new ModeDescription(width, height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                OutputHandle = renderForm.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            this.device = null;
            this.swapChain = null;
            Device.CreateWithSwapChain(SharpDX.Direct3D.DriverType.Hardware, DeviceCreationFlags.Debug, swapChainDescription, out device, out swapChain);
        }

        private void RegisterServices()
        {
            engine.RegisterService(renderForm);
            engine.RegisterService(device);
            engine.RegisterService<PrimitiveMeshFactory>();
        }

        public void Render()
        {
            deviceContext.ClearRenderTargetView(renderTargetView, Color.Blue);

            Transform cameraTransform = engine.MainCamera.Transform;
            viewMatrix = Matrix4x4.CreateLookAt(cameraTransform.WorldPosition, -cameraTransform.Forward + cameraTransform.WorldPosition, Vector3.UnitY);
            projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView((float)((Math.PI/4)), aspectRatio, float.Epsilon, 1);
            foreach(Material material in engine.MaterialRendererGroupings.Keys)
            {
                deviceContext.InputAssembler.InputLayout = material.Shader.InputLayout;
                deviceContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
                deviceContext.VertexShader.SetConstantBuffer(0, wvpConstantBuffer);
                deviceContext.VertexShader.Set(material.Shader.VertexShader);
                deviceContext.PixelShader.Set(material.Shader.PixelShader);

                IEnumerable<MeshRenderer> meshRenderers = engine.MaterialRendererGroupings[material];
                foreach(MeshRenderer renderer in meshRenderers)
                {
                    Matrix4x4 wvpMatrix = Matrix4x4.Identity * renderer.GameObject.Transform.WorldMatrix * (viewMatrix * projectionMatrix);
                    wvpMatrix = wvpMatrix.Transpose();
                    deviceContext.UpdateSubresource(ref wvpMatrix, wvpConstantBuffer);

                    deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding()
                    {
                        Buffer = renderer.Mesh.VertexBuffer,
                        Offset = 0,
                        Stride = material.Shader.InputElementStride
                    });

                    deviceContext.Draw(renderer.Mesh.Vertices.Length,0);
                }
            }
            swapChain.Present(1, PresentFlags.None);
        }

    }
}
