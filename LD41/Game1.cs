using LD41.Gameplay;
using LD41.Raycasting;
using Microsoft.Xna.Framework;
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

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D target;
        Texture2D compTarget;
        //int[,] map;
        Vector2 pos = new Vector2(3, 3);
        Vector2 look = new Vector2(-1, 0);
        Vector2 fov = new Vector2(0, 0.66f);
        //Texture2D[] textures;
        ViewportAdapter va;
        GameState gameState;
        Dictionary<string, Texture2D> textures;
        Dictionary<string, TextureData> textureDataLookup;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Window.AllowUserResizing = true;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //map = new int[7, 5]
            //{
            //    { 5, 2, 4, 2, 5 },
            //    { 2, 2, 0, 2, 5 },
            //    { 2, 0, 0, 0, 2 },
            //    { 5, 0, 0, 0, 5 },
            //    { 2, 0, 0, 0, 2 },
            //    { 2, 2, 0, 2, 2 },
            //    { 0, 2, 3, 2, 0 },
            //};

            // TODO: Add your initialization logic here
            //map = new int[24, 24]
            //{
            //  {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
            //  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            //  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            //  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            //  {1,0,0,0,0,0,2,2,2,2,2,0,0,0,0,3,0,3,0,3,0,0,0,1},
            //  {1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,1},
            //  {1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,3,0,0,0,3,0,0,0,1},
            //  {1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,1},
            //  {1,0,0,0,0,0,2,2,0,2,2,0,0,0,0,3,0,3,0,3,0,0,0,1},
            //  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            //  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            //  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            //  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            //  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            //  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            //  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            //  {1,4,4,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            //  {1,4,0,4,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            //  {1,4,0,0,0,0,5,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            //  {1,4,0,4,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            //  {1,4,0,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            //  {1,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            //  {1,4,4,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            //  {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
            //};

            base.Initialize();

            va = new ViewportAdapter(Window, GraphicsDevice, width, height);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            textures = new Dictionary<string, Texture2D>();
            textureDataLookup = new Dictionary<string, TextureData>();

            LoadTexture("ShipFloor", "floor");
            LoadTexture("ShipCeiling", "ceiling");
            LoadTexture("ShipWall", "wall");
            LoadTexture("ShipWindow", "window");
            LoadTexture("ShipComputer", "todo");
            LoadTexture("ShipExit", "todo");

            LoadTexture("AsteroidFloor", "todo");
            LoadTexture("AsteroidCeiling", "todo");
            LoadTexture("AsteroidEntry", "todo");
            LoadTexture("AsteroidRock", "todo");
            LoadTexture("AsteroidResource", "todo");
            LoadTexture("AsteroidBoundary", "todo");

            LoadTexture("StationFloor", "todo");
            LoadTexture("StationCeiling", "todo");
            LoadTexture("StationWall", "todo");
            LoadTexture("StationComputer");
            LoadTexture("StationEntry", "todo");
            LoadTexture("StationBaySign", "todo");

            LoadTexture("Threshold", "todo");
            LoadTexture("Crosshair", "crosshair");
            LoadTexture("font");

            gameState = new GameState(new Map(Stations.Starting),
                new Computer(new Display(textures["font"]), new StationOperatingSystem()));

            target = new Texture2D(GraphicsDevice, width, height);
            compTarget = new Texture2D(GraphicsDevice,
                gameState.Computer.Display.Width, gameState.Computer.Display.Height);
        }

        private void LoadTexture(string name, string path = null)
        {
            var tex = Content.Load<Texture2D>(path ?? name);
            textures[name] = tex;
            textureDataLookup[name] = new TextureData(tex);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var delta = deltaTime(gameTime);

            var kb = Keyboard.GetState();
            if (kb.IsKeyDown(Keys.Left) && !kb.IsKeyDown(Keys.RightAlt))
            {
                var rot = Matrix.Identity * Matrix.CreateRotationZ(0.3f * delta);
                look = Vector2.Transform(look, rot);
                fov = Vector2.Transform(fov, rot);
            }
            if (kb.IsKeyDown(Keys.Right) && !kb.IsKeyDown(Keys.RightAlt))
            {
                var rot = Matrix.Identity * Matrix.CreateRotationZ(-0.3f * delta);
                look = Vector2.Transform(look, rot);
                fov = Vector2.Transform(fov, rot);
            }

            
            var newPos = pos;
            if (kb.IsKeyDown(Keys.Up))
            {
                newPos += look * (0.3f * delta);
            }
            if (kb.IsKeyDown(Keys.Down))
            {
                newPos -= look * (0.3f * delta);
            }
            if (kb.IsKeyDown(Keys.Left) && kb.IsKeyDown(Keys.RightAlt))
            {
                newPos += (new Vector2(-look.Y, look.X) * (0.3f * delta));
            }
            if (kb.IsKeyDown(Keys.Right) && kb.IsKeyDown(Keys.RightAlt))
            {
                newPos -= (new Vector2(-look.Y, look.X) * (0.3f * delta));
            }

            var dv = pos - newPos;
            if (dv == Vector2.Zero) return;
            var dx = dv.X;
            var dy = dv.Y;

            var np = new Vector2(newPos.X + (dx > 0 ? -0.6f : dx < 0 ? 0.6f : 0f), pos.Y).ToPoint();
            if (gameState.Map[np.X, np.Y] is Cell.VoidCell)
                pos.X = newPos.X;

            np = new Vector2(pos.X, newPos.Y + (dy > 0 ? -0.6f : dy < 0 ? 0.6f : 0f)).ToPoint();
            if (gameState.Map[np.X, np.Y] is Cell.VoidCell)
                pos.Y = newPos.Y;
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

            gameState.Computer.Display.RenderTo(compTarget);

            var cpixels = new Color[compTarget.Width * compTarget.Height];
            compTarget.GetData(cpixels);

            var tex = textures["StationComputer"];
            var pixels = new Color[tex.Width * tex.Height];
            tex.GetData(pixels);
            int j = 0;
            for (int p = 0; p < pixels.Length && j < cpixels.Length; p++)
                if (pixels[p] == Color.Magenta)
                    pixels[p] = cpixels[j++];
            tex.SetData(pixels);
            textureDataLookup["StationComputer"] = new TextureData(tex);

            var data = Raycaster.Raycast(
                gameState.Map,
                new Point(width, height),
                pos,
                look,
                fov,
                textureDataLookup);

            target.SetData(data);

            spriteBatch.Begin(blendState: BlendState.NonPremultiplied, 
                samplerState: SamplerState.PointWrap,
                transformMatrix: va.GetScaleMatrix());
            spriteBatch.Draw(target, Vector2.Zero, Color.White);
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
    }
}
