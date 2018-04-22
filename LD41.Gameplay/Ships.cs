namespace LD41.Gameplay
{
    abstract class Ship
    {
        public Ship(int price, string typeName, Computer computer)
        {
            Price = price;
            TypeName = typeName;
            Computer = computer;
            if (Computer != null)
                Computer.OS = new ShipOperatingSystem(this);
        }

        public int Price { get; }

        public string TypeName { get; }

        public virtual bool HasInventory { get; } = false;

        public abstract Layout GetLayout();

        public Computer Computer { get; }
    }

    class NoShip : Ship
    {
        public NoShip() : base(0, "NONE", null)
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
            {' ','#','+','#',' ' },
        };

        public BasicMiningShip(Computer computer) : base(4000, "Basic Mining", computer)
        {
        }

        public override Layout GetLayout() => Layout.FromChars(layout);
    }
}
