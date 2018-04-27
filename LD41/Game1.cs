using LD41.Gameplay;
using LD41.PV8SDK;
using LD41.Raycasting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace LD41
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        const int width = 400;
        const int height = 300;

        public static GraphicsDevice GraphicsDeviceInstance { get; set; }

        Point screenDim = new Point(width, height);
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D target;
        ViewportAdapter va;
        GameState gameState;
        Dictionary<string, Texture2D> textures;
        Dictionary<string, TextureData> textureDataLookup;
        SpriteFont hudFont;
        RaycasterTarget lookingAt;
        //PixelVisionRunnerAdapter testrunner;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Window.AllowUserResizing = true;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            va = new ViewportAdapter(Window, GraphicsDevice, width, height);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();

            GraphicsDeviceInstance = this.GraphicsDevice;
            //testrunner = new PixelVisionRunnerAdapter(this.GraphicsDevice);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            textures = new Dictionary<string, Texture2D>();
            textureDataLookup = new Dictionary<string, TextureData>();

            LoadTexture("ShipFloor", "floor");
            LoadTexture("ShipCeiling", "ceiling");
            LoadTexture("ShipWall", "wall");
            LoadTexture("ShipWindow", "window");
            LoadTexture("ShipComputer");
            LoadTexture("ShipExit");
            LoadTexture("ShipSystem");

            LoadTexture("AsteroidFloor", "AsteroidBoundary");
            LoadTexture("AsteroidCeiling", "AsteroidBoundary");
            LoadTexture("AsteroidEntry", "todo");
            LoadTexture("AsteroidRock");
            LoadTexture("AsteroidResource");
            LoadTexture("AsteroidBoundary");

            LoadTexture("StationFloor");
            LoadTexture("StationCeiling", "ceiling");
            LoadTexture("StationWall");
            LoadTexture("StationComputer");
            LoadTexture("StationEntry", "StationExit");
            LoadTexture("StationBaySign");

            LoadTexture("None", "todo");
            LoadTexture("Threshold");
            LoadTexture("Crosshair", "crosshair");
            LoadTexture("font");

            hudFont = Content.Load<SpriteFont>("HudFont");
            Sfx.Confirm = Content.Load<SoundEffect>("confirm");
            Sfx.Select = Content.Load<SoundEffect>("select");
            Sfx.Reject = Content.Load<SoundEffect>("reject");

            Display.FontTexture = textures["font"];

            var map = new Map(null, null);

            gameState = new GameState(screenDim,
                map,
                new Player(new Vector2((map.Width / 2) + 0.5f, (map.Height *0.5f) + 3)));

            target = new Texture2D(GraphicsDevice, width, height);
        }

        private void LoadTexture(string name, string path = null)
        {
            var tex = Content.Load<Texture2D>(path ?? name);
            textures[name] = tex;
            textureDataLookup[name] = new TextureData(tex);
        }

        protected override void Update(GameTime gameTime)
        {
            lookingAt = Raycaster.GetTarget(gameState.Map, screenDim, gameState.Player);

            var delta = deltaTime(gameTime);

            gameState.Player.Update(gameState, delta);
            gameState.Player.Ship.Computer?.Update(delta);
            gameState.ActiveEnvironment.Update(delta);

            //testrunner.Update(delta);
        }

        private static float deltaTime(GameTime gameTime) =>
            ((float)gameTime.ElapsedGameTime.TotalMilliseconds / 100f);

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //var p2 = new Color[192 * 128];
            //var pixels = testrunner.Draw();
            //for (int y = 0; y < 128; y++)
            //    for (int x = 0; x < 128; x++)
            //    {
            //        p2[(y * 192) + x] = pixels[(y * 128) + x];
            //    }
            //RenderToVirtualScreen("StationComputer", p2);

            UpdateStationComputerTexture();
            UpdateShipComputerTexture();

            var data = Raycaster.Raycast(
                gameState.Map,
                screenDim,
                gameState.Player,
                textureDataLookup);

            target.SetData(data);

            spriteBatch.Begin(blendState: BlendState.NonPremultiplied,
                samplerState: SamplerState.PointClamp,
                transformMatrix: va.GetScaleMatrix());
            spriteBatch.Draw(target, Vector2.Zero, Color.White);
            spriteBatch.End();


            //var t = Raycaster.GetTarget(
            //    gameState.Map,
            //    new Point(width, height),
            //    gameState.Player);

            spriteBatch.Begin();
            spriteBatch.DrawString(hudFont, lookingAt.Cell.GetType().Name, Vector2.One, Color.Lime);
            spriteBatch.DrawString(hudFont, lookingAt.Distance.ToString(), new Vector2(0, 30), Color.Lime);
            spriteBatch.End();

            //var crosshair = textures["Crosshair"];
            //spriteBatch.Draw(crosshair,
            //    new Vector2(width * 0.5f, height * 0.5f),
            //    null,
            //    new Color(Color.White, 0.5f),
            //    0f,
            //    new Vector2(crosshair.Width * 0.5f, crosshair.Height * 0.5f),
            //    1f,
            //    SpriteEffects.None,
            //    0f);



            base.Draw(gameTime);
        }

        private void UpdateStationComputerTexture()
        {
            if (gameState.ActiveEnvironment is Station station)
                UpdateComputerTexture(station.Computer, "StationComputer");
        }

        private void UpdateShipComputerTexture()
        {
            if (gameState.Player.Ship?.Computer != null)
                UpdateComputerTexture(gameState.Player.Ship.Computer, "ShipComputer");
        }

        private void UpdateComputerTexture(Computer computer, string texture)
        {
            var cpixels = computer.Display.Render();
            RenderToVirtualScreen(texture, cpixels);
        }

        private void RenderToVirtualScreen(string texture, Color[] cpixels)
        {
            var tex = textures[texture];
            var pixels = new Color[tex.Width * tex.Height];
            tex.GetData(pixels);
            int j = 0;
            for (int p = 0; p < pixels.Length && j < cpixels.Length; p++)
                if (pixels[p] == Color.Magenta)
                    pixels[p] = cpixels[j++];
            var compTarget = new Texture2D(GraphicsDevice, tex.Width, tex.Height);
            compTarget.SetData(pixels);
            textureDataLookup[texture] = new TextureData(compTarget);
            compTarget.Dispose();
        }
    }
}
