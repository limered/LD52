using UnityEngine;

namespace Systems.Grid
{
    public static partial class GameGridExtensions
    {
        public static Vector2Int? FindStartCoord(this GameGrid<BackgroundCellType> g)
        {
            for (var i = 0; i < g.Length; i++)
            {
                if (g.Cell(i) == BackgroundCellType.Start)
                {
                    return g.IndexToCoord(i);
                }
            }

            return null;
        }

        public static void ResetHarvested(this GameGrid<BackgroundCellType> g)
        {
            for (var i = 0; i < g.Length; i++)
            {
                if (g.Cell(i) == BackgroundCellType.Harvested)
                {
                    g.Cell(i, BackgroundCellType.Wheat);
                }
            }
        }
    }
}