using System;
using SystemBase.Core;
using Systems.Grid;
using UniRx;
using UnityEngine;

namespace Systems.GridRendering
{
    public class ForegroundCellComponent : GameComponent
    {
        public ReactiveProperty<ForegroundCellType> type = new(ForegroundCellType.Empty);
        public Texture[] images;
        [NonSerialized]public Renderer rendererCache;
    }
}