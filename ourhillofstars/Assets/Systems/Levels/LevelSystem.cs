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
        public override void Register(LevelOverviewComponent component)
        {
            var cellPrefab = IoC.Game.PrefabByName("LevelButton");
            var levels = GetLevels();

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

            var max = Math.Min(levels.Count, component.gridDimensions.x * component.gridDimensions.y);
            for (var i = 0; i < max; i++)
            {
                var cell = Object.Instantiate(cellPrefab,
                    Vector3.zero, Quaternion.Euler(0, 0, 0),
                    parentTransform);

                cell.GetComponentInChildren<LevelCellComponent>().level = i;
                cell.GetComponentInChildren<Button>().image.sprite = LoadSprite(levels[i]);
                cell.GetComponentInChildren<TextMeshProUGUI>().text = levels[i].Name;
            }

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
                .Subscribe(_ => component.gameObject.transform.parent.gameObject.SetActive(true));
        }

        private List<Level> GetLevels()
        {
            var levelsJson = Resources.Load<TextAsset>("Levels/levels");
            return JsonConvert.DeserializeObject<List<Level>>(levelsJson.text);
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