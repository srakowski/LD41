using System;

namespace LD41.Gameplay
{
    abstract class Ship
    {
        public Ship(int price, string typeName, int maxCargo)
        {
            Price = price;
            TypeName = typeName;
            Computer = new Computer();
            if (Computer != null)
                Computer.OS = new ShipOperatingSystem(this);
            MaxCargo = maxCargo;
        }

        public int Price { get; }

        public string TypeName { get; }

        public int MaxCargo { get; }

        public virtual bool HasInventory => (CargoHold.Food.Quantity +
            CargoHold.Textiles.Quantity +
            CargoHold.Ore.Quantity +
            CargoHold.Gems.Quantity +
            CargoHold.Computers.Quantity +
            CargoHold.Medicine.Quantity) > 0;

        public abstract Layout GetLayout();

        public Computer Computer { get; }

        public CargoHold CargoHold { get; } = new CargoHold();

        public bool HasCargoCapacity =>
            (CargoHold.Food.Quantity +
            CargoHold.Textiles.Quantity +
            CargoHold.Ore.Quantity +
            CargoHold.Gems.Quantity +
            CargoHold.Computers.Quantity +
            CargoHold.Medicine.Quantity) < MaxCargo;

        internal void AddInventory(Good good)
        {
            if (!HasCargoCapacity) return;
            if (good.Type == GoodsTypes.Food) CargoHold.Food.Quantity += 1;
            if (good.Type == GoodsTypes.Textiles) CargoHold.Textiles.Quantity += 1;
            if (good.Type == GoodsTypes.Ore) CargoHold.Ore.Quantity += 1;
            if (good.Type == GoodsTypes.Gems) CargoHold.Gems.Quantity += 1;
            if (good.Type == GoodsTypes.Computers) CargoHold.Computers.Quantity += 1;
            if (good.Type == GoodsTypes.Medicine) CargoHold.Medicine.Quantity += 1;
        }

        internal void RemoveInventory(GoodsTypes type)
        {
            if (type == GoodsTypes.Food) CargoHold.Food.Quantity -= 1;
            if (type == GoodsTypes.Textiles) CargoHold.Textiles.Quantity -= 1;
            if (type == GoodsTypes.Ore) CargoHold.Ore.Quantity -= 1;
            if (type == GoodsTypes.Gems) CargoHold.Gems.Quantity -= 1;
            if (type == GoodsTypes.Computers) CargoHold.Computers.Quantity -= 1;
            if (type == GoodsTypes.Medicine) CargoHold.Medicine.Quantity -= 1;
        }
    }

    public class CargoHold
    {
        public Cargo Food { get; } = new Cargo(GoodsTypes.Food);
        public Cargo Textiles { get; } = new Cargo(GoodsTypes.Textiles);
        public Cargo Ore { get; } = new Cargo(GoodsTypes.Ore);
        public Cargo Gems { get; } = new Cargo(GoodsTypes.Gems);
        public Cargo Computers { get; } = new Cargo(GoodsTypes.Computers);
        public Cargo Medicine { get; } = new Cargo(GoodsTypes.Medicine);
    }

    public class Cargo
    {
        public Cargo(GoodsTypes types) => Type = types;
        public string Name => Type.ToString();
        public GoodsTypes Type { get; set; }
        public int Quantity { get; set; }
    }

    class NoShip : Ship
    {
        public NoShip() : base(0, "NONE", 0)
        {
        }

        public override bool HasInventory => false;

        public override Layout GetLayout() => throw new System.NotImplementedException();
    }

    class BasicMiningShip : Ship
    {
        private static readonly char[,] layout = new char[7, 5]
        {
            {' ','#','C','#',' ' },
            {'#','#','_','#','#' },
            {'?','_','_','_','?' },
            {'[','_','_','_',']' },
            {'?','_','_','_','?' },
            {'#','#','_','#','#' },
            {' ',':','+',':',' ' },
        };

        public BasicMiningShip() : base(4000, "Basic Trading", 8)
        {
        }

        public override Layout GetLayout() => Layout.FromChars(layout);
    }

    class MediumUtilityShip : Ship
    {
        private static readonly char[,] layout = new char[10, 9]
        {
            {' ',' ',' ','#','C','#',' ',' ',' ',},
            {' ',' ','#','#','_','#','#',' ',' ',},
            {' ',' ','[','_','_','_',']',' ',' ',},
            {' ',' ','[','_','_','_',']',' ',' ',},
            {'#','?','#','_','_','_','#','?','#',},
            {'[','_','_','_','_','_','_','_',']',},
            {'[','_','#','#','_','#','#','_',']',},
            {'#','_','#','?','_','?','#','_','#',},
            {'#','?','#','#','_','#','#','?','#',},
            {' ',' ',' ',':','+',':',' ',' ',' ',},
        };

        public MediumUtilityShip() : base(1000, "Med Utility", 12)
        {
        }

        public override Layout GetLayout() => Layout.FromChars(layout);
    }
}
