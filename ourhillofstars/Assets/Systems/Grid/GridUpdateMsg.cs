using UnityEngine;

namespace Systems.Grid
{
    public class GridUpdateMsg
    {
        public Vector2Int Coord { get; set; }
        public int Index { get; set; }
        public GridCellType CellType { get; set; }
    }
}