namespace LD41.Gameplay
{
    class GameState
    {
        public GameState(Map map, Computer computer)
        {
            Map = map;
            Computer = computer;
        }

        public Map Map { get; }

        public Computer Computer { get; }
    }
}
