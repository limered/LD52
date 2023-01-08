using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Systems.Grid
{
    public enum BackgroundCellType
    {
        Empty,
        Start,
        Grass,
        Wheat,
        Harvested,
        Basket,
        AppleBasket,
        Bush,
        Rock,
        Tree,
    }


    public static partial class GameGridExtensions
    {
        private static readonly Dictionary<Color32, BackgroundCellType> cellColorMap =
            new()
            {
                { new Color32(0, 0, 0, 255), BackgroundCellType.Empty },
                { new Color32(255, 0, 0, 255), BackgroundCellType.Start },
                { new Color32(0, 255, 0, 255), BackgroundCellType.Grass },
                { new Color32(255, 235, 4, 255), BackgroundCellType.Wheat },
                { new Color32(130, 78, 20, 255), BackgroundCellType.Harvested },
                { new Color32(61, 25, 0, 255), BackgroundCellType.Basket },
                { new Color32(255, 27, 195, 255), BackgroundCellType.AppleBasket },
                { new Color32(0, 72, 229, 255), BackgroundCellType.Bush },
                { new Color32(105, 105, 105, 255), BackgroundCellType.Rock },
                { new Color32(0, 69, 0, 255), BackgroundCellType.Tree },
            };

        public static BackgroundCellType ToCell(this Color32 cell)
        {
            if (!cellColorMap.Keys.Any(x => x.r == cell.r
                                            && x.g == cell.g && x.b == cell.b && x.a == cell.a))
                throw new Exception($"unknown color {cell}");
            return cellColorMap.First(x => x.Key.r == cell.r
                                           && x.Key.g == cell.g && x.Key.b == cell.b && x.Key.a == cell.a).Value;
        }
    }
}