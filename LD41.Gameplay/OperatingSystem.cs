namespace LD41.Gameplay
{
    abstract class OperatingSystem
    {
        public Computer Computer { get; set; }
        public abstract void Run();
    }

    class StationOperatingSystem : OperatingSystem
    {
        public override void Run()
        {
            Computer.Display
                .WriteLines("STATION TERMINAL 1");
        }
    }
}
