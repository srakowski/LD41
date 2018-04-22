using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD41.Raycasting
{
    struct TextureData
    {
        public TextureData(Texture2D texture)
        {
            Width = texture.Width;
            Height = texture.Height;
            var data = new Color[texture.Width * texture.Height];
            texture.GetData(data);
            Data = data;
        }
        public int Width { get; }
        public int Height { get; }
        public Color[] Data { get; }
    }
}
