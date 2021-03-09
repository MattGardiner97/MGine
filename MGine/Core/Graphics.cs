using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using SharpDX;
using System.Threading;

using Device = SharpDX.Direct3D11.Device;
using Resource = SharpDX.Direct3D11.Resource;
using MGine.Components;
using MGine.Shaders;
using MGine.Materials;
using MGine.Factories;
using Buffer = SharpDX.Direct3D11.Buffer;
using MGine.Structures;
using MGine.Interfaces;

namespace MGine.Core
{
    public class Graphics : IService
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
        private SwapChainDescription swapChainDescription;

        private Light light;
        private Buffer perObjectBuffer;
        private Buffer perFrameBuffer;

        private RenderForm renderForm;

        private int width;
        private int height;
        private float aspectRatio;

        private RenderService renderService;


        public Graphics(Engine Engine, int Width, int Height)
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
            backBuffer?.Dispose();
            depthBuffer?.Dispose();
            depthStencilView?.Dispose();
            perObjectBuffer?.Dispose();
        }

        public void Init()
        {
            CreateDeviceAndSwapChain();
            deviceContext = device.ImmediateContext;

            swapChain.GetParent<Factory>().MakeWindowAssociation(renderForm.Handle, WindowAssociationFlags.IgnoreAll);

            this.backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
            renderTargetView = new RenderTargetView(device, backBuffer);
            //this.backBuffer.Dispose();

            deviceContext.OutputMerger.SetRenderTargets(renderTargetView);

            viewport = new Viewport(0, 0, width, height);
            deviceContext.Rasterizer.SetViewport(viewport);
            deviceContext.Rasterizer.SetScissorRectangle(0, 0, renderForm.ClientSize.Width, renderForm.ClientSize.Height);
            deviceContext.Rasterizer.State = new RasterizerState(device, new RasterizerStateDescription()
            {
                CullMode = CullMode.Back,
                DepthBias = 0,
                DepthBiasClamp = 0,
                FillMode = SharpDX.Direct3D11.FillMode.Solid,
                IsAntialiasedLineEnabled = false,
                IsDepthClipEnabled = true,
                IsFrontCounterClockwise = false,
                IsMultisampleEnabled = true,
                IsScissorEnabled = false,
                SlopeScaledDepthBias = 0
            });

            perObjectBuffer = new Buffer(
                this.device,
                SharpDX.Utilities.SizeOf<PerObjectCB>(),
                ResourceUsage.Default,
                BindFlags.ConstantBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                SharpDX.Utilities.SizeOf<PerObjectCB>());

            perFrameBuffer = new Buffer(
                this.device,
                SharpDX.Utilities.SizeOf<PerFrameCB>(),
                ResourceUsage.Default,
                BindFlags.ConstantBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                SharpDX.Utilities.SizeOf<PerFrameCB>());


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
            deviceContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

            renderForm.Show();
            RegisterServices();

            renderService.Init();

            light = new Light()
            {
                Direction = new Vector3(0.5f,-1,1),
                Ambient = new Vector4(0.2f,0.2f,0.2f,1),
                Diffuse = new Vector4(1f,1f,1f,1f)
            };
            PerFrameCB perFrameStruct = new PerFrameCB()
            {
                Light = light
            };
            deviceContext.UpdateSubresource(ref perFrameStruct, perFrameBuffer);
        }

        private void CreateDeviceAndSwapChain()
        {
            swapChainDescription = new SwapChainDescription()
            {
                BufferCount = 1,
                Flags = SwapChainFlags.None,
                IsWindowed = true,
                ModeDescription = new ModeDescription(renderForm.ClientSize.Width, renderForm.ClientSize.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                OutputHandle = renderForm.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            this.device = null;
            this.swapChain = null;
            Device.CreateWithSwapChain(SharpDX.Direct3D.DriverType.Hardware, DeviceCreationFlags.Debug | DeviceCreationFlags.BgraSupport, swapChainDescription, out device, out swapChain);
        }

        private void RegisterServices()
        {
            engine.GraphicsServices.RegisterService(renderForm);
            engine.GraphicsServices.RegisterService(device);
            engine.GraphicsServices.RegisterService(deviceContext);
            this.renderService = engine.GraphicsServices.RegisterService<RenderService>();
        }

        public void Render()
        {
            deviceContext.ClearDepthStencilView(depthStencilView, DepthStencilClearFlags.Depth, 1, 0);
            deviceContext.ClearRenderTargetView(renderTargetView, Color.Gray);

            Transform cameraTransform = engine.MainCamera.Transform;
            Matrix viewMatrix = Matrix.LookAtLH(cameraTransform.WorldPosition, (cameraTransform.Forward + cameraTransform.WorldPosition), Vector3.UnitY);
            Matrix projectionMatrix = Matrix.PerspectiveFovLH((float)((Math.PI / 4)), aspectRatio, 0.01f, 200f);

            deviceContext.PixelShader.SetConstantBuffer(1, perFrameBuffer);


            IEnumerable<Shader> shaders = engine.ShaderMaterialGroupings.Keys;
            foreach (Shader shader in shaders)
            {
                shader.BeginRender(renderService);
                deviceContext.VertexShader.SetConstantBuffer(0, perObjectBuffer);


                IEnumerable<Material> materials = engine.MaterialRendererGroupings.Keys;
                foreach (Material material in materials)
                {
                    material.BeginRender(renderService);


                    IEnumerable<MeshRenderer> meshRenderers = engine.MaterialRendererGroupings[material];
                    foreach (MeshRenderer renderer in meshRenderers)
                    {
                        Matrix wvpMatrix = Matrix.Identity * renderer.GameObject.Transform.WorldMatrix * viewMatrix * projectionMatrix;
                        wvpMatrix.Transpose();
                        Matrix worldMatrix = renderer.GameObject.Transform.WorldMatrix;
                        worldMatrix.Transpose();
                        PerObjectCB perObjectCB = new PerObjectCB()
                        {
                            WorldViewProjection = wvpMatrix,
                            World = worldMatrix
                        };
                        deviceContext.UpdateSubresource(ref perObjectCB, perObjectBuffer);

                        deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding()
                        {
                            Buffer = renderer.Mesh.VertexBuffer,
                            Offset = 0,
                            Stride = material.Shader.InputElementStride
                        });
                        deviceContext.InputAssembler.SetIndexBuffer(renderer.Mesh.IndexBuffer, Format.R32_UInt, 0);

                        deviceContext.DrawIndexed(renderer.Mesh.Indices.Length, 0, 0);
                    }
                }

            }

            swapChain.Present(1, PresentFlags.None);
        }



    }
}
