using System;
using System.Linq;
using SystemBase.Core;
using SystemBase.Utils;
using Systems.Grid;
using Systems.GridRendering;
using Systems.Levels;
using Systems.Selector;
using Systems.UI;
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
            var currentLevelComponent = IoC.Game.GetComponent<CurrentLevelComponent>();
            if (currentLevelComponent.harvesterRunning.Value) return;
            if (currentLevelComponent.IsPaused) return;

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
            var nextCellType = (ForegroundCellType)NextCellType(fGrid, x, y);
            var currLevel = IoC.Game.GetComponent<CurrentLevelComponent>();
            var tries = 1;
            while (!CanUseArrow(grid.foregroundGrid, nextCellType, currLevel) && tries < 5)
            {
                tries++;
                nextCellType = (ForegroundCellType)NextCellType(fGrid, x, y, tries);
            }

            if (!CanUseArrow(grid.foregroundGrid, nextCellType, currLevel)) return;
            
            fGrid.Cell(x, y, nextCellType);
            SetAmountOfArrows(fGrid);
        }

        private static bool CanUseArrow(
            GameGrid<ForegroundCellType> grid,
            ForegroundCellType nextCellType, 
            CurrentLevelComponent currLevel)
        {
            var usedArrowsOfType = grid.CountElementsOfType(nextCellType);
            return nextCellType switch
            {
                ForegroundCellType.Empty => true,
                ForegroundCellType.Left => currLevel.maxLeftArrows.Value - usedArrowsOfType > 0,
                ForegroundCellType.Top => currLevel.maxTopArrows.Value - usedArrowsOfType > 0,
                ForegroundCellType.Right => currLevel.maxRightArrows.Value - usedArrowsOfType > 0,
                ForegroundCellType.Bottom => currLevel.maxBottomArrows.Value - usedArrowsOfType > 0,
                _ => throw new ArgumentOutOfRangeException(nameof(nextCellType), nextCellType, null)
            };
        }

        private static int NextCellType(GameGrid<ForegroundCellType> fGrid, int x, int y, int switchAmount = 1)
        {
            var maxValue = Enum.GetValues(typeof(ForegroundCellType)).Cast<int>().Last() + 1;
            var nextCellType = (int)(fGrid.Cell(x, y)) + switchAmount;
            nextCellType %= maxValue;
            return nextCellType;
        }

        private static void SetAmountOfArrows(GameGrid<ForegroundCellType> grid)
        {
            var rightArrowCount = grid.CountElementsOfType(ForegroundCellType.Right);
            var leftArrowCount = grid.CountElementsOfType(ForegroundCellType.Left);
            var topArrowCount = grid.CountElementsOfType(ForegroundCellType.Top);
            var bottomArrowCount = grid.CountElementsOfType(ForegroundCellType.Bottom);

            var currentLevelComponent = IoC.Game.GetComponent<CurrentLevelComponent>();
            currentLevelComponent.topArrows.Value = currentLevelComponent.maxTopArrows.Value - topArrowCount;
            currentLevelComponent.leftArrows.Value = currentLevelComponent.maxLeftArrows.Value - leftArrowCount;
            currentLevelComponent.rightArrows.Value = currentLevelComponent.maxRightArrows.Value - rightArrowCount;
            currentLevelComponent.bottomArrows.Value = currentLevelComponent.maxBottomArrows.Value - bottomArrowCount;
        }
    }
}