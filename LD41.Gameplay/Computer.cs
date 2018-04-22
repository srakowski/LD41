using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LD41.Gameplay
{
    class Computer
    {
        public Computer(Display display, OperatingSystem os)
        {
            Display = display;
            OS = os;
            OS.Computer = this;
            OS.Run();
        }

        public OperatingSystem OS { get; }

        public Display Display { get; }
    }

    class Display
    {
        private const int CHAR_DIM = 8;
        private Color[,] _pixels;
        private char[,] _charBuffer;
        private Point _cursorPos;
        private Texture2D _fontTexture;
        private Color[] _fontData;

        public Display(Texture2D fontTexture)
        {
            CharWidth = 24;
            Width = CharWidth * CHAR_DIM;
            CharHeight = 16;
            Height = CharHeight * CHAR_DIM;
            _pixels = new Color[CharHeight * CHAR_DIM, CharWidth * CHAR_DIM];
            _charBuffer = new char[CharHeight, CharWidth];
            _fontTexture = fontTexture;
            _fontData = new Color[_fontTexture.Width * _fontTexture.Height];
            _fontTexture.GetData(_fontData);
            Clear();
        }

        public int Width { get; }
        public int Height { get; }

        public int CharWidth { get; }
        public int CharHeight { get; }

        public void Clear()
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    _pixels[y, x] = Color.Blue;

            for (int y = 0; y < CharHeight; y++)
                for (int x = 0; x < CharWidth; x++)
                    _charBuffer[y, x] = '\0';
            _cursorPos = Point.Zero;
        }

        public void WriteLines(params string[] lines)
        {
            foreach (var line in lines)
            {
                foreach (var c in line)
                    Putc(c);
                Putc('\n');
            }
            FlushCharDisplay();
        }

        private void FlushCharDisplay()
        {
            for (int y = 0; y < CharHeight; y++)
                for (int x = 0; x < CharWidth; x++)
                {
                    var c = _charBuffer[y, x];
                    byte b = ((byte)c);
                    int row = b / 16;
                    int col = b % 16;
                    for (int rw = 0; rw < CHAR_DIM; rw++)
                    {
                        var i = (((row * CHAR_DIM) + rw) * _fontTexture.Width);
                        for (int cl = 0; cl < CHAR_DIM; cl++)
                        {
                            var pixel = _fontData[i + ((col * 8) + cl)];
                            _pixels[(y * CHAR_DIM) + rw, (x * CHAR_DIM) + cl] = 
                                pixel.A == 0 ? Color.Blue : pixel;
                        }
                    }
                }
        }

        private void Putc(char c)
        {
            if (c == '\n')
            {
                NewLine();
                return;
            }

            _charBuffer[_cursorPos.Y, _cursorPos.X] = c;

            _cursorPos.X += 1;
            if (_cursorPos.X >= CharWidth)
                NewLine();
        }

        private void NewLine()
        {
            _cursorPos.Y += 1;
            _cursorPos.X = 0;
            _cursorPos.Y %= CharHeight;
        }

        internal void RenderTo(Texture2D compTarget)
        {
            var pixels = new Color[_pixels.Length];
            var p = 0;
            foreach (var color in _pixels)
                pixels[p++] = color;

            compTarget.SetData(pixels);
        }
    }
}
