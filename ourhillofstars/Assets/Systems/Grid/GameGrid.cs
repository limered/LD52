using UniRx;
using UnityEngine;

namespace Systems.Grid
{
    public class GameGrid
    {
        private readonly GridCellType[] _grid;
        private readonly int _x;
        private int _y;

        public GameGrid(int x, int y)
        {
            _x = x;
            _y = y;
            _grid = new GridCellType[x * y];
            Clear();
        }

        public void Cell(int index, GridCellType cell)
        {
            _grid[index] = cell;

            var x = index % _x;
            var y = index / _x;

            MessageBroker.Default.Publish(new GridUpdateMsg
                { Coord = new Vector2Int(x, y), Index = index, CellType = cell });
        }

        public void Cell(int x, int y, GridCellType cell)
        {
            var i = y * _x + x;
            _grid[i] = cell;

            MessageBroker.Default.Publish(
                new GridUpdateMsg { Coord = new Vector2Int(x, y), Index = i, CellType = cell });
        }

        public GridCellType Cell(int x, int y)
        {
            var i = y * _x + x;
            return _grid[i];
        }

        public void Clear()
        {
            for (var i = 0; i < _grid.Length; i++) _grid[i] = GridCellType.Empty;
        }
    }
}