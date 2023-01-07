using UnityEngine;

namespace Systems.Grid
{
    public class GridUpdateMsg<TGridType>
    {
        public Vector2Int Coord { get; set; }
        public int Index { get; set; }
        public TGridType CellType { get; set; }
    }
}