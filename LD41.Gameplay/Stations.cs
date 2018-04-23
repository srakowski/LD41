using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LD41.Gameplay
{
    enum StationType
    {
        Type1,
        Type2,
        Type3,
    }

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

        private static readonly char[,] layout2 = new char[10, 9]
        {
            {'%','%','%',':','=',':','%','%','%' },
            {'%','%','%','1','.','1','%','%','%' },
            {'%','%','%','.','.','.','.','%','%' },
            {'%','2','%','.','.','.','.','%','%' },
            {'=','.','%','.','.','.','.','.','c' },
            {'%','2','%','.','.','.','.','%','%' },
            {'%','%','%','.','.','.','.','%','%' },
            {'%','%','%','%','%','%','%','%','%' },
            {' ',' ',' ','%','c','%',' ',' ',' ' },
            {' ',' ',' ','%','%','%',' ',' ',' ' },
        };

        private static readonly char[,] layout3 = new char[14, 9]
        {
            {'%','%','%',':','=',':','%','%','%' },
            {'%','%','%','1','.','1','%','%','%' },
            {'%','%','.','.','.','.','.','%','%' },
            {'%','2','.','.','.','.','.','3','%' },
            {'=','.','.','.','.','.','.','.','=' },
            {'%','2','.','.','.','.','.','3','%' },
            {'%','%','.','%','.','%','.','%','%' },
            {'%','2','.','%','.','%','.','3','%' },
            {'=','.','.','%','.','%','.','.','=' },
            {'%','2','%','%','.','%','.','3','%' },
            {'%','%','%','%','.','%','%','%','%' },
            {'%','%','c','.','.','.','c','%','%' },
            {' ',' ','%','1','.','1','%',' ',' ' },
            {' ',' ','%','%','=','%','%',' ',' ' },
        };

        public string Name { get; }

        private Layout _currentLayout;

        public Station(StationType type, Vector2 position)
        {
            Name = StationsNames.Dequeue();
            Type = type;
            Position = position;
            Computer = new Computer();
            Computer.OS = new StationOperatingSystem(this);
            if (type == StationType.Type1)
                _currentLayout = Layout.FromChars(layout2);
            else if (type == StationType.Type2)
                _currentLayout = Layout.FromChars(layout);
            else
                _currentLayout = Layout.FromChars(layout3);
        }

        public StationType Type { get; }

        public Vector2 Position { get; }

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

        public MarketRates Rates { get; private set; }

        public void Update(float delta)
        {
            this.Computer.Update(delta);
        }

        internal void GenerateMarketRates(Random random)
        {
            Rates = new MarketRates();
            if (Type == StationType.Type1)
            {
                Rates.Food.Price = random.Next(30, 60)* 2;
                Rates.Textiles.Price = random.Next(30, 60)* 2;
                Rates.Ore.Price = random.Next(50, 80);
                Rates.Gems.Price = random.Next(50, 80);
                Rates.Computers.Price = random.Next(70, 100)* 3;
                Rates.Medicine.Price = random.Next(70, 100)* 3;
            }
            else if (Type == StationType.Type2)
            {
                Rates.Food.Price = random.Next(50, 80)* 2;
                Rates.Textiles.Price = random.Next(50, 80)* 2;
                Rates.Ore.Price = random.Next(50, 80);
                Rates.Gems.Price = random.Next(50, 80);
                Rates.Computers.Price = random.Next(50, 80)* 2;
                Rates.Medicine.Price = random.Next(50, 80)* 2;
            }
            else
            {
                Rates.Food.Price = random.Next(70, 100) * 3;
                Rates.Textiles.Price = random.Next(70, 100) * 3;
                Rates.Ore.Price = random.Next(50, 80);
                Rates.Gems.Price = random.Next(50, 80);
                Rates.Computers.Price = random.Next(30, 60) * 2;
                Rates.Medicine.Price = random.Next(30, 60) * 2;
            }
        }

        public Layout GetLayout() => _currentLayout;

        internal IEnumerable<Ship> GetAvailableShips()
        {

            return new Ship[]
            {
                new BasicMiningShip(),
                new MediumUtilityShip(),
            };
        }

        internal IEnumerable<Good> GetMarketPrices()
        {
            return new Good[]
            {
                Rates.Food,
                Rates.Textiles,
                Rates.Ore,
                Rates.Gems,
                Rates.Computers,
                Rates.Medicine,
            };
        }

        public void Initialize(Random random)
        {
            this.GenerateMarketRates(random);
        }

        static Station()
        {
            StationsNames = new Queue<string>();
            _sectorFirstNames = new List<string>(_sectorFirstNames.OrderBy(g => Guid.NewGuid()));
            _sectorLastNames = new List<string>(_sectorLastNames.OrderBy(g => Guid.NewGuid()));
            foreach (var first in _sectorFirstNames)
                foreach (var second in _sectorLastNames)
                    StationsNames.Enqueue($"{first} {second}");
        }

        private static Queue<string> StationsNames { get; }

        private static List<string> _sectorFirstNames = new List<string>
        {
            "red",
            "green",
            "blue",
            "yellow",
            "orange",
            "purple",
            "cyan",
            "black",
            "white",
            "magenta",
            "dark",
            "light",
            "rogue",
            "Kasprzak",
            "quill",
            "Pixel",
            "cherry"
        };

        private static List<string> _sectorLastNames = new List<string>
        {
            "alpha",
            "beta",
            "gamma",
            "delta",
            "epsilon",
            "zeta",
            "eta",
            "theta",
            "iota",
            "kappa",
            "lambda",
            "mu",
            "nu",
            "xi",
            "omicron",
            "pi",
            "rho",
            "sigma",
            "tau",
            "upsilon",
            "phi",
            "chi",
            "psi",
            "omega"
        };
    }

    public class MarketRates
    {
        public Good Food { get; } = new Good(GoodsTypes.Food);
        public Good Textiles { get; } = new Good(GoodsTypes.Textiles);
        public Good Ore { get; } = new Good(GoodsTypes.Ore);
        public Good Gems { get; } = new Good(GoodsTypes.Gems);
        public Good Computers { get; } = new Good(GoodsTypes.Computers);
        public Good Medicine { get; } = new Good(GoodsTypes.Medicine);
    }

    public class Good
    {
        public Good(GoodsTypes types, string unit = "T")
        {
            Type = types;
            Unit = unit;
        }
        public string Name => Type.ToString();
        public GoodsTypes Type { get; set; }
        public int Price { get; set; }
        public string Unit { get; }
    }

}
