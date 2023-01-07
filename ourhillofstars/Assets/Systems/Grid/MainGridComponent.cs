using SystemBase.Core;
using UniRx;
using UnityEngine;

namespace Systems.Grid
{
    public class MainGridComponent : GameComponent
    {
        public Vector2Int dimensions = new(13, 13);
        public GameObject[] backgroundCells;
        public GameObject[] foregroundCells;
        public readonly ReactiveCommand<MainGridComponent> gridsInitialized = new();
        public readonly ReactiveCommand<MainGridComponent> gridUpdated = new();
        public GameGrid<BackgroundCellType> backgroundGrid;
        public GameGrid<ForegroundCellType> foregroundGrid;
    }
}