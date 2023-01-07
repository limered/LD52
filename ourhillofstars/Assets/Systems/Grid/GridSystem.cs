﻿using System;
using System.Collections;
using System.IO;
using SystemBase.Core;
using Systems.GridRendering;
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

            component.gridsInitialized.Execute(component);
            MessageBroker.Default.Receive<GridLoadMsg>().Subscribe(msg => LoadGrid(component, msg.Level))
                .AddTo(component);

            Observable.Timer(new TimeSpan(0, 0, 3)).Subscribe(_ => MessageBroker.Default.Publish(new GridLoadMsg
            {
                Level = "Levels/level_1"
            }));
        }

        private void LoadGrid(MainGridComponent component, string asset)
        {
            var tex = Resources.Load<Texture2D>(asset);

            if (!tex)
            {
                throw new FileNotFoundException(asset);
            }

            component.StartCoroutine(SetGridCellsFromTexture(component, tex));
        }

        private IEnumerator SetGridCellsFromTexture(MainGridComponent component, Texture2D texture)
        {
            for (int x = 0; x < component.dimensions.x; x++)
            {
                for (int y = 0; y < component.dimensions.y; y++)
                {
                    try
                    {
                        component.backgroundGrid.SetCell(x, y, ((Color32)texture.GetPixel(x, y)).ToCell());
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
}