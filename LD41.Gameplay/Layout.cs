namespace LD41.Gameplay
{
    class Layout
    {
        private Cell[,] _cells;

        public Layout(Cell[,] cells)
        {
            Height = cells.GetLength(0);
            Width = cells.GetLength(1);
            _cells = cells;
        }

        public int Width { get; }

        public int Height { get; }

        internal static Layout FromChars(char[,] layout)
        {
            var height = layout.GetLength(0);
            var width = layout.GetLength(1);
            var cells = new Cell[height, width];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    cells[y, x] = Cell.Create(layout[y, x]);

            return new Layout(cells);
        }

        public Cell this[int x, int y] =>
            (x < 0 || x >= Width || y < 0 || y >= Height)
                ? Cell.Default
                : _cells[y, x];

        internal void Replace(Cell cell, Cell with)
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    if (_cells[y, x] == cell)
                    {
                        _cells[y, x] = with;
                        return;
                    }
        }
    }
}
