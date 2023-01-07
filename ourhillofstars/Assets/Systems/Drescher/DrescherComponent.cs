using SystemBase.Core;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace Systems.Drescher
{
    public class DrescherComponent : GameComponent
    {
        public Vector2Int targetCellCoord;
        public IntReactiveProperty direction = new(0);
        public Renderer rendererCache;
        public Texture[] directionImages;

        public bool isMoving = false;
    }
}