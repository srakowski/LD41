using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LD41.Gameplay
{
    abstract class OperatingSystem
    {
        private OSModule _currentModule;
        private Stack<OSModule> _moduleStack = new Stack<OSModule>();

        public Computer Computer { get; set; }

        public virtual void Init() { }

        public void Enter()
        {
            if (_currentModule is ProcRun)
                return;

            _currentModule.Enter();
        }

        public void Num(int value)
        {
            if (_currentModule is ProcRun)
                return;

            _currentModule?.Select(value);
        }

        public void PushModule(OSModule module)
        {
            if (_currentModule != null)
                _moduleStack.Push(_currentModule);

            _currentModule = module;
            _currentModule?.Activate();
        }

        public void Update(float delta) =>
            _currentModule?.Update(delta);

        public OSModule PopModule()
        {
            var prev = _currentModule;

            _currentModule = _moduleStack.Any()
                ? _moduleStack.Pop()
                : null;

            _currentModule?.Activate();
            return prev;
        }
    }

    interface OSModule
    {
        void Activate();
        void Enter();
        void Select(int num);
        void Update(float delta);
    }

    class Mnu : OSModule
    {
        private string _title;
        private List<MnuOpt> _opts = new List<MnuOpt>();
        private Computer computer;

        public Mnu(OperatingSystem os, string title)
        {
            computer = os.Computer;
            _title = title.ToUpper();
        }

        public Mnu AddOpt(string name, Action onSelect)
        {
            _opts.Add(new MnuOpt(name, onSelect));
            return this;
        }

        public void Enter() { }

        public void Activate()
        {
            computer.Display.Clear();
            computer.Display.WriteLines(_title);
            for (int i = 0; i < _opts.Count; i++)
                computer.Display.WriteLines($"{i + 1}. {_opts[i].Name}");
            computer.Display.WriteLines("\nPRESS #\nOR PRESS 0 TO EXIT");
        }

        public void Select(int num)
        {
            if (num == 0)
            {
                Sfx.Select.Play();
                computer.OS.PopModule();
                return;
            }

            var idx = num - 1;
            if (idx < 0 || idx >= _opts.Count) return;
            Sfx.Select.Play();
            _opts[idx].OnSelect();
        }

        public void Update(float delta) { }
    }

    class MnuOpt
    {
        public MnuOpt(string name, Action onSelect)
        {
            Name = name.ToUpper();
            OnSelect = onSelect;

        }

        public string Name { get; }
        public Action OnSelect { get; }
    }

    class Msg : OSModule
    {
        private Computer computer;

        public Msg(OperatingSystem os, string value, Action onEnter)
        {
            computer = os.Computer;
            Value = value.ToUpper();
            OnEnter = onEnter;
        }

        public string Value { get; }

        public Action OnEnter { get; }

        public void Enter()
        {
            Sfx.Select.Play();
            OnEnter?.Invoke();
        }

        public void Activate()
        {
            computer.Display.Clear();
            computer.Display.WriteLines(
                Value, "\nPRESS ENTER");
        }

        public void Select(int num) { }

        public void Update(float delta) { }
    }

    class ProcRun : OSModule
    {
        private string _procName;
        private Computer computer;
        private float _secs;
        private float _secsRemaining;
        private Action _onDone;

        public ProcRun(OperatingSystem os, string procName,
            int procTimeSec, Action onDone)
        {
            computer = os.Computer;
            _procName = procName;
            _secs = procTimeSec;
            _secsRemaining = procTimeSec;
            _onDone = onDone;
        }
        
        public void Activate()
        {
            computer.Display.Clear();
            computer.Display.WriteLines(
                $": EXE {_procName}");
        }

        public void Enter() { }

        public void Select(int num) { }

        public void Update(float delta)
        {
            _secsRemaining -= delta;
            computer.Display.Clear();
            computer.Display.WriteLines(
                $": EXE {_procName}",
                $": ESTIMATED COMPLETION IN",
                $": {Math.Ceiling(_secsRemaining)} SECONDS");

            if (_secsRemaining < 0)
                _onDone?.Invoke();
        }
    }

    class StationOperatingSystem : OperatingSystem
    {
        public Station station;

        public StationOperatingSystem(Station station)
        {
            this.station = station;
        }

        public override void Init() => PushModule(new Msg(
            this,
            Lns($"{station.Name} Station\n",
                "Your fully automated",
                "self service space",
                "station since the",
                "year 0x10c"),
            () => PushModule(HomeMenu()))
        );

        private Mnu HomeMenu()
        {
            var mnu = new Mnu(this, "Main Menu")
                .AddOpt("Credit Balance", () => PushModule(AccountInfo()))
                .AddOpt("Ship Inventory", () => PushModule(Inventory()))
                .AddOpt("Sell", () => PushModule(SellInvantory()))
                .AddOpt("Buy Tradeable Goods", () => PushModule(BuyGoods()))
                .AddOpt("Buy Ship", () => PushModule(BuyShip()));

            return mnu; 
                
        }

        private OSModule Inventory()
        {
            return new Msg(this,
                Lns($"Inventory\nCargo Capacity {Player.Ship.MaxCargo}T",
                $"{Player.Ship.CargoHold.Food.Quantity}t Food",
                $"{Player.Ship.CargoHold.Textiles.Quantity}t Textiles",
                $"{Player.Ship.CargoHold.Ore.Quantity}t Ore",
                $"{Player.Ship.CargoHold.Gems.Quantity}t Gems",
                $"{Player.Ship.CargoHold.Computers.Quantity}t Computers",
                $"{Player.Ship.CargoHold.Medicine.Quantity}t Medicine"
                ), () => PopModule());
        }

        private OSModule BuyGoods()
        {
            var mnu = new Mnu(this, "Buy Tradeable Goods");
            var rates = station.GetMarketPrices();
            foreach (var rate in rates)
                mnu.AddOpt($"1{rate.Unit} {rate.Name} {rate.Price}$", 
                    () => PushModule(BuyGood(rate)));
            return mnu;
        }

        private Msg AccountInfo() =>
            new Msg(this,
                Lns("Credits Balance",
                    $"{Computer.GameState.Player.Credits}$"
                ),
                () => PopModule());

        private Mnu BuyShip()
        {
            var shipsAvail = station.GetAvailableShips();

            var mod = new Mnu(this, "Buy Ship");
            foreach (var ship in shipsAvail.OrderBy(s => s.Price))
                mod.AddOpt($"{ship.Price}$ {ship.TypeName}", () => PushModule(BuyShip(ship)));

            return mod;
        }

        private Msg BuyShip(Ship ship)
        {
            if (Player.CanAfford(ship.Price))
            {
                Sfx.Confirm.Play();
                Player.Buy(ship);
                var layout = ship.GetLayout();
                GameState.Map.SetShipLayout(layout);

                PopModule();
                PopModule();
                PushModule(HomeMenu());
                return new Msg(this,
                    Lns("Transaction Complete",
                    $"Purchased: {ship.TypeName}",
                    $"For: {ship.Price}$",
                    "Ship is now available",
                    "in station bay 1"),
                    () => PopModule());
            }
            else
            {
                Sfx.Reject.Play();
                return new Msg(this,
                    Lns("Transaction Failed",
                    "Insufficient funds",
                    $"Ship price: {ship.Price}$",
                    $"Credits Available: {Player.Credits}$"),
                    () => PopModule());
            }
        }

        private OSModule SellInvantory()
        {
            if (!Player.Ship.HasInventory || (Player.Ship is NoShip))
            {
                Sfx.Reject.Play();
                return new Msg(this,
                    Lns("Id10T Error",
                    "you have no inventory"),
                    () => PopModule());
            }
            else
            {
                var mnu = new Mnu(this, "Sell Goods");
                if (Player.Ship.CargoHold.Food.Quantity > 0)
                {
                    mnu.AddOpt($"1T Food {station.Rates.Food.Price}",
                        () => PushModule(Sell(Player.Ship.CargoHold.Food, station.Rates.Food.Price)));
                }
                if (Player.Ship.CargoHold.Textiles.Quantity > 0)
                {
                    mnu.AddOpt($"1T Textiles {station.Rates.Textiles.Price}",
                        () => PushModule(Sell(Player.Ship.CargoHold.Textiles, station.Rates.Textiles.Price)));
                }
                if (Player.Ship.CargoHold.Ore.Quantity > 0)
                {
                    mnu.AddOpt($"1T Ore {station.Rates.Ore.Price}",
                        () => PushModule(Sell(Player.Ship.CargoHold.Ore, station.Rates.Ore.Price)));
                }
                if (Player.Ship.CargoHold.Gems.Quantity > 0)
                {
                    mnu.AddOpt($"1T Gems {station.Rates.Gems.Price}",
                        () => PushModule(Sell(Player.Ship.CargoHold.Gems, station.Rates.Gems.Price)));
                }
                if (Player.Ship.CargoHold.Computers.Quantity > 0)
                {
                    mnu.AddOpt($"1T Computers {station.Rates.Computers.Price}",
                        () => PushModule(Sell(Player.Ship.CargoHold.Computers, station.Rates.Computers.Price)));
                }
                if (Player.Ship.CargoHold.Medicine.Quantity > 0)
                {
                    mnu.AddOpt($"1T Medicine {station.Rates.Medicine.Price}",
                        () => PushModule(Sell(Player.Ship.CargoHold.Medicine, station.Rates.Medicine.Price)));
                }
                return mnu;
            }
        }

        private OSModule Sell(Cargo cargo, int price)
        {
            if (cargo.Quantity <= 0)
            {
                Sfx.Reject.Play();
                return new Msg(this,
                    Lns("Transaction Failed",
                    "Bad programming"),
                    () => PopModule());
            }
            else
            {
                Sfx.Confirm.Play();
                PopModule();
                Player.Ship.RemoveInventory(cargo.Type);
                Player.Credit(price);
                PushModule(SellInvantory());
                return new Msg(this,
                    Lns("Transaction Complete",
                    $"Sold: {cargo.Name}",
                    $"For: {price}$",
                    "Supplies have been",
                    "removed from your ship"),
                    () => PopModule());
            }
        }

        private OSModule BuyGood(Good good)
        {
            if (Player.Ship is NoShip)
            {
                Sfx.Reject.Play();
                return new Msg(this,
                    Lns("Transaction Failed",
                    "No transfer target",
                    $"You must own a ship to",
                    $"purchase this supply"),
                    () => PopModule());
            }
            else if (!Player.CanAfford(good.Price))
            {
                Sfx.Reject.Play();
                return new Msg(this,
                    Lns("Transaction Failed",
                    "Insufficient funds",
                    $"Supply price: {good.Price}$",
                    $"Credits Available: {Player.Credits}$"),
                    () => PopModule());
            }
            else if (!Player.Ship.HasCargoCapacity)
            {
                Sfx.Reject.Play();
                return new Msg(this,
                    Lns("Transaction Failed",
                    "Ship cargo is at",
                    $"capacity of {Player.Ship.MaxCargo}T"),
                    () => PopModule());
            }
            else
            {
                Sfx.Confirm.Play();
                Player.Buy(good);
                return new Msg(this,
                    Lns("Transaction Complete",
                    $"Purchased: {good.Name}",
                    $"For: {good.Price}$",
                    "Supplies have been",
                    "transferred to your ship"),
                    () => PopModule());
            }
        }

        private Player Player => Computer.GameState.Player;
        private GameState GameState => Computer.GameState;

        public Action Todo => () => { };

        public string Lns(params string[] lines) =>
            string.Join("\n", lines);
    }

    class ShipOperatingSystem : OperatingSystem
    {
        public Ship Ship { get; set; }

        public ShipOperatingSystem(Ship ship) => Ship = ship;

        public override void Init() => PushModule(new Msg(
            this,
            Lns("SS Shippy McShipface", "System Idle"),
            () => PushModule(HomeMenu()))
        );

        private Mnu HomeMenu()
        {
            var mnu = new Mnu(this, "Ship Main Menu")
                .AddOpt("Inventory", () => PushModule(Inventory()))
                .AddOpt("Navigation", () => PushModule(Navigation()));
            return mnu;
        }

        private OSModule Inventory()
        {
            return new Msg(this,
                Lns($"Inventory\nCargo Capacity {Ship.MaxCargo}T",
                $"{Ship.CargoHold.Food.Quantity}t Food",
                $"{Ship.CargoHold.Textiles.Quantity}t Textiles",
                $"{Ship.CargoHold.Ore.Quantity}t Ore",
                $"{Ship.CargoHold.Gems.Quantity}t Gems",
                $"{Ship.CargoHold.Computers.Quantity}t Computers",
                $"{Ship.CargoHold.Medicine.Quantity}t Medicine"
                ), () => PopModule());
        }

        private OSModule Navigation()
        {
            var mnu = new Mnu(this, "Navigate to");
            mnu.AddOpt("trade stations", () => PushModule(SelectStation()));
            mnu.AddOpt("asteroids", () => PushModule(SelectAsteroid()));
            return mnu;
        }

        private OSModule SelectStation()
        {
            var mnu = new Mnu(this, "Navigate to station");
            var stations = GameState.GetNearbyStations(8);
            foreach (var station in stations)
                mnu.AddOpt(station.Name, () => PushModule(TradingStationInfo(station)));
            return mnu;
        }

        private OSModule SelectAsteroid()
        {
            var mnu = new Mnu(this, "Navigate to asteroid");
            var asteroids = GameState.GetNearbyAsteroids(8);
            foreach (var station in asteroids)
                mnu.AddOpt(station.Name, () => PushModule(AsteroidInfo(station)));
            return mnu;
        }

        private OSModule TradingStationInfo(Station station)
        {
            var distance = Vector2.Distance(GameState.ActiveEnvironment.Position,
                station.Position);
            distance = (float)Math.Round(distance, 2);

            return new Mnu(this,
                Lns("Trading Station",
                    $"Type: {station.Type}",
                    $"Distance: {distance}"))
                .AddOpt("Execute Jump", () => PushModule(JumpToStation(station)));
        }

        private OSModule JumpToStation(Station station)
        {
            var distance = Vector2.Distance(GameState.ActiveEnvironment.Position,
    station.Position);
            distance = (float)Math.Round(distance, 2);

            GameState.Map.SetEnvLayout(null);
            PopModule();
            PopModule();
            return new ProcRun(this, "JMPTO",
                (int)Math.Floor(distance), () =>
                {
                    PopModule();
                    GameState.LoadEnv(station);
                    PushModule(Arrived("Trading Station"));
                });
        }

        private OSModule AsteroidInfo(Asteroid asteroid)
        {
            var distance = Vector2.Distance(GameState.ActiveEnvironment.Position,
                asteroid.Position);
            distance = (float)Math.Round(distance, 2);

            return new Mnu(this,
                Lns("Asteroid",
                    $"Distance: {distance}"))
                .AddOpt("Execute Jump", () => PushModule(JumpToAsteroid(asteroid)));
        }

        private OSModule JumpToAsteroid(Asteroid asteroid)
        {
            var distance = Vector2.Distance(GameState.ActiveEnvironment.Position,
                asteroid.Position);
            distance = (float)Math.Round(distance, 2);

            Sfx.Confirm.Play();
            GameState.Map.SetEnvLayout(null);
            PopModule();
            PopModule();
            return new ProcRun(this, "JMPTO",
                (int)Math.Floor(distance), () =>
                {
                    PopModule();
                    GameState.LoadEnv(asteroid);
                    PushModule(Arrived("Asteroid"));
                });
        }

        private OSModule Arrived(string place) =>
            new Msg(this, Lns("JMPTO PROCESS complete",
                $"Welcome to the {place}"
                ), () => {
                    PopModule();
                });

        public Action Todo => () => { };

        private Player Player => Computer.GameState.Player;
        private GameState GameState => Computer.GameState;

        public string Lns(params string[] lines) =>
            string.Join("\n", lines);
    }
}
