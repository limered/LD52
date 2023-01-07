using System;
using SystemBase.Core;
using SystemBase.Utils;
using Systems.Grid;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Systems.Drescher
{
    [GameSystem]
    public class DrescherSystem : GameSystem<DrescherComponent, MainGridComponent>
    {
        private readonly ReactiveProperty<MainGridComponent> _grid = new();

        public override void Register(DrescherComponent component)
        {
            component.rendererCache = component.GetComponent<Renderer>();
            component.direction
                .Subscribe(i => UpdateDirection(i, component))
                .AddTo(component);

            _grid.WhereNotNull()
                .Subscribe(g =>
                {
                    SystemUpdate(component)
                        .Subscribe(drescher => Drive(drescher, g))
                        .AddTo(component);
                })
                .AddTo(component);
        }

        private void SpawnDrescher(MainGridComponent grid)
        {
            var startCoord = grid.backgroundGrid.FindStartCoord();
            if (startCoord == null) throw new NullReferenceException("Level Missing Startpoint");
            var position = new Vector3((float)startCoord?.x, 0.5f, (float)startCoord?.y);
            var drescherPrefab = IoC.Game.PrefabByName("Drescher");
            var drescher = Object.Instantiate(drescherPrefab, position, quaternion.Euler(0, 0, 0));
            var target = new Vector2Int((int)startCoord?.x, (int)startCoord?.y);
            drescher.GetComponent<DrescherComponent>().targetCellCoord = target;
        }

        private static void Drive(DrescherComponent drescherComponent, MainGridComponent g)
        {
            var position = drescherComponent.transform.position.XZ();
            
            if ((position - drescherComponent.targetCellCoord).magnitude < 0.1f)
                drescherComponent.isMoving = false;
            else
                drescherComponent.isMoving = true;


            if (drescherComponent.isMoving)
            {
                var nextPosition = Vector2.Lerp(position, drescherComponent.targetCellCoord, 0.1f);
                drescherComponent.transform.position = new Vector3(nextPosition.x, 0.5f, nextPosition.y);
                return;
            }

            // switch direction
            var newDirection = drescherComponent.direction.Value;
            var arrowDirType = g.foregroundGrid.Cell(drescherComponent.targetCellCoord.x, drescherComponent.targetCellCoord.y);
            switch (arrowDirType)
            {
                case ForegroundCellType.Empty:
                    break;
                case ForegroundCellType.Left:
                    newDirection = 1;
                    break;
                case ForegroundCellType.Top:
                    newDirection = 0;
                    break;
                case ForegroundCellType.Right:
                    newDirection = 3;
                    break;
                case ForegroundCellType.Bottom:
                    newDirection = 2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            drescherComponent.direction.Value = newDirection;
            
            // set target
            
            var nextCellCoord = drescherComponent.targetCellCoord;
            var maxX = g.dimensions.x;
            var maxY = g.dimensions.y;
            switch (drescherComponent.direction.Value)
            {
                case 0:
                    nextCellCoord.y += 1;
                    break;
                case 1:
                    nextCellCoord.x -= 1;
                    break;
                case 2:
                    nextCellCoord.y -= 1;
                    break;
                case 3:
                    nextCellCoord.x += 1;
                    break;
            }

            if (nextCellCoord.x < 0 || nextCellCoord.x >= maxX || nextCellCoord.y < 0 ||
                nextCellCoord.y >= maxY) return;

            var nextCellType = g.backgroundGrid.Cell(nextCellCoord.x, nextCellCoord.y);
            if (nextCellType != BackgroundCellType.Harvested && 
                nextCellType != BackgroundCellType.Wheat &&
                nextCellType != BackgroundCellType.Start) return;

            drescherComponent.targetCellCoord = nextCellCoord;



            // check for win || lose
        }


        private static void UpdateDirection(int i, DrescherComponent component)
        {
            component.rendererCache.material.mainTexture = component.directionImages[i];
        }

        public override void Register(MainGridComponent g)
        {
            _grid.Value = g;

            MessageBroker.Default.Receive<SpawnPlayerMessage>()
                .Subscribe(_ => SpawnDrescher(g))
                .AddTo(g);
        }
    }
}