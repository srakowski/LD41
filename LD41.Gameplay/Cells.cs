namespace LD41.Gameplay
{
    abstract class Cell
    {
        public class None : Cell { }
        public abstract class VoidCell : Cell
        {
            public abstract string CeilingTextureName { get; }
        }
        public class Threshold : Cell { }

        public class ShipVoid : VoidCell
        {
            public override string TextureName => "ShipFloor";
            public override string CeilingTextureName => "ShipCeiling";
        }
        public class ShipWall : Cell { }
        public class ShipWindow : Cell
        {
            public ShipWindow(bool isLeft) => IsLeftSide = isLeft;
            public bool IsLeftSide { get; }
        }
        public class ShipComputer : Cell { }
        public class ShipSystem : Cell { }
        public class ShipExit : Cell { }

        public class AsteroidVoid : VoidCell
        {
            public override string TextureName => "AsteroidFloor";
            public override string CeilingTextureName => "AsteroidCeiling";
        }
        public class AsteroidEntry : Cell { }
        public class AsteroidRock : Cell { }
        public class AsteroidResource : Cell { }
        public class AsteroidBoundary : Cell { }

        public interface StationCell { }
        public class StationVoid : VoidCell
        {
            public override string TextureName => "StationFloor";
            public override string CeilingTextureName => "StationCeiling";
        }
        public class StationWall : Cell { }
        public class StationComputer : Cell { }
        public class StationEntry : Cell { }
        public class StationBaySign : Cell
        {
            public StationBaySign(int num) => Num = num;
            public int Num { get; }
        }

        public static Cell Get(char charCode)
        {
            switch (charCode)
            {
                case '_': return new ShipVoid();
                case '#': return new ShipWall();
                case '[': return new ShipWindow(isLeft: true);
                case ']': return new ShipWindow(isLeft: false);
                case 'C': return new ShipComputer();
                case '?': return new ShipSystem();
                case '+': return new ShipExit();

                case ',': return new AsteroidVoid();
                case '-': return new AsteroidEntry();
                case 'R': return new AsteroidRock();
                case '$': return new AsteroidResource();
                case '!': return new AsteroidBoundary();

                case '.': return new StationVoid();
                case '%': return new StationWall();
                case 'c': return new StationComputer();
                case '=': return new StationEntry();
                case '1': return new StationBaySign(1);
                case '2': return new StationBaySign(2);
                case '3': return new StationBaySign(3);
                case '4': return new StationBaySign(4);

                case ':': return new Threshold();
                default: return new None();
            }
        }

        public static Cell Default => new None();

        public virtual string TextureName => GetType().Name;
    }
}
