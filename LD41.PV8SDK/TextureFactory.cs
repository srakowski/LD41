using Microsoft.Xna.Framework.Graphics;
using PixelVisionRunner;

namespace LD41.PV8SDK
{
    internal class TextureFactory : ITextureFactory
    {
        private GraphicsDevice graphicsDevice;

        public TextureFactory(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
        }

        public bool flip { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public ITexture2D NewTexture2D(int width, int height)
        {
            return new Texture2DAdapter(new Texture2D(this.graphicsDevice, width, height));
        }
    }
}
