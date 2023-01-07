using System;
using System.Linq;
using SystemBase.Core;
using SystemBase.Utils;
using Systems.Grid;
using Systems.GridInteraction.Events;
using Systems.GridRendering;
using Systems.Levels;
using Systems.Selector;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Systems.GridInteraction
{
    [GameSystem]
    public class GridInteractionSystem : GameSystem<MainGridComponent>
    {
        public override void Register(MainGridComponent grid)
        {
            grid.gridsInitialized
                .WhereNotNull()
                .Subscribe(InitializeInteractions)
                .AddTo(grid);
        }

        private void InitializeInteractions(MainGridComponent grid)
        {
            var selectorPrefab = IoC.Game.PrefabByName("Selector");
            var selector = Object.Instantiate(selectorPrefab, new Vector3(0, 1, 0), Quaternion.identity);
            var selectorComponent = selector.GetComponent<SelectorComponent>();
            SystemUpdate(grid)
                .Subscribe(g => StartInteraction(g, selectorComponent))
                .AddTo(grid);
        }

        private static void StartInteraction(MainGridComponent grid, SelectorComponent selector)
        {
            var fGrid = grid.foregroundGrid;
            var fGridComponent = grid.GetComponentInChildren<ForegroundParentComponent>();
            var ray = fGridComponent.mainCamera.ScreenPointToRay(Input.mousePosition);
            var bGrid = grid.backgroundGrid;

            if (!Physics.Raycast(ray, out var hit))
            {
                selector.shouldBeInvisible.Value = false;
                return;
              
               
            }

            var x = (int)(hit.point.x + 0.5);
            var y = (int)(hit.point.z + 0.5);

            selector.targetCoord = new Vector2Int(x, y);
            selector.shouldBeInvisible.Value = true;
            var cell = bGrid.Cell(x, y);
            if (cell != BackgroundCellType.Harvested && cell != BackgroundCellType.Wheat &&
                cell != BackgroundCellType.Start)
            {
                selector.shouldChangeTexture.Value = true;
                return;
            }

            selector.shouldChangeTexture.Value = false;

            if (!Input.GetMouseButtonDown(0)) return;
            var maxValue = Enum.GetValues(typeof(ForegroundCellType)).Cast<int>().Last() + 1;
            var nextCellType = (int)(fGrid.Cell(x, y)) + 1;
            nextCellType %= maxValue;
            fGrid.Cell(x, y, (ForegroundCellType)nextCellType);
            var foregroundCellType = (ForegroundCellType)nextCellType;
            MessageBroker.Default
            .Publish(
                new SetForegroundCellTypeMessage()
                {
                        foregroundCellType = foregroundCellType
                }
            );
        }

        private static void SetAmountOfArrows(ForegroundCellType foregroundCellType)
        {
            var currentLevelComponent = IoC.Game.GetComponent<CurrentLevelComponent>();
            
            switch (foregroundCellType)
            {
                case ForegroundCellType.Empty:
                    currentLevelComponent.bottomArrow.Value += 1;
                    break;
                case ForegroundCellType.Left:
                    currentLevelComponent.topArrow.Value -= 1;
                    break;
                case ForegroundCellType.Top:
                    currentLevelComponent.leftArrow.Value -= 1;
                    currentLevelComponent.topArrow.Value += 1;
                    break;
                case ForegroundCellType.Right:
                    currentLevelComponent.rightArrow.Value -= 1;
                    currentLevelComponent.leftArrow.Value += 1;
                    break;
                case ForegroundCellType.Bottom:
                    currentLevelComponent.bottomArrow.Value -= 1;
                    currentLevelComponent.rightArrow.Value += 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}