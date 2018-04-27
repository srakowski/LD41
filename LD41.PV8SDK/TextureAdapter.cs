using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelVisionRunner;
using PixelVisionSDK;
using System;
using System.IO;
using System.Linq;

namespace LD41.PV8SDK
{
    class Texture2DAdapter : ITexture2D
    {
        private Texture2D texture;

        public int width { get { return texture.Width; } }

        public int height { get { return texture.Height; } }

        public Texture2DAdapter(Texture2D texture)
        {
            this.texture = texture;
            FlipTexture();
        }

        public IColor GetPixel(int x, int y)
        {
            return this.GetPixels(x, y, 1, 1)[0];
        }

        public IColor[] GetPixels()
        {
            var data = new Color[texture.Width * texture.Height];
            texture.GetData(data);
            return data
                .Select(c => new ColorAdapter(c) as IColor)
                .ToArray();
        }

        public IColor[] GetPixels(int x, int y, int width, int height)
        {
            var data = new Color[width * height];
            texture.GetData(0, new Rectangle(x, y, width, height), data, 0, data.Length);
            return data.Select(c => new ColorAdapter(c) as IColor).ToArray();
        }

        public void LoadImage(byte[] data)
        {
            var graphicsDevice = texture.GraphicsDevice;
            if (!texture.IsDisposed) texture.Dispose();
            using (var stream = new MemoryStream(data))
            {
                texture = Texture2D.FromStream(graphicsDevice, stream);
                //FlipTexture();
            }
        }

        public void UsePointFiltering()
        {
            // set filter point if this was Unity
        }

        public void Resize(int width, int height)
        {
            throw new NotImplementedException();
        }

        public void SetPixels(int x, int y, int width, int height, IColor[] pixelData)
        {
            throw new NotImplementedException();
        }

        private void FlipTexture()
        {
            var data = new Color[texture.Width * texture.Height];
            texture.GetData(data);
            data = FlipY(data, texture.Width, texture.Height);
            texture.SetData(data);
        }

        private static Color[] FlipY(Color[] data, int width, int height)
        {
            var newData = new Color[data.Length];
            int i = 0;
            for (int y = height - 1; y >= 0; y--)
            {
                var x0 = y * width;
                var xLength = (y * width) + width;
                for (int x = x0; x < xLength; x++)
                {
                    newData[i] = data[x];
                    i++;
                }
            }
            return newData;
        }

        public void SetPixels(IColor[] colorData)
        {
            throw new NotImplementedException();
        }

        public byte[] EncodeToPNG()
        {
            throw new NotImplementedException();
        }

        public void LoadTextureData(TextureData textureData, ColorData[] colors, string transColor = "#ff00ff")
        {
            throw new NotImplementedException();
        }

        public void Apply()
        {
            throw new NotImplementedException();
        }
    }
}
