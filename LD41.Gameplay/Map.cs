using System;

namespace LD41.Gameplay
{
    class Map
    {
        private Cell[,] _cells;

        private Layout _shipLayout;

        private Layout _envLayout;

        public Map(Layout shipLayout, Layout envLayout)
        {
            Height = 200;
            Width = 100;
            _cells = new Cell[Height, Width];
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    _cells[y, x] = new Cell.None();

            SetShipLayout(shipLayout);
            SetEnvLayout(envLayout);
        }

        public int Width { get; }

        public int Height { get; }

        public Cell this[int x, int y] => 
            (x < 0 || x >= Width || y < 0 || y >= Height)
                ? Cell.Default
                : _cells[y, x];

        public void SetShipLayout(Layout shipLayout)
        {
            _shipLayout = shipLayout;
            var destMidY = Height / 2;
            var destMidX = Width / 2;
            for (int y = 0; y < destMidY; y++)
                for (int x = 0; x < Width; x++)
                    _cells[y, x] = new Cell.None();

            if (_shipLayout == null)
            {
                if (_envLayout != null && _cells[destMidY, destMidX] is Cell.StationEntryVoid)
                    _cells[destMidY, destMidX] = new Cell.StationEntry();
                else if (_envLayout != null && _cells[destMidY, destMidX] is Cell.AsteroidEntryVoid)
                    _cells[destMidY, destMidX] = new Cell.AsteroidEntry();

                return;
            }

            var destY = destMidY - shipLayout.Height;
            var destX = destMidX - shipLayout.Width / 2;
            for (int y = 0; y < shipLayout.Height; y++)
                for (int x = 0; x < shipLayout.Width; x++)
                {
                    var cell = shipLayout[x, y];
                    if (_envLayout != null && cell is Cell.ShipExit)
                        cell = new Cell.ShipExitVoid();
                    _cells[destY + y, destX + x] = cell;
                }

            if (_envLayout != null && _cells[destMidY, destMidX] is Cell.StationEntry)
                _cells[destMidY, destMidX] = new Cell.StationEntryVoid();
            if (_envLayout != null && _cells[destMidY, destMidX] is Cell.AsteroidEntry)
                _cells[destMidY, destMidX] = new Cell.AsteroidEntryVoid();
        }

        public void SetEnvLayout(Layout envLayout)
        {
            _envLayout = envLayout;
            var destMidY = Height / 2;
            var destMidX = Width / 2;
            for (int y = destMidY; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    _cells[y, x] = new Cell.None();

            if (_envLayout == null)
            {
                if (_shipLayout != null && _cells[destMidY - 1, destMidX] is Cell.ShipExitVoid)
                    _cells[destMidY - 1, destMidX] = new Cell.ShipExit();

                return;
            }

            var destY = destMidY;
            var destX = destMidX - envLayout.Width / 2;
            for (int y = 0; y < envLayout.Height; y++)
                for (int x = 0; x < envLayout.Width; x++)
                {
                    var cell = envLayout[x, y];
                    if (_shipLayout != null && cell is Cell.StationEntry)
                        cell = new Cell.StationEntryVoid();
                    if (_shipLayout != null && cell is Cell.AsteroidEntry)
                        cell = new Cell.AsteroidEntryVoid();
                    _cells[destY + y, destX + x] = cell;
                }

            if (_shipLayout != null && _cells[destMidY - 1, destMidX] is Cell.ShipExit)
                _cells[destMidY - 1, destMidX] = new Cell.ShipVoid();
        }
    }
}
