using MGine.Core;
using MGine.Interfaces;
using MGine.Structures;
using SharpDX.Direct3D11;
using SharpDX.WIC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Factories
{
    public class TextureFactory : IService
    {
        private Engine engine;
        private Dictionary<string, Texture2D> cache = new Dictionary<string, Texture2D>();

        public TextureFactory(Engine Engine)
        {
            engine = Engine;
        }

        public void Init() { }
        public void Dispose()
        {
            foreach (Texture2D tex in cache.Values)
                tex?.Dispose();
        }

        public void CacheDefinitions(IEnumerable<TextureDefinition> TextureDefinitions)
        {
            foreach(TextureDefinition definition in TextureDefinitions)
            {
                BitmapSource bitmapSource = LoadBitmap(definition.Filename);
                Texture2D textureResult = CreateTexture2DFromBitmap(bitmapSource);
                if (cache.ContainsKey(definition.Filename))
                    throw new InvalidOperationException($"Texture cache already contains entry with name: {definition.Name}");
                cache.Add(definition.Name, textureResult);
            }
        }

        public Texture2D GetTextureByName(string Name)
        {
            Texture2D result = null;
            cache.TryGetValue(Name, out result);

            return result;
        }

        private BitmapSource LoadBitmap(string filename)
        {
            ImagingFactory2 factory = new ImagingFactory2();

            var bitmapDecoder = new BitmapDecoder(
                factory,
                filename,
                DecodeOptions.CacheOnDemand
                );

            var formatConverter = new FormatConverter(factory);

            formatConverter.Initialize(
                bitmapDecoder.GetFrame(0),
                PixelFormat.Format32bppPRGBA,
                BitmapDitherType.None,
                null,
                0.0,
                BitmapPaletteType.Custom);

            return formatConverter;
        }

        private Texture2D CreateTexture2DFromBitmap(BitmapSource bitmapSource)
        {
            Device device = engine.GraphicsServices.GetService<Device>();

            // Allocate DataStream to receive the WIC image pixels
            int stride = bitmapSource.Size.Width * 4;
            using (var buffer = new SharpDX.DataStream(bitmapSource.Size.Height * stride, true, true))
            {
                // Copy the content of the WIC to the buffer
                bitmapSource.CopyPixels(stride, buffer);
                return new Texture2D(device, new Texture2DDescription()
                {
                    Width = bitmapSource.Size.Width,
                    Height = bitmapSource.Size.Height,
                    ArraySize = 1,
                    BindFlags = BindFlags.ShaderResource,
                    Usage = ResourceUsage.Immutable,
                    CpuAccessFlags = CpuAccessFlags.None,
                    Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
                    MipLevels = 1,
                    OptionFlags = ResourceOptionFlags.None,
                    SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                }, new SharpDX.DataRectangle(buffer.DataPointer, stride));
            }
        }

        
    }
}