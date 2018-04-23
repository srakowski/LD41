using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LD41.Gameplay
{
    class GameState
    {
        private Random random = new Random();

        public GameState(Point screenDim, Map map, Player player)
        {
            ScreenDim = screenDim;
            Map = map;
            Player = player;
            Player.GameState = this;
            MakeWorld();
        }

        public Map Map { get; set; }
        public Player Player { get; }
        public Point ScreenDim { get; }
        public IEnvironment ActiveEnvironment { get; set; }

        private List<IEnvironment> Envs { get; set; }

        private void MakeWorld()
        {
            Envs = new List<IEnvironment>();
            Envs.AddRange(GenEnvs());
            Envs.ForEach(s =>
            {
                s.GameState = this;
                s.Initialize(random);
            });
            
            LoadEnv(Envs.OfType<Station>().OrderBy(s => Guid.NewGuid()).First());
        }

        internal void LoadEnv(IEnvironment env)
        {
            ActiveEnvironment = env;
            Map.SetEnvLayout(env.GetLayout());
        }

        private IEnumerable<IEnvironment> GenEnvs()
        {
            for (int x = 0; x < 18; x++)
                for (int y = 0; y < 18; y++)
                {
                    var xpos = random.Next(x * 100, (x + 1) * 100);
                    var ypos = random.Next(y * 100, (y + 1) * 100);
                    if (random.Next(100) > 60)
                    {
                        var next = random.Next(90);
                        yield return new Station(
                            next < 30 ? StationType.Type1 :
                            next < 60 ? StationType.Type2 :
                            StationType.Type3,
                            new Vector2(xpos, ypos));
                    }
                    else
                    {
                        yield return new Asteroid(new Vector2(xpos, ypos));
                    }
                }
        }

        internal Station[] GetNearbyStations(int max)
        {
            return this.Envs.OfType<Station>()
                .Where(s => s != ActiveEnvironment)
                .OrderBy(s => Vector2.Distance(s.Position, ActiveEnvironment.Position))
                .Take(max)
                .ToArray();
        }

        internal Asteroid[] GetNearbyAsteroids(int max)
        {
            return this.Envs.OfType<Asteroid>()
                .Where(s => s != ActiveEnvironment)
                .OrderBy(s => Vector2.Distance(s.Position, ActiveEnvironment.Position))
                .Take(max)
                .ToArray();
        }
    }
}
