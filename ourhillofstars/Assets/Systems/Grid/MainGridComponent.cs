﻿using SystemBase.Core;
using UniRx;
using UnityEngine;

namespace Systems.Grid
{
    public class MainGridComponent : GameComponent
    {
        public Vector2Int dimensions = new(13, 13);
        public float updateAnimationDelay = 0.01f;
        public GameObject[] backgroundCells;
        public GameObject[] foregroundCells;
        public readonly ReactiveProperty<MainGridComponent> gridsInitialized = new(null);
        public readonly ReactiveCommand<MainGridComponent> gridUpdated = new();
        public GameGrid<BackgroundCellType> backgroundGrid;
        public GameGrid<ForegroundCellType> foregroundGrid;
    }
}