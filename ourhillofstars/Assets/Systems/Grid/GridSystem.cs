using System;
using System.Collections;
using System.IO;
using SystemBase.Core;
using SystemBase.Utils;
using Systems.Drescher;
using Systems.GameState;
using Systems.GridRendering;
using Systems.Levels;
using UniRx;
using UnityEngine;

namespace Systems.Grid
{
    [GameSystem(typeof(GridRenderingSystem))]
    public class GridSystem : GameSystem<MainGridComponent>
    {
        public override void Register(MainGridComponent component)
        {
            component.backgroundGrid = new GameGrid<BackgroundCellType>(component.dimensions.x, component.dimensions.y);
            component.foregroundGrid = new GameGrid<ForegroundCellType>(component.dimensions.x, component.dimensions.y);

            component.gridsInitialized.SetValueAndForceNotify(component);

            MessageBroker.Default.Receive<GridLoadMsg>()
                .Subscribe(msg => LoadGrid(component, msg.Level))
                .AddTo(component);

            MessageBroker.Default.Receive<LevelProgressUpdate>()
                .Subscribe(_ => { ClearGrids(component); })
                .AddTo(component);

            MessageBroker.Default.Receive<ShowLevelOverviewMsg>()
                .Subscribe(_ => { ClearGrids(component); })
                .AddTo(component);
        }

        private static void ClearGrids(MainGridComponent component)
        {
            IoC.Game.GetComponent<CurrentLevelComponent>().harvesterRunning.Value = false;
            component.backgroundGrid.Clear();
            component.foregroundGrid.Clear();

            for (var i = 0; i < component.backgroundGrid.Length; i++)
            {
                component.backgroundCells[i].GetComponent<BackgroundCellComponent>().type.Value =
                    BackgroundCellType.Empty;
                
                component.backgroundCells[i].GetComponent<ForegroundCellComponent>().type.Value =
                    ForegroundCellType.Empty;
            }
        }

        private void LoadGrid(MainGridComponent component, Level level)
        {
            var asset = $"Levels/{level.File}";
            var tex = Resources.Load<Sprite>(asset).texture;

            if (!tex) throw new FileNotFoundException(asset);

            var currentLevelComponent = IoC.Game.GetComponent<CurrentLevelComponent>();
            currentLevelComponent.Level = level;
            currentLevelComponent.topArrows.Value = level.TopArrows;
            currentLevelComponent.leftArrows.Value = level.LeftArrows;
            currentLevelComponent.rightArrows.Value = level.RightArrows;
            currentLevelComponent.bottomArrows.Value = level.BottomArrows;

            currentLevelComponent.maxTopArrows.Value = level.TopArrows;
            currentLevelComponent.maxLeftArrows.Value = level.LeftArrows;
            currentLevelComponent.maxRightArrows.Value = level.RightArrows;
            currentLevelComponent.maxBottomArrows.Value = level.BottomArrows;

            Observable.FromCoroutine(() => SetGridCellsFromTexture(component, tex))
                .DoOnCompleted(() => MessageBroker.Default.Publish(new SpawnPlayerMessage
                {
                    InitialDirection = level.StartDirection
                }))
                .Subscribe()
                .AddTo(component);
        }

        private IEnumerator SetGridCellsFromTexture(MainGridComponent component, Texture2D texture)
        {
            for (var y = component.dimensions.y - 1; y >= 0; y--)
            {
                for (var x = 0; x < component.dimensions.x; x++)
                    try
                    {
                        component.backgroundGrid.Cell(x, y, ((Color32)texture.GetPixel(x, y)).ToCell());
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"unknown color at {x}, {y}");
                        Debug.LogException(e);
                    }

                yield return new WaitForSeconds(component.updateAnimationDelay);
            }
        }
    }
}