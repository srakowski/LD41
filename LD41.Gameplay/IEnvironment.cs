using System;
using Microsoft.Xna.Framework;

namespace LD41.Gameplay
{
    interface IEnvironment
    {
        Vector2 Position { get; }
        string Name { get; }
        GameState GameState { get; set; }
        void Initialize(Random random);
        Layout GetLayout();
        void Update(float delta);
    }
}
