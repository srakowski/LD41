using System;
using LD41.Raycasting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LD41.Gameplay
{
    class Player
    {
        Vector2 pos = Vector2.Zero;
        Vector2 look = new Vector2(-1, 0);
        Vector2 fov = new Vector2(0, 0.66f);
        PlayerState state = new PlayerFreeMove();
        PlayerState interact = null;
        int _credits = 5000;
        Ship _ship = new NoShip();

        public Player(Vector2 pos)
        {
            this.pos = pos;
            var rot = Matrix.Identity * Matrix.CreateRotationZ(MathHelper.ToRadians(-90));
            look = Vector2.Transform(look, rot);
            fov = Vector2.Transform(fov, rot);
        }

        private GameState _gameState;
        public GameState GameState
        {
            get => _gameState;
            set
            {
                _gameState = value;
                if (Ship.Computer != null)
                    Ship.Computer.GameState = value;
            }
        }

        public Vector2 Position => pos;
        public Vector2 Look => look;
        public Vector2 Fov => fov;
        public int Credits => _credits;
        public Ship Ship => _ship;

        public void Update(GameState gameState, float delta)
        {
            state.Update(this, gameState, delta);
            interact?.Update(this, gameState, delta); // must be after other
        }

        abstract class PlayerState
        {
            public abstract void Update(Player player, GameState gameState, float delta);
        }

        class PlayerFreeMove : PlayerState
        {
            public override void Update(Player player, GameState gameState, float delta)
            {
                var kb = Keyboard.GetState();

                player.interact = null;

                var lookingAt = Raycaster.GetTarget(gameState.Map, gameState.ScreenDim, gameState.Player);
                if (lookingAt.Distance < .95)
                {
                    if (lookingAt.Cell is Cell.ComputerCell)
                    {
                        if (lookingAt.Cell is Cell.StationComputer)
                            player.interact = new PlayerComputerInteract((gameState.ActiveEnvironment as Station).Computer);
                        else if (lookingAt.Cell is Cell.ShipComputer)
                            player.interact = new PlayerComputerInteract(gameState.Player.Ship.Computer);
                    }
                    if (lookingAt.Cell is Cell.AsteroidMineableCell mineable)
                    {
                        if (kb.IsKeyDown(Keys.Space))
                        {
                            (gameState.ActiveEnvironment as Asteroid).Mine(mineable);
                            if (player.Ship.HasCargoCapacity)
                            {
                                if (lookingAt.Cell is Cell.AsteroidRock) player.Ship.CargoHold.Ore.Quantity += 1;
                                if (lookingAt.Cell is Cell.AsteroidResource) player.Ship.CargoHold.Ore.Quantity += 1;
                            }
                        }
                    }
                }

                if (kb.IsKeyDown(Keys.Left) && !kb.IsKeyDown(Keys.RightAlt))
                {
                    var rot = Matrix.Identity * Matrix.CreateRotationZ(0.3f * delta);
                    player.look = Vector2.Transform(player.look, rot);
                    player.fov = Vector2.Transform(player.fov, rot);
                }
                if (kb.IsKeyDown(Keys.Right) && !kb.IsKeyDown(Keys.RightAlt))
                {
                    var rot = Matrix.Identity * Matrix.CreateRotationZ(-0.3f * delta);
                    player.look = Vector2.Transform(player.look, rot);
                    player.fov = Vector2.Transform(player.fov, rot);
                }

                var newPos = player.pos;
                if (kb.IsKeyDown(Keys.Up))
                {
                    newPos += player.look * (0.3f * delta);
                }
                if (kb.IsKeyDown(Keys.Down))
                {
                    newPos -= player.look * (0.3f * delta);
                }
                if (kb.IsKeyDown(Keys.Left) && kb.IsKeyDown(Keys.RightAlt))
                {
                    newPos += (new Vector2(-player.look.Y, player.look.X) * (0.3f * delta));
                }
                if (kb.IsKeyDown(Keys.Right) && kb.IsKeyDown(Keys.RightAlt))
                {
                    newPos -= (new Vector2(-player.look.Y, player.look.X) * (0.3f * delta));
                }

                var dv = player.pos - newPos;
                if (dv == Vector2.Zero) return;
                var dx = dv.X;
                var dy = dv.Y;

                var np = new Vector2(newPos.X + (dx > 0 ? -0.6f : dx < 0 ? 0.6f : 0f), player.pos.Y).ToPoint();
                if (gameState.Map[np.X, np.Y] is Cell.VoidCell)
                    player.pos.X = newPos.X;

                np = new Vector2(player.pos.X, newPos.Y + (dy > 0 ? -0.6f : dy < 0 ? 0.6f : 0f)).ToPoint();
                if (gameState.Map[np.X, np.Y] is Cell.VoidCell)
                    player.pos.Y = newPos.Y;
            }
        }

        class PlayerComputerInteract : PlayerState
        {
            private Computer _computer;

            private static KeyboardState _prevkb;
            private static KeyboardState _currkb;

            public PlayerComputerInteract(Computer computer)
            {
                _computer = computer;
            }

            public override void Update(Player player, GameState gameState, float delta)
            {
                _prevkb = _currkb;
                _currkb = Keyboard.GetState();
                if (KeyPressed(Keys.Enter)) _computer.OS.Enter();
                else if (KeyPressed(Keys.NumPad1) || KeyPressed(Keys.D1)) _computer.OS.Num(1);
                else if (KeyPressed(Keys.NumPad2) || KeyPressed(Keys.D2)) _computer.OS.Num(2);
                else if (KeyPressed(Keys.NumPad3) || KeyPressed(Keys.D3)) _computer.OS.Num(3);
                else if (KeyPressed(Keys.NumPad4) || KeyPressed(Keys.D4)) _computer.OS.Num(4);
                else if (KeyPressed(Keys.NumPad5) || KeyPressed(Keys.D5)) _computer.OS.Num(5);
                else if (KeyPressed(Keys.NumPad6) || KeyPressed(Keys.D6)) _computer.OS.Num(6);
                else if (KeyPressed(Keys.NumPad7) || KeyPressed(Keys.D7)) _computer.OS.Num(7);
                else if (KeyPressed(Keys.NumPad8) || KeyPressed(Keys.D8)) _computer.OS.Num(8);
                else if (KeyPressed(Keys.NumPad9) || KeyPressed(Keys.D9)) _computer.OS.Num(9);
                else if (KeyPressed(Keys.NumPad0) || KeyPressed(Keys.D0)) _computer.OS.Num(0);
            }

            private bool KeyPressed(Keys key) => _currkb.IsKeyUp(key) && _prevkb.IsKeyDown(key);
        }

        public bool CanAfford(int price) => Credits >= price;

        internal void Buy(Ship ship)
        {
            if (!CanAfford(ship.Price)) return;
            _credits -= ship.Price;
            _ship = ship;
            if (Ship.Computer != null)
                Ship.Computer.GameState = _gameState;
        }

        internal void Buy(Good good)
        {
            if (!CanAfford(good.Price)) return;
            _credits -= good.Price;
            Ship.AddInventory(good);
        }

        public void Credit(int price) => _credits += price;
    }
}
