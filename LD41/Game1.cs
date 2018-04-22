using LD41.Raycasting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        int[,] map;
        Vector2 pos = new Vector2(3, 3);
        Vector2 look = new Vector2(-1, 0);
        Vector2 fov = new Vector2(0, 0.66f);
        Texture2D[] textures;
        ViewportAdapter va;

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
            map = new int[11, 5]
            {
                { 2, 2, 4, 2, 2 },
                { 5, 0, 0, 0, 5 },
                { 5, 0, 0, 0, 5 },
                { 5, 0, 0, 0, 5 },
                { 5, 0, 0, 0, 5 },
                { 5, 0, 0, 0, 5 },
                { 2, 2, 0, 2, 2 },
                { 2, 0, 0, 0, 2 },
                { 2, 0, 0, 0, 2 },
                { 2, 0, 0, 0, 2 },
                { 2, 2, 2, 2, 2 },
            };

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
            target = new Texture2D(GraphicsDevice, width, height);
            var floor = Content.Load<Texture2D>("floor");
            var wall = Content.Load<Texture2D>("wall");
            var comp = Content.Load<Texture2D>("comp");
            var window = Content.Load<Texture2D>("window");
            var ceiling = Content.Load<Texture2D>("ceiling");
            textures = new Texture2D[]
            {
                floor,
                wall,
                floor,
                comp,
                window,
                ceiling,
            };
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
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
                newPos += look * delta;
            }
            if (kb.IsKeyDown(Keys.Down))
            {
                newPos -= look * delta;
            }
            if (kb.IsKeyDown(Keys.Left) && kb.IsKeyDown(Keys.RightAlt))
            {
                newPos += (new Vector2(-look.Y, look.X) * delta);
            }
            if (kb.IsKeyDown(Keys.Right) && kb.IsKeyDown(Keys.RightAlt))
            {
                newPos -= (new Vector2(-look.Y, look.X) * delta);
            }

            var dv = pos - newPos;
            if (dv == Vector2.Zero) return;
            var dx = dv.X;
            var dy = dv.Y;

            var np = new Vector2(newPos.X + (dx > 0 ? -0.6f : dx < 0 ? 0.6f : 0f), pos.Y).ToPoint();
            if (map[np.X, np.Y] == 0)
                pos.X = newPos.X;

            np = new Vector2(pos.X, newPos.Y + (dy > 0 ? -0.6f : dy < 0 ? 0.6f : 0f)).ToPoint();
            if (map[np.X, np.Y] == 0)
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

            var data = Raycaster.Raycast(
                map,
                new Point(width, height),
                pos,
                look,
                fov,
                textures);

            target.SetData(data);

            spriteBatch.Begin(blendState: BlendState.NonPremultiplied, transformMatrix: va.GetScaleMatrix());
            spriteBatch.Draw(target, Vector2.Zero, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
