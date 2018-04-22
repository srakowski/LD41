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

        public Asteroid()
        {
            _currLayout = Layout.FromChars(layout);
        }

        internal Layout GetLayout() => _currLayout;


        public GameState GameState { get; set; }

        public void Mine(Cell.AsteroidMineableCell cell)
        {
            _currLayout.Replace(cell, new Cell.AsteroidVoid());
            GameState.Map.SetEnvLayout(this._currLayout);
        }
    }
}
