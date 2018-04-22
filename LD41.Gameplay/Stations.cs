using System;
using System.Collections.Generic;

namespace LD41.Gameplay
{
    class Station : IEnvironment
    {
        private GameState _gameState;

        private static readonly char[,] layout = new char[10, 9]
        {
            {'%','%','%',':','=',':','%','%','%' },
            {'%','%','%','1','.','1','%','%','%' },
            {'%','%','.','.','.','.','.','%','%' },
            {'%','2','.','.','.','.','.','3','%' },
            {'=','.','.','.','.','.','.','.','=' },
            {'%','2','.','.','.','.','.','3','%' },
            {'%','%','.','.','.','.','.','%','%' },
            {'%','%','%','%','.','%','%','%','%' },
            {' ',' ',' ','%','c','%',' ',' ',' ' },
            {' ',' ',' ','%','%','%',' ',' ',' ' },
        };

        private Layout _currentLayout;

        public Station(Computer computer)
        {
            this.Computer = computer;
            Computer.OS = new StationOperatingSystem();
            _currentLayout = Layout.FromChars(layout);
        }

        public Computer Computer { get; }

        public GameState GameState
        {
            get => _gameState;
            set
            {
                _gameState = value;
                Computer.GameState = value;
            }
        }

        internal Layout GetLayout() => _currentLayout;

        internal IEnumerable<Ship> GetAvailableShips()
        {

            return new[]
            {
                new BasicMiningShip(new Computer(new Display(this.Computer.Display.FontTexture))),
            };
        }
    }
}
