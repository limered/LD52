using SystemBase.Core;
using UniRx;
using UnityEngine;

namespace Systems.Grid
{
    public class MainGridComponent : GameComponent
    {
        public Vector2Int dimensions = new(13, 13);
        public readonly ReactiveCommand<MainGridComponent> gridLoaded = new();
        public readonly ReactiveCommand<MainGridComponent> gridUpdated = new();
        public GameGrid grid;
        public GameObject[] Cells;
    }
}