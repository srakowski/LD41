namespace LD41.Gameplay
{
    public class Supply
    {
        public Supply(string name, int quantity, int price)
        {
            Name = name;
            Price = price;
        }

        public string Name { get; }
        public int Price { get; }
    }
}
