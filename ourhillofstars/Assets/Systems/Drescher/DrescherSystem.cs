using System;
using SystemBase.Core;
using SystemBase.Utils;
using Systems.Grid;
using Systems.Levels;
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
            var drescher = Object.Instantiate(drescherPrefab, position, drescherPrefab.transform.rotation);
            var target = new Vector2Int((int)startCoord?.x, (int)startCoord?.y);
            drescher.GetComponent<DrescherComponent>().targetCellCoord = target;
        }

        private static void Drive(DrescherComponent drescherComponent, MainGridComponent g)
        {
            var currentLevel = IoC.Game.GetComponent<CurrentLevelComponent>();
            if (!currentLevel.harvesterRunning.Value)
            {
                drescherComponent.Reset((Vector2Int)g.backgroundGrid.FindStartCoord());
                g.backgroundGrid.ResetHarvested();
            }
            
            var position = drescherComponent.transform.position.XZ();
            drescherComponent.isMoving = !DrescherReachedTarget(drescherComponent, position);

            if (drescherComponent.isMoving)
            {
                AnimateDrescherToNextCell(drescherComponent, position);
                return;
            }

            SwitchDrescherDirection(drescherComponent, g);
            CheckNextCellAndSwitchTarget(drescherComponent, g);
            Harvest(g, position);
            
            // check for win || lose
        }

        private static void Harvest(MainGridComponent g, Vector2 position)
        {
            var cellCoord = new Vector2Int((int)(position.x + 0.5f), (int)(position.y + 0.5f));
            var currentCelType = g.backgroundGrid.Cell(cellCoord.x, cellCoord.y);
            if (currentCelType != BackgroundCellType.Wheat) return;
            
            g.backgroundGrid.Cell(cellCoord.x, cellCoord.y, BackgroundCellType.Harvested);
            MessageBroker.Default.Publish(new HarvestedMsg { coord = cellCoord });
        }

        private static void CheckNextCellAndSwitchTarget(DrescherComponent drescherComponent, MainGridComponent g)
        {
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
        }

        private static void SwitchDrescherDirection(DrescherComponent drescherComponent, MainGridComponent g)
        {
            var newDirection = drescherComponent.direction.Value;
            var arrowDirType =
                g.foregroundGrid.Cell(drescherComponent.targetCellCoord.x, drescherComponent.targetCellCoord.y);
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
        }

        private static void AnimateDrescherToNextCell(DrescherComponent drescherComponent, Vector2 position)
        {
            var direction = (drescherComponent.targetCellCoord - position).normalized; 
            var nextPosition = position + direction * drescherComponent.speed * Time.deltaTime;
            
            drescherComponent.transform.position = new Vector3(nextPosition.x, 0.5f, nextPosition.y);
        }

        private static bool DrescherReachedTarget(DrescherComponent drescherComponent, Vector2 position)
        {
            return (position - drescherComponent.targetCellCoord).magnitude < 0.1f;
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