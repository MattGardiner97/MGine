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
using MGine.Services;
using MGine.Enum;
using MGine.Shaders.ShaderStructures;

using PointLightStructure = MGine.Shaders.ShaderStructures.PointLightStructure;

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
        private RenderForm renderForm;
        private RasterizerState renderState;

        private Buffer perFrameBuffer;
        private Buffer perObjectBuffer;
        private Buffer PointLightBuffer;
        private Buffer DirectionalLightBuffer;

        private PointLightStructure[] pointLights = new PointLightStructure[Constants.MaxLightCounts.POINT];
        private DirectionalLightStructure[] directionalLights = new DirectionalLightStructure[Constants.MaxLightCounts.DIRECTIONAL];

        private int width;
        private int height;
        private float aspectRatio;

        private RenderService renderService;
        private LightService lightService;

        public Graphics(Engine Engine, int Width, int Height)
        {
            this.engine = Engine;
            this.width = Width;
            this.height = Height;
            this.aspectRatio = (float)width / height;

            renderForm = new RenderForm()
            {
                ClientSize = new System.Drawing.Size(Width, Height)
            };
        }

        public void Dispose()
        {
            //Use reflection to dispose all IDisposable private fields
            var fields = typeof(Graphics).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            foreach (var fieldInfo in fields)
            {
                if (fieldInfo.FieldType == typeof(Engine))
                    continue;

                var disposeMethod = fieldInfo.FieldType.GetMethod("Dispose");
                if (disposeMethod != null)
                    disposeMethod.Invoke(fieldInfo.GetValue(this), null);
            }
        }

        public void Init()
        {
            #region Device Setup
            CreateDeviceAndSwapChain();
            deviceContext = device.ImmediateContext;

            swapChain.GetParent<Factory>().MakeWindowAssociation(renderForm.Handle, WindowAssociationFlags.IgnoreAll);

            this.backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
            renderTargetView = new RenderTargetView(device, backBuffer);

            deviceContext.OutputMerger.SetRenderTargets(renderTargetView);

            viewport = new Viewport(0, 0, width, height);
            deviceContext.Rasterizer.SetViewport(viewport);
            deviceContext.Rasterizer.SetScissorRectangle(0, 0, renderForm.ClientSize.Width, renderForm.ClientSize.Height);
            renderState = new RasterizerState(device, new RasterizerStateDescription()
            {
                CullMode = CullMode.Back,
                DepthBias = 0,
                DepthBiasClamp = 0,
                FillMode = FillMode.Solid,
                IsAntialiasedLineEnabled = false,
                IsDepthClipEnabled = true,
                IsFrontCounterClockwise = false,
                IsMultisampleEnabled = true,
                IsScissorEnabled = false,
                SlopeScaledDepthBias = 0
            });
            deviceContext.Rasterizer.State = renderState;

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
            #endregion

            RegisterServices();

            this.renderService = engine.Services.GetService<RenderService>();
            this.lightService = engine.Services.GetService<LightService>();

            renderService.Init();

            //Constant Buffers
            perFrameBuffer = engine.Services.GetService<BufferFactory>().CreateConstantBuffer<PerFrameCB>();
            perObjectBuffer = engine.Services.GetService<BufferFactory>().CreateConstantBuffer<PerObjectCB>();
            PointLightBuffer = engine.Services.GetService<BufferFactory>().CreateConstantBuffer(SharpDX.Utilities.SizeOf<PointLightStructure>() * Constants.MaxLightCounts.POINT);
            DirectionalLightBuffer = engine.Services.GetService<BufferFactory>().CreateConstantBuffer(SharpDX.Utilities.SizeOf<DirectionalLightStructure>() * Constants.MaxLightCounts.DIRECTIONAL);
            
            renderService.RegisterConstantBuffer(Constants.ConstantBufferNames.PER_FRAME_CB, perFrameBuffer, ConstantBufferType.PixelShader);
            renderService.RegisterConstantBuffer(Constants.ConstantBufferNames.POINT_LIGHT_CB, PointLightBuffer, ConstantBufferType.PixelShader);
            renderService.RegisterConstantBuffer(Constants.ConstantBufferNames.DIRECTIONAL_LIGHT_CB, DirectionalLightBuffer, ConstantBufferType.PixelShader);
            renderService.RegisterConstantBuffer(Constants.ConstantBufferNames.PER_OBJECT_CB, perObjectBuffer, ConstantBufferType.VertexShader);

            renderForm.Show();
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
#if DEBUG
            Device.CreateWithSwapChain(SharpDX.Direct3D.DriverType.Hardware, DeviceCreationFlags.Debug | DeviceCreationFlags.BgraSupport, swapChainDescription, out device, out swapChain);
#else
            Device.CreateWithSwapChain(SharpDX.Direct3D.DriverType.Hardware, DeviceCreationFlags.BgraSupport, swapChainDescription, out device, out swapChain);
#endif
        }

        private void RegisterServices()
        {
            engine.GraphicsServices.RegisterService(renderForm);
            engine.GraphicsServices.RegisterService(device);
            engine.GraphicsServices.RegisterService(deviceContext);
        }

        public void Render()
        {
            UpdatePerFrameBuffer();

            UpdateLightBuffers();

            //Clear targets
            deviceContext.ClearDepthStencilView(depthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1, 0);
            deviceContext.ClearRenderTargetView(renderTargetView, Color.Gray);

            //Camera matrix calulcations
            Transform cameraTransform = engine.MainCamera.Transform;
            Matrix viewMatrix = Matrix.LookAtLH(cameraTransform.WorldPosition, (cameraTransform.Forward + cameraTransform.WorldPosition), Vector3.UnitY);
            Matrix projectionMatrix = Matrix.PerspectiveFovLH((float)((Math.PI / 4)), aspectRatio, 0.01f, 200f);

            IEnumerable<Shader> shaders = engine.ShaderMaterialGroupings.Keys;
            foreach (Shader shader in shaders)
            {
                shader.BeginRender(renderService);

                //Set per-frame and light buffers (camera position)
                renderService.SetConstantBuffer(Constants.ConstantBufferNames.PER_FRAME_CB, shader);
                renderService.SetConstantBuffer(Constants.ConstantBufferNames.POINT_LIGHT_CB, shader);
                renderService.SetConstantBuffer(Constants.ConstantBufferNames.DIRECTIONAL_LIGHT_CB, shader);

                IEnumerable<Material> materials = engine.MaterialRendererGroupings.Keys;
                foreach (Material material in materials)
                {
                    material.BeginRender(renderService);

                    IEnumerable<MeshRenderer> meshRenderers = engine.MaterialRendererGroupings[material];
                    foreach (MeshRenderer renderer in meshRenderers)
                    {
                        PerObjectCB perObjectCB = UpdateWorldMatrices(renderer.Transform.WorldMatrix, viewMatrix, projectionMatrix);

                        renderService.SetConstantBuffer(Constants.ConstantBufferNames.PER_OBJECT_CB, shader);
                        renderService.UpdateConstantBuffer(Constants.ConstantBufferNames.PER_OBJECT_CB, ref perObjectCB);

                        deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(renderer.Mesh.VertexBuffer, shader.InputElementStride, 0));
                        deviceContext.InputAssembler.SetIndexBuffer(renderer.Mesh.IndexBuffer, Format.R32_UInt, 0);

                        deviceContext.DrawIndexed(renderer.Mesh.Indices.Length, 0, 0);
                    }
                }
            }

            swapChain.Present(1, PresentFlags.None);
        }

        private PerObjectCB UpdateWorldMatrices(Matrix WorldMatrix, Matrix ViewMatrix, Matrix ProjMatrix)
        {
            Matrix wvpMatrix = WorldMatrix * ViewMatrix * ProjMatrix;
            Matrix worldMatrix = WorldMatrix;

            wvpMatrix.Transpose();
            worldMatrix.Transpose();

            PerObjectCB result = new PerObjectCB(wvpMatrix, worldMatrix);
            return result;
        }

        private void UpdateLightBuffers()
        {
            if (lightService.PointLightUpdated)
            {
                var lightArray = lightService.PointLightStructures;
                renderService.UpdateConstantBuffer(Constants.ConstantBufferNames.POINT_LIGHT_CB, lightArray);
                lightService.ResetPointLight();
            }
            if(lightService.DirectionalLightUpdated)
            {
                var lightArray = lightService.DirectionalLightStructures;
                renderService.UpdateConstantBuffer(Constants.ConstantBufferNames.DIRECTIONAL_LIGHT_CB, lightArray);
                lightService.ResetDirectionalLight();
            }
        }

        private void UpdatePerFrameBuffer()
        {
            PerFrameCB frameCB = new PerFrameCB()
            {
                CameraPosition = engine.MainCamera.Transform.WorldPosition
            };

            renderService.UpdateConstantBuffer(Constants.ConstantBufferNames.PER_FRAME_CB, ref frameCB);
        }
    }
}
