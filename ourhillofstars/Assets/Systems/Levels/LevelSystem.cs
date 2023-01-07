using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SystemBase.Core;
using Systems.Grid;
using UniRx;
using UnityEngine;

namespace Systems.Levels
{
    [GameSystem]
    public class LevelSystem : GameSystem<MainGridComponent>
    {
        public override void Register(MainGridComponent component)
        {
            var levelsJson = Resources.Load<TextAsset>("Levels/levels");
            var levels = JsonConvert.DeserializeObject<List<Level>>(levelsJson.text);
            
            // load 1st level
            MessageBroker.Default.Publish(new GridLoadMsg
            {
                Level = levels.First()
            });
        }
    }
}