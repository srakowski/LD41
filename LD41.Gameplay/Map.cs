namespace LD41.Gameplay
{
    class Map
    {
        private Cell[,] _cells;

        public Map(char[,] cells)
        {
            Height = cells.GetLength(0);
            Width = cells.GetLength(1);
            _cells = new Cell[Height, Width];
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    _cells[y, x] = Cell.Get(cells[y, x]);
        }

        public int Width { get; }

        public int Height { get; }

        public Cell this[int x, int y] => 
            (x < 0 || x >= Width || y < 0 || y >= Height)
                ? Cell.Default
                : _cells[y, x];
    }
}
