using System;
using SystemBase.Core;
using Systems.Grid;
using UniRx;
using UnityEngine;

namespace Systems.GridRendering
{
    public class BackgroundCellComponent : GameComponent
    {
        public ReactiveProperty<BackgroundCellType> type = new(BackgroundCellType.Empty);
        public Texture[] images;
        [NonSerialized]public Renderer rendererCache;
    }
}