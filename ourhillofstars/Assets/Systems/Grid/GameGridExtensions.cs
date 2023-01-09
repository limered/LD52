using System.Collections.Generic;
using UnityEngine;

namespace Systems.Grid
{
    public static partial class GameGridExtensions
    {
        public static Vector2Int? FindStartCoord(this GameGrid<BackgroundCellType> g)
        {
            for (var i = 0; i < g.Length; i++)
                if (g.Cell(i) == BackgroundCellType.Start)
                    return g.IndexToCoord(i);

            return null;
        }

        public static void ResetHarvested(this GameGrid<BackgroundCellType> g)
        {
            for (var i = 0; i < g.Length; i++)
                if (g.Cell(i) == BackgroundCellType.Harvested)
                    g.Cell(i, BackgroundCellType.Harvestable);
        }

        public static int CountElementsOfType(this GameGrid<ForegroundCellType> grid, ForegroundCellType type)
        {
            var count = 0;
            for (var i = 0; i < grid.Length; i++)
                if (grid.Cell(i) == type)
                    count++;
            return count;
        }
        
        public static int CountElementsOfType(this GameGrid<BackgroundCellType> grid, BackgroundCellType type)
        {
            var count = 0;
            for (var i = 0; i < grid.Length; i++)
                if (grid.Cell(i) == type)
                    count++;
            return count;
        }

        public static Vector2Int[] GetCoordinatesOfType(this GameGrid<BackgroundCellType> grid, BackgroundCellType type)
        {
            var result = new List<Vector2Int>();
            for (var i = 0; i < grid.Length; i++)
                if (grid.Cell(i) == type)
                {
                    
                    result.Add(grid.IndexToCoord(i));
                }

            return result.ToArray();
        }
    }
}