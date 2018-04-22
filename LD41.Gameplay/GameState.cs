using Microsoft.Xna.Framework;

namespace LD41.Gameplay
{
    class GameState
    {
        public GameState(Point screenDim, Map map, Station station, Player player,
            Asteroid asteroid)
        {
            ScreenDim = screenDim;
            Map = map;
            Station = station;
            Station.GameState = this;
            Player = player;
            Player.GameState = this;
            ActiveEnvironment = Station;
            Asteroid = asteroid;
            Asteroid.GameState = this;
        }

        public Map Map { get; set; }
        public Station Station { get; }
        public Player Player { get; }
        public Point ScreenDim { get; }
        public IEnvironment ActiveEnvironment { get; set; }
        public Asteroid Asteroid { get; }

        public void LoadStation()
        {
            Map.SetEnvLayout(Station.GetLayout());
            ActiveEnvironment = Station;
        }

        public void LoadAsteroid()
        {
            Map.SetEnvLayout(Asteroid.GetLayout());
            ActiveEnvironment = Asteroid;
        }
    }
}
