using Microsoft.Xna.Framework;
using PixelVisionRunner;
using PixelVisionSDK;
using System;
using System.Linq;

namespace LD41.PV8SDK
{
    class DisplayTarget : IDisplayTarget
    {
        public Color[] Data { get; private set; }

        private Func<IEngine> _engineRef;

        public void CacheColors() { }
        
        public DisplayTarget(Func<IEngine> engineRef)
        {
            this._engineRef = engineRef;
        }

        public void Render()
        {
            var engine = _engineRef();

            var pixelMap = engine.displayChip.pixels;

            var colors = engine.colorChip.colors;

            var backgroundColor = engine.colorChip.backgroundColor;

            Data = pixelMap.Select(p =>
                (p < 0 || p >= colors.Length)
                    ? colors[backgroundColor]
                    : colors[p]
                )
                .Select(c => new Color(c.r, c.g, c.b))
                .ToArray();
        }

        public void ResetResolution(int width, int height) { }
    }
}
