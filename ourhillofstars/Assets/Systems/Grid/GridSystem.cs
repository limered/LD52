using System;
using System.Collections;
using System.Collections.Generic;
using SystemBase.Core;
using SystemBase.Utils;
using Systems.Drescher;
using Systems.GameState;
using Systems.GridRendering;
using Systems.Levels;
using Systems.Levels.Events;
using Systems.Theme;
using UniRx;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.U2D;

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
            
            MessageBroker.Default.Receive<AskToGoToNextLevelMsg>()
                .Subscribe(_ => { ClearGrids(component, true); })
                .AddTo(component);

            MessageBroker.Default.Receive<ShowLevelOverviewMsg>()
                .Subscribe(_ => { ClearGrids(component, false); })
                .AddTo(component);
            
            MessageBroker.Default.Receive<ClearArrowsMsg>()
                .Subscribe(_ => { ClearArrows(component); })
                .AddTo(component);
        }

        private void ClearArrows(MainGridComponent component)
        {
            component.foregroundGrid.Clear();
            
            for (var i = 0; i < component.backgroundGrid.Length; i++)
            {
                component.foregroundCells[i].GetComponent<ForegroundCellComponent>().type.Value =
                    ForegroundCellType.Empty;
            }
            
            var currentGame = IoC.Game.GetComponent<CurrentLevelComponent>();
            currentGame.arrowsUsed.Value = 0;
        }

        private static void ClearGrids(MainGridComponent component, bool levelDone)
        {
            var currentGame = IoC.Game.GetComponent<CurrentLevelComponent>();
            currentGame.harvesterRunning.Value = false;
            component.foregroundGrid.Clear();

            if (!levelDone)
            {
                component.backgroundGrid.Clear();
                for (var i = 0; i < component.backgroundGrid.Length; i++)
                {
                    component.backgroundCells[i].GetComponent<BackgroundCellComponent>().type.Value =
                        BackgroundCellType.Empty;
                    component.foregroundCells[i].GetComponent<ForegroundCellComponent>().type.Value =
                        ForegroundCellType.Empty;
                }
                return;
            }

            for (var i = 0; i < component.backgroundGrid.Length; i++)
            {
                component.foregroundCells[i].GetComponent<ForegroundCellComponent>().type.Value =
                    ForegroundCellType.Empty;
            }
            
            Observable.FromCoroutine(() => ResetGridCellsFromTexture(component))
                .DoOnCompleted(() => MessageBroker.Default.Publish(new GoToNextLevelMsg()
                {
                    CompletedLevel = currentGame.Level.LevelIndex,
                    Grade = currentGame.CurrentGrade
                }))
                .Subscribe()
                .AddTo(component);
        }

        private void LoadGrid(MainGridComponent component, Level level)
        {
            var currentLevelComponent = IoC.Game.GetComponent<CurrentLevelComponent>();
            currentLevelComponent.Level = level;
            currentLevelComponent.arrowsUsed.Value = 0;

            var theme = IoC.Game.GetComponent<ThemeComponent>();
            var levelTheme = theme.levelThemes[currentLevelComponent.Level.themeFile];
            var sprites = new List<Texture2D>(11);
            for (var y = 0; y < 2; y++)
            {
                for (var x = 0; x < 5; x++)
                {
                    var spriteY = y == 0 ? 1 : 0;
                    var texture = new Texture2D(32, 32, TextureFormat.RGB24, false)
                    {
                        filterMode = FilterMode.Point
                    };
                    var pixels = levelTheme.GetPixels(x * 32, spriteY * 32, 32, 32);
                    texture.SetPixels(pixels);
                    texture.Apply();
                    sprites.Add(texture);
                }
            }

            sprites.Insert(0, theme.backGroundBlankTexture);
            var textureArray = sprites.ToArray();
            for (var i = 0; i < component.backgroundCells.Length; i++)
            {
                component.backgroundCells[i].GetComponent<BackgroundCellComponent>().images = textureArray;
            }
            
            Observable.FromCoroutine(() => SetGridCellsFromTexture(component, level.LoadImage().texture))
                .DoOnCompleted(() => MessageBroker.Default.Publish(new SpawnPlayerMessage
                {
                    InitialDirection = level.startDirection
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
        
        private static IEnumerator ResetGridCellsFromTexture(MainGridComponent component)
        {
            for (var y = component.dimensions.y - 1; y >= 0; y--)
            {
                for (var x = 0; x < component.dimensions.x; x++)
                    component.backgroundGrid.Cell(x, y, BackgroundCellType.Empty);

                yield return new WaitForSeconds(component.updateAnimationDelay);
            }
        }
    }
}