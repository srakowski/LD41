namespace LD41.Gameplay
{
    abstract class Cell
    {
        public class None : Cell { }

        public class ShipVoid : Cell { }
        public class ShipWall : Cell { }
        public class ShipWindowLeft : Cell { }
        public class ShipWindowRight : Cell { }
        public class ShipComputer : Cell { }
        public class ShipSystem : Cell { }
        public class ShipExit : Cell { }

        public class AsteroidVoid : Cell { }
        public class AsteroidEntry : Cell { }
        public class AsteroidRock : Cell { }
        public class AsteroidResource : Cell { }
        public class AsteroidBoundary : Cell { }

        public class StationVoid : Cell { }
        public class StationWall : Cell { }
        public class StationComputer : Cell { }
        public class StationExit : Cell { }

        public static Cell Get(char charCode)
        {
            switch (charCode)
            {
                case '_': return new ShipVoid();
                case '#': return new ShipWall();
                case '[': return new ShipWindowLeft();
                case ']': return new ShipWindowRight();
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
                case '=': return new StationExit();
                
                default: return new None();
            }
        }

        public static Cell Default => new None();
    }
}
