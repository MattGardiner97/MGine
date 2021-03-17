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
        private RenderForm renderForm;
        private RasterizerState renderState;

        //Shadow mapping
        private Texture2D shadowDepthBuffer;
        private DepthStencilView shadowDepthStencilView;
        private ShaderResourceView shadowShaderResourceView;
        private SamplerState shadowComparisonSamplerState;
        private RasterizerState shadowRenderState;
        private Viewport shadowViewport;


        private Light light;
        private Buffer perObjectBuffer;
        private Buffer perFrameBuffer;
        private Buffer lightBuffer;
        private Matrix lightMatrix;

        private int width;
        private int height;
        private float aspectRatio;
        private int shadowMapWidth = 800;
        private int shadowMapHeight = 800;

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
            //Use reflection to dispose all IDisposable private fields
            var fields = typeof(Graphics).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            foreach(var fieldInfo in fields)
            {
                if (fieldInfo.FieldType == typeof(Engine))
                    continue;

                var disposeMethod  = fieldInfo.FieldType.GetMethod("Dispose");
                if (disposeMethod!= null)
                    disposeMethod.Invoke(fieldInfo.GetValue(this),null);
            }
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

            shadowDepthBuffer = new Texture2D(device, new Texture2DDescription()
            {
                Format = Format.R24G8_Typeless,
                BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = new SampleDescription() { Count = 1 },
                Height = shadowMapHeight,
                Width = shadowMapWidth,
            });

            shadowDepthStencilView = new DepthStencilView(device, shadowDepthBuffer, new DepthStencilViewDescription()
            {
                Format = Format.D24_UNorm_S8_UInt,
                Dimension = DepthStencilViewDimension.Texture2D,
                Texture2D = new DepthStencilViewDescription.Texture2DResource() { MipSlice = 0}
            });

            shadowShaderResourceView = new ShaderResourceView(device, shadowDepthBuffer, new ShaderResourceViewDescription()
            {
                Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D,
                Format = Format.R24_UNorm_X8_Typeless,
                Texture2D = new ShaderResourceViewDescription.Texture2DResource() { MipLevels = 1 }
            });

            shadowComparisonSamplerState = new SamplerState(this.device, new SamplerStateDescription()
            {
                AddressU = TextureAddressMode.Border,
                AddressV = TextureAddressMode.Border,
                AddressW = TextureAddressMode.Border,
                BorderColor = new SharpDX.Mathematics.Interop.RawColor4(1, 1, 1, 1),
                MinimumLod = 0.0f,
                MaximumLod = float.MaxValue,
                MipLodBias = 0.0f,
                MaximumAnisotropy = 0,
                ComparisonFunction = Comparison.LessEqual,
                Filter = Filter.ComparisonMinMagMipPoint

            });

            shadowRenderState = new RasterizerState(device, new RasterizerStateDescription()
            {
                CullMode = CullMode.Front,
                FillMode = FillMode.Solid,
                IsDepthClipEnabled = true
            });

            lightMatrix = Matrix.Translation(0, 2, -2) 
                * Matrix.LookAtLH(new Vector3(0, 2, -2), Vector3.Zero, Vector3.UnitY) 
                * Matrix.PerspectiveFovLH(MathF.PI / 2, 1f, 12f, 50f);
            lightMatrix.Transpose();
            lightBuffer = Buffer.Create(device, ref lightMatrix, new BufferDescription()
            {
                BindFlags = BindFlags.ConstantBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = SharpDX.Utilities.SizeOf<Matrix>(),
                StructureByteStride = SharpDX.Utilities.SizeOf<Matrix>(),
                Usage = ResourceUsage.Default
            });
            deviceContext.UpdateSubresource(ref lightMatrix, lightBuffer);
            shadowViewport = new Viewport(0, 0, shadowMapWidth, shadowMapHeight, 0f, 1f);

            renderForm.Show();
            RegisterServices();

            renderService.Init();



            //Constant Buffers
            perFrameBuffer = engine.Services.GetService<BufferFactory>().CreateConstantBuffer<PerFrameCB>();
            perObjectBuffer = engine.Services.GetService<BufferFactory>().CreateConstantBuffer<PerObjectCB>();

            renderService.RegisterConstantBuffer(Constants.ConstantBufferNames.PER_FRAME_CB, perFrameBuffer, Enum.ConstantBufferType.PixelShader);
            renderService.RegisterConstantBuffer(Constants.ConstantBufferNames.PER_OBJECT_CB, perObjectBuffer, Enum.ConstantBufferType.VertexShader);
            renderService.RegisterConstantBuffer(Constants.ConstantBufferNames.LIGHT_CB, lightBuffer, Enum.ConstantBufferType.VertexShader);

            light = new Light()
            {
                Direction = new Vector3(0.5f, -1, 1),
                Ambient = new Vector4(0.2f, 0.2f, 0.2f, 1),
                Diffuse = new Vector4(1f, 1f, 1f, 1f)
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
            //Render shadows
            RenderShadows();

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

                //Set per-frame buffer (ambient lighting?)
                renderService.SetConstantBuffer(Constants.ConstantBufferNames.PER_FRAME_CB, 0);
                renderService.SetConstantBuffer(Constants.ConstantBufferNames.LIGHT_CB, 2);
                renderService.UpdateConstantBuffer(Constants.ConstantBufferNames.LIGHT_CB, ref lightMatrix);

                IEnumerable<Material> materials = engine.MaterialRendererGroupings.Keys;
                foreach (Material material in materials)
                {
                    material.BeginRender(renderService);


                    IEnumerable<MeshRenderer> meshRenderers = engine.MaterialRendererGroupings[material];
                    foreach (MeshRenderer renderer in meshRenderers)
                    {
                        Matrix wvpMatrix = renderer.GameObject.Transform.WorldMatrix * viewMatrix * projectionMatrix;
                        wvpMatrix.Transpose();
                        Matrix worldMatrix = renderer.GameObject.Transform.WorldMatrix;
                        worldMatrix.Transpose();
                        PerObjectCB perObjectCB = new PerObjectCB(wvpMatrix, worldMatrix);
                        renderService.SetConstantBuffer(Constants.ConstantBufferNames.PER_OBJECT_CB, 1);
                        renderService.UpdateConstantBuffer(Constants.ConstantBufferNames.PER_OBJECT_CB, ref perObjectCB);

                        deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(renderer.Mesh.VertexBuffer,shader.InputElementStride,0));
                        deviceContext.InputAssembler.SetIndexBuffer(renderer.Mesh.IndexBuffer, Format.R32_UInt, 0);

                        deviceContext.DrawIndexed(renderer.Mesh.Indices.Length, 0, 0);
                    }
                }

            }

            swapChain.Present(1, PresentFlags.None);
        }

        public void RenderShadows()
        {
            deviceContext.ClearDepthStencilView(shadowDepthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1, 0);

            deviceContext.OutputMerger.SetRenderTargets(shadowDepthStencilView, renderTargetView: null);

            deviceContext.Rasterizer.State = shadowRenderState;
            deviceContext.Rasterizer.SetViewport(shadowViewport);

            Shader shadowShader = engine.Services.GetService<ShaderFactory>().GetShader<ShadowShader>();
            shadowShader.BeginRender(renderService);
            deviceContext.VertexShader.SetConstantBuffer(1, lightBuffer);
            deviceContext.UpdateSubresource(ref lightMatrix, lightBuffer);
            
            foreach(var _shader in engine.ShaderMaterialGroupings.Keys)
            {
                foreach(var _material in engine.ShaderMaterialGroupings[_shader])
                {
                    foreach(var _renderer in engine.MaterialRendererGroupings[_material])
                    {
                        deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_renderer.Mesh.VertexBuffer, shadowShader.InputElementStride, 0));
                        deviceContext.InputAssembler.SetIndexBuffer(_renderer.Mesh.IndexBuffer, Format.R32_UInt, 0);
                        deviceContext.InputAssembler.InputLayout = shadowShader.InputLayout;
                    }
                }
            }
        }



    }
}
