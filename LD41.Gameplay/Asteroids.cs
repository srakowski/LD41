using System;
using Microsoft.Xna.Framework;

namespace LD41.Gameplay
{
    class Asteroid : IEnvironment
    {
        private static readonly char[,] layout = new char[12, 9]
        {
            {'!','!','!',':','-',':','!','!','!' },
            {'!','!','!','!',',','!','!','!','!' },
            {'!','!','!','R','R','R','!','!','!' },
            {'!','!','R','R','R','R','R','!','!' },
            {'!','R','R','R','R','R','R','R','!' },
            {'!','R','R','R','R','$','R','R','!' },
            {'!','R','R','R','R','$','$','R','!' },
            {'!','R',',','R','R','$','$','R','!' },
            {'!','R','R','R','R','R','R','R','!' },
            {'!','!','R','R','R','R','R','!','!' },
            {'!','!','!','R','R','R','!','!','!' },
            {'!','!','!','!','!','!','!','!','!' },
        };

        private Layout _currLayout;

        private static int val;

        public Asteroid(Vector2 position)
        {
            Name = $"Asteroid A{val++}";
            Position = position;
            _currLayout = Layout.FromChars(layout);
        }

        public Layout GetLayout() => _currLayout;

        public GameState GameState { get; set; }

        public Vector2 Position { get; }

        public string Name { get; }

        public void Mine(Cell.AsteroidMineableCell cell)
        {
            _currLayout.Replace(cell, new Cell.AsteroidVoid());
            GameState.Map.SetEnvLayout(this._currLayout);
        }

        public void Initialize(Random random)
        {
        }

        public void Update(float delta)
        {
        }
    }
}
