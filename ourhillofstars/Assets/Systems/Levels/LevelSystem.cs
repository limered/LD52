using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SystemBase.Core;
using SystemBase.Utils;
using Systems.Grid;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Systems.Levels
{
    [GameSystem]
    public class LevelSystem : GameSystem<LevelOverviewComponent>
    {
        public const string LevelProgressSettingsKey = "furthest_level";

        public LevelSystem()
        {
            MessageBroker.Default.Receive<LevelProgressUpdate>().Subscribe(msg =>
            {
                PlayerPrefs.SetInt(LevelProgressSettingsKey, msg.FurthestLevel);
            });
        }

        public override void Register(LevelOverviewComponent component)
        {
            var levels = GetLevels();
            ReloadLevelOverview(levels, component);
            HandleMessages(levels, component);
        }

        private void ReloadLevelOverview(List<Level> levels, LevelOverviewComponent component)
        {
            var cellPrefab = IoC.Game.PrefabByName("LevelButton");

            var parentTransform = component.GetComponent<RectTransform>();
            var layoutGroup = component.GetComponent<GridLayoutGroup>();
            var biggerSize =
                Math.Max((parentTransform.rect.size.x - component.gridDimensions.x * layoutGroup.spacing.x) /
                         component.gridDimensions.x,
                    (parentTransform.rect.size.y - component.gridDimensions.y * layoutGroup.spacing.y) /
                    component.gridDimensions.y);

            var cellSize = new Vector2(biggerSize, biggerSize);
            layoutGroup.cellSize = cellSize;

            parentTransform.DetachChildren();

            var furthestLevel = PlayerPrefs.GetInt(LevelProgressSettingsKey, 0);

            var max = Math.Min(levels.Count, component.gridDimensions.x * component.gridDimensions.y);
            for (var i = 0; i < max; i++)
            {
                var cell = Object.Instantiate(cellPrefab,
                    Vector3.zero, Quaternion.Euler(0, 0, 0),
                    parentTransform);

                cell.GetComponentInChildren<TextMeshProUGUI>().text = levels[i].Name;
                if (i <= furthestLevel)
                {
                    cell.GetComponentInChildren<LevelCellComponent>().level = i;
                    cell.GetComponentInChildren<Button>().image.sprite = LoadSprite(levels[i]);
                }
                else
                {
                    cell.GetComponentInChildren<LevelCellComponent>().level = null;
                    cell.GetComponentInChildren<Button>().image.sprite = null;
                    cell.GetComponentInChildren<TextMeshProUGUI>().color = Color.grey;
                }
            }
        }

        private void HandleMessages(List<Level> levels, LevelOverviewComponent component)
        {
            MessageBroker.Default.Receive<LoadLevelMsg>().Subscribe(msg =>
                {
                    component.transform.parent.gameObject.SetActive(false);
                    MessageBroker.Default.Publish(new GridLoadMsg
                    {
                        Level = levels[msg.LevelIndex]
                    });
                })
                .AddTo(component);

            MessageBroker.Default.Receive<ShowLevelOverviewMsg>()
                .Subscribe(_ =>
                {
                    component.gameObject.transform.parent.gameObject.SetActive(true);
                    ReloadLevelOverview(levels, component);
                });
        }

        private List<Level> GetLevels()
        {
            var levelsJson = Resources.Load<TextAsset>("Levels/levels");
            var levels = JsonConvert.DeserializeObject<List<Level>>(levelsJson.text);
            for (int i = 0; i < levels.Count; i++)
            {
                levels[i].Index = i;
            }

            return levels;
        }

        private Sprite LoadSprite(Level level)
        {
            var asset = $"Levels/{level.File}";
            var sprite = Resources.Load<Sprite>(asset);

            if (!sprite)
            {
                throw new FileNotFoundException(asset);
            }

            return sprite;
        }
    }
}