using System;
using System.Collections;
using System.IO;
using SystemBase.Core;
using SystemBase.Utils;
using Systems.Drescher;
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

            MessageBroker.Default.Receive<GridLoadMsg>().Subscribe(msg => LoadGrid(component, msg.Level))
                .AddTo(component);
        }

        public void LoadGrid(MainGridComponent component, Level level)
        {
            var asset = $"Levels/{level.File}";
            var tex = Resources.Load<Texture2D>(asset);

            if (!tex)
            {
                throw new FileNotFoundException(asset);
            }
            
            var currentLevelComponent = IoC.Game.GetComponent<CurrentLevelComponent>();
            currentLevelComponent.topArrows.Value = level.TopArrows;
            currentLevelComponent.leftArrows.Value = level.LeftArrows;
            currentLevelComponent.rightArrows.Value = level.RightArrows;
            currentLevelComponent.bottomArrows.Value = level.BottomArrows;

            Observable.FromCoroutine(() => SetGridCellsFromTexture(component, tex))
                .DoOnCompleted(() => MessageBroker.Default.Publish(new SpawnPlayerMessage()))
                .Subscribe()
                .AddTo(component);
        }

        private IEnumerator SetGridCellsFromTexture(MainGridComponent component, Texture2D texture)
        {
            for (int y = component.dimensions.y - 1; y >= 0; y--)
            {
                for (int x = 0; x < component.dimensions.x; x++)
                {
                    try
                    {
                        component.backgroundGrid.Cell(x, y, ((Color32)texture.GetPixel(x, y)).ToCell());
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"unknown color at {x}, {y}");
                        Debug.LogException(e);
                    }
                }

                yield return new WaitForSeconds(component.updateAnimationDelay);
            }
        }
    }
}