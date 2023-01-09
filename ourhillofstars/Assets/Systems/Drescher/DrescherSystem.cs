using System;
using System.Collections.Generic;
using SystemBase.CommonSystems.Audio;
using SystemBase.Core;
using SystemBase.Utils;
using Systems.GameState;
using Systems.Grid;
using Systems.Levels;
using Systems.Levels.Events;
using Systems.Theme;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Systems.Drescher
{
    [GameSystem]
    public class DrescherSystem : GameSystem<DrescherComponent, MainGridComponent>
    {
        private static CurrentLevelComponent _currentLevelComponent;
        private readonly ReactiveProperty<MainGridComponent> _grid = new();

        public override void Register(DrescherComponent component)
        {
            component.rendererCache = component.GetComponent<Renderer>();
            component.direction
                .Subscribe(dir => UpdateDirection(dir, component))
                .AddTo(component);

            _grid.WhereNotNull()
                .Subscribe(g =>
                {
                    SystemUpdate(component)
                        .Subscribe(drescher => Drive(drescher, g))
                        .AddTo(component);
                })
                .AddTo(component);

            MessageBroker.Default.Receive<SpawnPlayerMessage>()
                .Subscribe(_ => Object.Destroy(component.gameObject))
                .AddTo(component);

            MessageBroker.Default.Receive<AskToGoToNextLevelMsg>()
                .Subscribe(_ => Object.Destroy(component.gameObject))
                .AddTo(component);

            _currentLevelComponent = IoC.Game.GetComponent<CurrentLevelComponent>();
            
            "tractor".Play(new PlaySFXParameters { Loop = true });
        }

        private void SpawnDrescher(MainGridComponent grid)
        {
            var startCoord = grid.backgroundGrid.FindStartCoord();
            if (startCoord == null) throw new NullReferenceException("Level Missing Startpoint");
            var position = new Vector3((float)startCoord?.x, 0.5f, (float)startCoord?.y);
            var drescherPrefab = IoC.Game.PrefabByName("Drescher");
            var drescher = Object.Instantiate(drescherPrefab, position, drescherPrefab.transform.rotation);
            var target = new Vector2Int((int)startCoord?.x, (int)startCoord?.y);
            var dresherObject = drescher.GetComponent<DrescherComponent>();
            dresherObject.targetCellCoord = target;

            var currentLevelComponent = IoC.Game.GetComponent<CurrentLevelComponent>();
            var themeComponent = IoC.Game.GetComponent<ThemeComponent>();
            var theme = themeComponent.harvesterThemes[currentLevelComponent.Level.playerThemeFile];
            var sprites = new List<Texture2D>(4);
            for (var i = 0; i < 4; i++)
            {
                var texture = new Texture2D(32, 32, TextureFormat.RGBA32, false)
                {
                    filterMode = FilterMode.Point,
                };
                var pixels = theme.GetPixels(i * 32, 0, 32, 32);
                texture.SetPixels(pixels);
                texture.Apply();
                sprites.Add(texture);
            }

            var textureArray = new[]
            {
                sprites[1],
                sprites[0],
                sprites[3],
                sprites[2]
            };
            dresherObject.directionImages = textureArray;

            //set particle theme
            var particleSystemRenderer = dresherObject.GetComponentInChildren<ParticleSystemRenderer>();
            var particleThemeSprite = IoC.Game.GetComponent<ThemeComponent>()
                .harvestParticleThemes[currentLevelComponent.Level.themeFile];
            particleSystemRenderer.material.mainTexture = particleThemeSprite.texture;
        }

        private static void Drive(DrescherComponent drescherComponent, MainGridComponent g)
        {
            var startCoord = g.backgroundGrid.FindStartCoord();
            if (startCoord == null) return;

            if (!_currentLevelComponent.harvesterRunning.Value)
            {
                drescherComponent.Reset((Vector2Int)startCoord, _currentLevelComponent.Level.startDirection);
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
        }

        private static void Harvest(MainGridComponent g, Vector2 position)
        {
            if (_currentLevelComponent.Level.levelType == LevelType.Harvester)
            {
                var cellCoord = new Vector2Int((int)(position.x + 0.5f), (int)(position.y + 0.5f));
                var currentCelType = g.backgroundGrid.Cell(cellCoord.x, cellCoord.y);
                if (currentCelType != BackgroundCellType.Harvestable) return;

                g.backgroundGrid.Cell(cellCoord.x, cellCoord.y, BackgroundCellType.Harvested);
                MessageBroker.Default.Publish(new HarvestedMsg { coord = cellCoord });
                new[] { "harvest1", "harvest2", "harvest3", "harvest4" }.PlayRandom();
            }
            else if (_currentLevelComponent.Level.levelType == LevelType.ApplePicker)
            {
                var cellCoords = new Vector2Int[]
                {
                    new((int)(position.x + 0.5f), (int)(position.y + 1.5f)),
                    new((int)(position.x + 0.5f), (int)(position.y - 0.5f)),
                    new((int)(position.x + 1.5f), (int)(position.y + 0.5f)),
                    new((int)(position.x - 0.5f), (int)(position.y + 0.5f))
                };
                for (var i = 0; i < cellCoords.Length; i++)
                {
                    if (cellCoords[i].x < 0 || cellCoords[i].y < 0 || cellCoords[i].x >= g.dimensions.x ||
                        cellCoords[i].y >= g.dimensions.y)
                    {
                        continue;
                    }
                    
                    var currentCelType = g.backgroundGrid.Cell(cellCoords[i].x, cellCoords[i].y);
                    if (currentCelType != BackgroundCellType.Harvestable) continue;
                    g.backgroundGrid.Cell(cellCoords[i].x, cellCoords[i].y, BackgroundCellType.Harvested);
                    MessageBroker.Default.Publish(new HarvestedMsg { coord = cellCoords[i] });
                }
            }

            // level complete
            if (g.backgroundGrid.CountElementsOfType(BackgroundCellType.Harvestable) <= 0)
            {
                new[]
                {
                    "complete_level1",
                    "complete_level2",
                    "complete_level3",
                    "complete_level4",
                    "complete_level5",
                }.PlayRandom();

                MessageBroker.Default.Publish(
                    new LevelCompleteMsg
                    {
                        CompletedLevel = _currentLevelComponent.Level.LevelIndex,
                        Grade = _currentLevelComponent.CurrentGrade
                    });
            }
        }

        private static void CheckNextCellAndSwitchTarget(DrescherComponent drescherComponent, MainGridComponent g)
        {
            var nextCellCoord = drescherComponent.targetCellCoord;
            var maxX = g.dimensions.x;
            var maxY = g.dimensions.y;
            switch (drescherComponent.direction.Value)
            {
                case DrescherDirection.Up:
                    nextCellCoord.y += 1;
                    break;
                case DrescherDirection.Left:
                    nextCellCoord.x -= 1;
                    break;
                case DrescherDirection.Down:
                    nextCellCoord.y -= 1;
                    break;
                case DrescherDirection.Right:
                    nextCellCoord.x += 1;
                    break;
            }

            if (nextCellCoord.x < 0 || nextCellCoord.x >= maxX || nextCellCoord.y < 0 ||
                nextCellCoord.y >= maxY) return;

            var nextCellType = g.backgroundGrid.Cell(nextCellCoord.x, nextCellCoord.y);
            if (_currentLevelComponent.Level.levelType == LevelType.Harvester)
            {
                if (nextCellType != BackgroundCellType.Harvested &&
                    nextCellType != BackgroundCellType.Harvestable &&
                    nextCellType != BackgroundCellType.Start &&
                    nextCellType != BackgroundCellType.Path) return;
            }
            else if (_currentLevelComponent.Level.levelType == LevelType.ApplePicker)
            {
                if (nextCellType != BackgroundCellType.Start &&
                    nextCellType != BackgroundCellType.Path) return;
            }

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
                    newDirection = DrescherDirection.Left;
                    break;
                case ForegroundCellType.Top:
                    newDirection = DrescherDirection.Up;
                    break;
                case ForegroundCellType.Right:
                    newDirection = DrescherDirection.Right;
                    break;
                case ForegroundCellType.Bottom:
                    newDirection = DrescherDirection.Down;
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

        private static void UpdateDirection(DrescherDirection direction, DrescherComponent component)
        {
            component.rendererCache.material.mainTexture = component.directionImages[(int)direction];
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