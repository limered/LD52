using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public class LevelSystem : GameSystem<LevelOverviewComponent, MainGridComponent>
    {
        public override void Register(LevelOverviewComponent component)
        {
            var cellPrefab = IoC.Game.PrefabByName("LevelButton");
            var levels = GetLevels();
            
            var parentTransform = component.GetComponent<RectTransform>();
            parentTransform.DetachChildren();

            var max = Math.Min(levels.Count, component.gridDimensions.x * component.gridDimensions.y);
            for (var i = 0; i < max; i++)
            {
                var x = i % component.gridDimensions.x;
                var y = i / component.gridDimensions.x;

                var cell = Object.Instantiate(cellPrefab,
                    Vector3.zero, Quaternion.Euler(0, 0, 0),
                    parentTransform);

                cell.GetComponentInChildren<Button>().image.sprite = LoadSprite(levels[i]);
                cell.GetComponentInChildren<TextMeshProUGUI>().text = levels[i].Name;
            }
        }

        public override void Register(MainGridComponent component)
        {
            var levels = GetLevels();

            // load 1st level
            MessageBroker.Default.Publish(new GridLoadMsg
            {
                Level = levels.First()
            });
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