using SystemBase.Core;
using Systems.Levels;
using UniRx;
using UnityEngine;

namespace Systems.Drescher
{
    public class DrescherComponent : GameComponent
    {
        public Vector2Int targetCellCoord;
        public ReactiveProperty<DrescherDirection> direction = new(0);
        public Renderer rendererCache;
        public Texture2D[] directionImages;

        public bool isMoving = false;

        public float speed = 5f;

        public void Reset(Vector2Int startCoord, DrescherDirection dir)
        {
            targetCellCoord = startCoord;
            transform.position = new Vector3(startCoord.x, 0.5f, startCoord.y);
            direction.Value = dir;
        }
    }
}