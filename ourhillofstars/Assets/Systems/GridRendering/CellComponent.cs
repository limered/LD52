using System;
using SystemBase.Core;
using Systems.Grid;
using UniRx;
using UnityEngine;

namespace Systems.GridRendering
{
    public class CellComponent : GameComponent
    {
        public ReactiveProperty<GridCellType> type = new(GridCellType.Empty);
        public Texture[] images;
        [NonSerialized]public Renderer rendererCache;
    }
}