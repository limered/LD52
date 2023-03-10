using System;
using System.Linq;
using SystemBase.CommonSystems.Audio;
using SystemBase.Core;
using SystemBase.Utils;
using Systems.GameState;
using Systems.Grid;
using Systems.GridRendering;
using Systems.Levels;
using Systems.Selector;
using Systems.Tutorial;
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
            var currLevel = IoC.Game.GetComponent<CurrentLevelComponent>();
            if (currLevel.harvesterRunning.Value) return;
            if (currLevel.IsPaused.Value) return;

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
            if (currLevel.Level.levelType == LevelType.Harvester)
            {
                if (cell != BackgroundCellType.Harvested &&
                    cell != BackgroundCellType.Harvestable &&
                    cell != BackgroundCellType.Path &&
                    cell != BackgroundCellType.Start)
                {
                    selector.shouldChangeTexture.Value = true;
                    return;
                }
            }
            else if (currLevel.Level.levelType == LevelType.ApplePicker)
            {
                if (cell != BackgroundCellType.Path &&
                    cell != BackgroundCellType.Start)
                {
                    selector.shouldChangeTexture.Value = true;
                    return;
                }
            }

            selector.shouldChangeTexture.Value = false;

            if (Input.GetMouseButtonDown(1))
            {
                if (fGrid.Cell(x, y) != ForegroundCellType.Empty) "flupp".Play();
                fGrid.Cell(x, y, ForegroundCellType.Empty);
                SetAmountOfArrows(fGrid);
                MessageBroker.Default.Publish(new TutorialMessage { stepToEnd = TutorialStep.RemoveArrow });
            }

            if (!Input.GetMouseButtonDown(0)) return;
            var nextCellType = (ForegroundCellType)NextCellType(fGrid, x, y);
            switch (nextCellType)
            {
                case ForegroundCellType.Left:
                    MessageBroker.Default.Publish(new TutorialMessage { stepToEnd = TutorialStep.AddArrow });
                    break;
                case ForegroundCellType.Bottom or
                    ForegroundCellType.Top or
                    ForegroundCellType.Right:
                    MessageBroker.Default.Publish(new TutorialMessage { stepToEnd = TutorialStep.RotateArrow });
                    break;
                case ForegroundCellType.Empty:
                    MessageBroker.Default.Publish(new TutorialMessage { stepToEnd = TutorialStep.RemoveArrow });
                    break;
            }

            switch (nextCellType)
            {
                case ForegroundCellType.Top:
                    "top".Play();
                    break;
                case ForegroundCellType.Left:
                    "left".Play();
                    break;
                case ForegroundCellType.Bottom:
                    "down".Play();
                    break;
                case ForegroundCellType.Right:
                    "right".Play();
                    break;
                case ForegroundCellType.Empty:
                    "flupp".Play();
                    break;
            }

            fGrid.Cell(x, y, nextCellType);
            SetAmountOfArrows(fGrid);
        }

        private static int NextCellType(GameGrid<ForegroundCellType> fGrid, int x, int y, int switchAmount = 1)
        {
            var maxValue = Enum.GetValues(typeof(ForegroundCellType)).Cast<int>().Last() + 1;
            var nextCellType = (int)fGrid.Cell(x, y) + switchAmount;
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
            currentLevelComponent.arrowsUsed.Value =
                topArrowCount + rightArrowCount + leftArrowCount + bottomArrowCount;
        }
    }
}