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
            computer.Display.WriteLines("\nPRESS # OR 0 TO RETURN");
        }

        public void Select(int num)
        {
            if (num == 0)
            {
                computer.OS.PopModule();
                return;
            }

            var idx = num - 1;
            if (idx < 0 || idx >= _opts.Count) return;
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

        public void Enter() => OnEnter?.Invoke();

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
        public override void Init() => PushModule(new Msg(
            this,
            Lns("Station Terminal",
                "Your fully automated",
                "self service space",
                "station since the",
                "year 0x10c"),
            () => PushModule(HomeMenu()))
        );

        private Mnu HomeMenu()
        {
            var mnu = new Mnu(this, "Station Terminal Menu")
                .AddOpt("My Info", () => PushModule(AccountInfo()))
                .AddOpt("Buy Ship", () => PushModule(BuyShip()));

            if (!(Computer.GameState.Player.Ship is NoShip))
                mnu.AddOpt("Buy Ship Upgrades", Todo);

            if (Computer.GameState.Player.Ship.HasInventory)
                mnu.AddOpt("Sell", () => { });

            return mnu; 
                
        }

        private Msg AccountInfo() =>
            new Msg(this,
                Lns("My Information",
                    $"CREDITS: {Computer.GameState.Player.Credits}$",
                    $"SHIP: {Computer.GameState.Player.Ship.TypeName}"
                ),
                () => PopModule());

        private Mnu BuyShip()
        {
            var shipsAvail = GameState.Station.GetAvailableShips();

            var mod = new Mnu(this, "Buy Ship");
            foreach (var ship in shipsAvail.OrderBy(s => s.Price))
                mod.AddOpt($"{ship.Price}$ {ship.TypeName}", () => PushModule(BuyShip(ship)));

            return mod;
        }

        private Msg BuyShip(Ship ship)
        {
            if (Player.CanAfford(ship.Price))
            {
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
                return new Msg(this,
                    Lns("Transaction Failed",
                    $"Ship price: {ship.Price}$",
                    $"Credits Available: {Player.Credits}$"),
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
                .AddOpt("Navigation", () => PushModule(Navigation()));
            return mnu;
        }

        private OSModule Navigation()
        {
            var mnu = new Mnu(this, "Destinations");

            if (GameState.ActiveEnvironment is Station)
                mnu.AddOpt("Mining Asteroid", () => PushModule(AsteroidInfo()));

            if (GameState.ActiveEnvironment is Asteroid)
                mnu.AddOpt("Trading Station", () => PushModule(TradingStationInfo()));

            return mnu;
        }

        private OSModule TradingStationInfo()
        {
            return new Mnu(this,
                Lns("Trading Station",
                    "Distance: TODO"))
                .AddOpt("Execute Jump", () => PushModule(JumpToStation()));
        }

        private OSModule JumpToStation()
        {
            GameState.Map.SetEnvLayout(null);
            PopModule();
            PopModule();
            return new ProcRun(this, "JMPTO",
                2, () =>
                {
                    PopModule();
                    GameState.LoadStation();
                    PushModule(Arrived("Trading Station"));
                });
        }

        private OSModule AsteroidInfo()
        {
            return new Mnu(this,
                Lns("Asteroid",
                    "Distance: TODO",
                    "Resources: TODO"))
                .AddOpt("Execute Jump", () => PushModule(JumpToAsteroid()));
        }

        private OSModule JumpToAsteroid()
        {
            GameState.Map.SetEnvLayout(null);
            PopModule();
            PopModule();
            return new ProcRun(this, "JMPTO",
                2, () =>
                {
                    PopModule();
                    GameState.LoadAsteroid();
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
