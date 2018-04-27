using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelVisionRunner;
using PixelVisionSDK;
using PixelVisionSDK.Chips;

namespace LD41.PV8SDK
{
    class PixelVisionRunnerAdapter
    {
        private Runner _runner;

        public IEngine _engine;

        private DisplayTarget _target;

        public PixelVisionRunnerAdapter(GraphicsDevice gd, string prog)
        {
            this._runner = new Runner(new TextureFactory(gd));

            Dictionary<string, byte[]> files;
            using (var data = File.OpenRead($"./Content/{prog}.pv8"))
            {
                files = ExtractZipFromMemoryStream(data);
            }

            _target = new DisplayTarget(() => _engine);

            _engine = new PixelVisionEngine(_target,
                new InputFactory(),
                new[]
                {
                    typeof(ColorChip).FullName,
                    typeof(SpriteChip).FullName,
                    typeof(TilemapChip).FullName,
                    typeof(FontChip).FullName,
                    typeof(ControllerChip).FullName,
                    typeof(DisplayChip).FullName,
                    typeof(ControllerChip).FullName,
                    typeof(LuaGameChip).FullName
                });

            _engine.chipManager.AddService(typeof(LuaService).FullName, new LuaService());

            this._runner.ProcessFiles(_engine, files);

            //this._runner.ActivateEngine(_engine);
        }

        internal void Update(float delta)
        {
            this._runner.Update(delta);
        }

        private static Dictionary<string, byte[]> ExtractZipFromMemoryStream(Stream stream)
        {
            var zip = ZipStorer.Open(stream, FileAccess.Read);

            var dir = zip.ReadCentralDir();

            var files = new Dictionary<string, byte[]>();

            // Look for the desired file
            foreach (var entry in dir)
            {
                var fileBytes = new byte[0];
                zip.ExtractFile(entry, out fileBytes);

                files.Add(entry.ToString(), fileBytes);
            }

            zip.Close();

            return files;
        }

        internal Color[] Draw()
        {
            _runner.Draw();
            return _target.Data;
        }
    }
}
